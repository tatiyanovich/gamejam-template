import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { neighbourComposer } from './neighbours.mjs';

export function vfxOutputs(root) {
  const layout = JSON.parse(readFileSync(join(root, 'src/d10/layout.json'), 'utf8'));
  const sources = Object.fromEntries(layout.assets.map(asset => [asset.name,
    readFileSync(join(root, `src/d10/${asset.name}.svg`), 'utf8')]));
  const { frame, label, scene } = neighbourComposer(root);
  let instance = 0;

  function body(markup) {
    const prefix = `d10-${instance++}-`;
    return markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  }
  function sprite(name, x, y, scale = 1, rotation = 0, opacity = 1) {
    const asset = layout.assets.find(item => item.name === name);
    const [px, py] = asset.pivotPixels;
    return `<g opacity="${opacity}" transform="translate(${x} ${y}) rotate(${rotation}) scale(${scale}) translate(${-px} ${-py})">${body(sources[name])}</g>`;
  }
  function burst(name, x, y, count, radius, scale) {
    let content = '';
    for (let index = 0; index < count; index++) {
      const angle = -Math.PI / 2 + (index - (count - 1) / 2) * (Math.PI / (count + 1));
      const distance = radius * (0.55 + 0.45 * ((index * 7) % 5) / 4);
      content += sprite(name, x + Math.cos(angle) * distance, y + Math.sin(angle) * distance,
        scale, (index % 2 ? 14 : -14), 1 - 0.5 * distance / radius);
    }
    return content;
  }
  function trail(name, x, y, count, step, scale) {
    let content = '';
    for (let index = 0; index < count; index++) {
      content += sprite(name, x + (index % 2 ? 16 : -16), y + index * step, scale * (1 - index * 0.12),
        0, 1 - index * 0.2);
    }
    return content;
  }
  function effect(name) {
    return layout.effects.find(item => item.name === name);
  }
  function overlay() {
    const sweat = effect('sweat');
    const alert = effect('alert');
    const chalk = effect('chalk');
    const sparkles = effect('sparkles');
    return trail('sweat_drop', sweat.emitter[0] - 96, sweat.emitter[1] - 40, 3, 46, 0.85)
      + sprite('exclaim', alert.emitter[0], alert.emitter[1], 1.1)
      + burst('note', effect('laughter_left').emitter[0], effect('laughter_left').emitter[1], 5, 120, 0.8)
      + burst('note', effect('laughter_right').emitter[0], effect('laughter_right').emitter[1], 5, 120, 0.8)
      + trail('chalk_dust', chalk.emitter[0], chalk.emitter[1], 3, 24, 0.5)
      + burst('sparkle', sparkles.emitter[0], sparkles.emitter[1], 7, 130, 0.85);
  }
  function stateSheet() {
    let content = '<rect width="1560" height="620" fill="{{PAPER}}"/>'
      + label(35, 54, 'COPYCAT / D10 / VFX SPRITES', 34)
      + label(35, 94, 'Seven 64x64 particle sprites. PNG @2x, PPU 200, pivot centred, no trim.');
    layout.assets.forEach((asset, index) => {
      const x = 30 + index * 218;
      content += `<rect x="${x}" y="128" width="200" height="300" rx="18" fill="{{WALL_LIGHT}}"/>`;
      content += sprite(asset.name, x + 100, 248, 2.4);
      content += label(x + 16, 396, asset.name, 20);
      content += label(x + 16, 420, `${asset.size.join(' x ')}`, 16);
    });
    layout.effects.forEach((item, index) => {
      const x = 35 + (index % 4) * 380;
      const y = 486 + Math.floor(index / 4) * 52;
      content += sprite(item.sprite, x + 14, y - 6, 0.55);
      content += label(x + 40, y, `${item.name} — ${item.trigger}`, 19);
    });
    return frame(1560, 620, content);
  }
  function classroom(scale) {
    return scene(0, { afterKitten: overlay }).replace(/^<svg[^>]*>/, match =>
      match.replace('width="1920"', `width="${1920 * scale}"`).replace('height="1080"', `height="${1080 * scale}"`));
  }
  for (const asset of layout.assets) {
    const [, width, height] = sources[asset.name].match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (+width !== asset.size[0] || +height !== asset.size[1]) throw new Error(`D10 size mismatch: ${asset.name}`);
    if (/<text\b/.test(sources[asset.name])) throw new Error(`D10 sprite contains text: ${asset.name}`);
  }
  for (const item of layout.effects) {
    if (!sources[item.sprite]) throw new Error(`D10 effect ${item.name} uses an unknown sprite`);
    if (item.emitter && item.emitter.some((value, index) => value < 0 || value > [1920, 1080][index]))
      throw new Error(`D10 effect ${item.name} emits outside the frame`);
  }
  if (layout.assets.length !== 7) throw new Error('D10 requires 7 sprites');
  return {
    'exports/d10': layout.assets.map(asset => ({ name: asset.name, markup: sources[asset.name], scale: layout.textureScale })),
    'previews/d10': [
      { name: 'state_sheet', markup: stateSheet(), scale: 1 },
      { name: 'classroom_vfx', markup: classroom(1), scale: 1 },
      { name: 'classroom_thumbnail', markup: classroom(1), scale: 0.25 }
    ]
  };
}
