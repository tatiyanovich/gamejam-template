import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { neighbourComposer } from './neighbours.mjs';

export function duckOutputs(root) {
  const layout = JSON.parse(readFileSync(join(root, 'src/d7/layout.json'), 'utf8'));
  const read = name => readFileSync(join(root, `src/d7/${name}.svg`), 'utf8');
  const keycapTemplate = read(layout.keycapTemplate);
  const sources = {};
  for (const asset of layout.assets) {
    const keycap = Object.entries(layout.keycaps).find(([key]) => asset.name === `keycap_${key}`);
    sources[asset.name] = keycap ? keycapTemplate.replace('{{KEYCAP_LABEL}}', keycap[1]) : read(asset.name);
  }
  const neighbours = neighbourComposer(root);
  const { frame, label } = neighbours;
  let instance = 0;

  function body(markup) {
    const prefix = `d7-${instance++}-`;
    return markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  }
  function sprite(name, x, y, scale = 1, rotation = 0) {
    const asset = layout.assets.find(item => item.name === name);
    const [px, py] = asset.pivotPixels;
    return `<g transform="translate(${x} ${y}) rotate(${rotation}) scale(${scale}) translate(${-px} ${-py})">${body(sources[name])}</g>`;
  }
  function shadow(cx, cy, rx, ry, opacity) {
    return `<ellipse cx="${cx}" cy="${cy}" rx="${rx}" ry="${ry}" fill="{{INK}}" opacity="${opacity}"/>`;
  }
  function deskDuck(options = {}) {
    const { duck, shadow: drop, keycap } = layout.desk;
    const scale = duck.scale * (options.hover ? layout.desk.hoverScale : 1);
    return shadow(drop.cx, drop.cy, drop.rx * scale, drop.ry, drop.opacity)
      + sprite('duck_idle', duck.x, duck.y, scale)
      + sprite('keycap_q', keycap.x, keycap.y, keycap.scale);
  }
  function arcPoint(t) {
    const [a, b, c] = layout.throw.points;
    const w = [(1 - t) * (1 - t), 2 * (1 - t) * t, t * t];
    const control = key => (b[key] - 0.25 * a[key] - 0.25 * c[key]) / 0.5;
    return {
      x: w[0] * a.x + w[1] * control('x') + w[2] * c.x,
      y: w[0] * a.y + w[1] * control('y') + w[2] * c.y,
      scale: a.scale + (c.scale - a.scale) * t
    };
  }
  function flightFrame(t) {
    const frames = layout.throw.frames;
    const index = Math.floor(t * layout.throw.durationSeconds / layout.throw.frameSwapSeconds) % frames.length;
    return frames[index];
  }
  function throwArc(steps) {
    let content = '';
    const [, , landing] = layout.throw.points;
    const drop = layout.throw.landingShadow;
    content += shadow(landing.x, landing.y + 56 * landing.scale, drop.rx, drop.ry, drop.opacity);
    for (let step = 0; step <= steps; step++) {
      const t = step / steps;
      const point = arcPoint(t);
      const opacity = step === steps ? 1 : 0.35 + 0.45 * t;
      content += `<g opacity="${opacity.toFixed(2)}">${sprite(flightFrame(t), point.x, point.y, point.scale, layout.throw.rotationDegrees * t)}</g>`;
    }
    return content;
  }
  function confiscatedDuck() {
    const { sprite: name, x, y, scale } = layout.confiscated;
    return shadow(x, y + 3, 62 * scale, 5, 0.12) + sprite(name, x, y, scale);
  }
  function scene(content) {
    return neighbours.scene(0, { beforeKitten: () => content });
  }
  function stateSheet() {
    let content = '<rect width="1920" height="1000" fill="{{WALL}}"/>' + label(35, 52, 'COPYCAT / D7 / EMOTIONAL SUPPORT DUCK', 34);
    content += label(35, 92, 'Idle on the desk with keycap Q, hover x1.1, two flight frames (rotation is code), sad on the teacher desk after the third throw.');
    const cell = (x, y, caption, markup) => `<rect x="${x}" y="${y}" width="360" height="360" rx="20" fill="{{WALL_LIGHT}}"/>` + label(x + 16, y + 34, caption, 20) + `<g transform="translate(${x + 180} ${y + 210})">${markup}</g>`;
    content += cell(35, 120, 'duck_idle + keycap_q', shadow(0, 62, 62, 10, .12) + sprite('duck_idle', 0, 58) + sprite('keycap_q', 0, 104));
    content += cell(415, 120, 'hover x1.1 / cursor hand', shadow(0, 62, 68, 10, .12) + sprite('duck_idle', 0, 58, layout.desk.hoverScale) + sprite('keycap_q', 0, 104));
    content += cell(795, 120, 'duck_fly_1 / wings up', sprite('duck_fly_1', 0, 0, 1.4));
    content += cell(1175, 120, 'duck_fly_2 / wings down, SQUEAK', sprite('duck_fly_2', 0, 0, 1.4));
    content += cell(1555, 120, 'duck_sad / confiscated', shadow(0, 62, 62, 10, .12) + sprite('duck_sad', 0, 58, 1.2));
    content += `<rect x="35" y="500" width="1850" height="420" rx="20" fill="{{WALL_LIGHT}}"/>` + label(51, 534, 'Throw: 0.6 s, 720 deg, frame swap every 0.1 s, scale 1 -> 0.5 towards the board', 20);
    for (let step = 0; step <= 6; step++) {
      const t = step / 6;
      const x = 170 + step * 265;
      content += sprite(flightFrame(t), x, 700, 1.2, layout.throw.rotationDegrees * t);
      content += label(x - 60, 860, `t=${(t * layout.throw.durationSeconds).toFixed(1)}s / ${Math.round(layout.throw.rotationDegrees * t)} deg`, 18);
      content += label(x - 60, 888, flightFrame(t), 16);
    }
    content += label(35, 968, 'Duck frames 140x130, keycap 64x64 - PNG @2x, PPU 200, no trim. Import D11, throw tween E4, gameplay Gameplay/Duck.', 21);
    return frame(1920, 1000, content);
  }
  function layerSheet() {
    let content = '<rect width="1300" height="420" fill="{{PAPER}}"/>' + label(30, 52, 'D7 / SPRITES AND PIVOTS', 32);
    layout.assets.forEach((asset, index) => {
      const x = 25 + index * 250;
      const y = 85;
      const scale = Math.min(200 / asset.size[0], 180 / asset.size[1], 2);
      content += `<rect x="${x}" y="${y}" width="230" height="300" rx="15" fill="{{WALL_LIGHT}}"/>`;
      content += `<g transform="translate(${x + (230 - asset.size[0] * scale) / 2} ${y + 20 + (180 - asset.size[1] * scale) / 2}) scale(${scale})">${body(sources[asset.name])}<circle cx="${asset.pivotPixels[0]}" cy="${asset.pivotPixels[1]}" r="${5 / scale}" fill="{{DANGER}}"/></g>`;
      content += label(x + 10, y + 246, asset.name, 17) + label(x + 10, y + 274, `${asset.size.join(' x ')} / pivot ${asset.pivotPixels.join(', ')}`, 14);
    });
    return frame(1300, 420, content);
  }
  for (const asset of layout.assets) {
    const [, width, height] = sources[asset.name].match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (+width !== asset.size[0] || +height !== asset.size[1]) throw new Error(`D7 size mismatch: ${asset.name}`);
    if (asset.pivotPixels.some((value, index) => value < 0 || value > asset.size[index])) throw new Error(`D7 invalid pivot: ${asset.name}`);
    if (/\{\{KEYCAP_/.test(sources[asset.name])) throw new Error(`D7 unresolved keycap token: ${asset.name}`);
    if (!asset.name.startsWith('keycap_') && /<text\b/.test(sources[asset.name])) throw new Error(`D7 sprite contains text: ${asset.name}`);
  }
  if (new Set(layout.assets.map(asset => asset.name)).size !== 5) throw new Error('D7 requires 5 unique sprites');
  return {
    'exports/d7': layout.assets.map(asset => ({ name: asset.name, markup: sources[asset.name], scale: layout.textureScale })),
    'previews/d7': [
      { name: 'state_sheet', markup: stateSheet(), scale: 1 },
      { name: 'layer_sheet', markup: layerSheet(), scale: 1 },
      { name: 'classroom_desk', markup: scene(deskDuck()), scale: 1 },
      { name: 'classroom_hover', markup: scene(deskDuck({ hover: true })), scale: 1 },
      { name: 'classroom_throw', markup: scene(throwArc(6)), scale: 1 },
      { name: 'classroom_confiscated', markup: scene(confiscatedDuck()), scale: 1 },
      { name: 'classroom_thumbnail', markup: scene(deskDuck()), scale: .25 }
    ]
  };
}
