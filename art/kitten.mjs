import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { classroomOutputs } from './classroom.mjs';

export function kittenOutputs(root) {
  const rig = JSON.parse(readFileSync(join(root, 'src/d3/rig.json'), 'utf8'));
  const sources = Object.fromEntries(rig.assets.map(asset => [asset.name,
    readFileSync(join(root, `src/d3/${asset.name}.svg`), 'utf8')]));
  let instance = 0;
  const frame = (width, height, content) => `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">${content}</svg>`;
  const label = (x, y, text, size = 22) => `<text x="${x}" y="${y}" font-family="Nunito" font-weight="700" font-size="${size}" fill="{{INK}}">${text}</text>`;

  function body(markup) {
    const prefix = `d3-${instance++}-`;
    return markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  }

  function sprite(name, position, rotation = 0, scale = 1) {
    const asset = rig.assets.find(item => item.name === name);
    if (!asset) throw new Error(`D3 missing asset: ${name}`);
    const [px, py] = asset.pivotPixels;
    return `<g transform="translate(${position.join(' ')}) rotate(${rotation}) scale(${scale}) translate(${-px} ${-py})">${body(sources[name])}</g>`;
  }

  function kitten(poseName, motion = {}) {
    const pose = rig.poses[poseName];
    const profile = rig.headProfiles[pose.profile];
    const expression = rig.expressions[pose.expression];
    let content = '';
    for (const node of rig.nodes) {
      let position = [...node.position];
      let name = node.asset;
      let rotation = 0;
      if (node.name === 'tail') rotation = motion.tail || 0;
      if (node.name === rig.motion.typing.node) rotation = motion.paw || 0;
      if (node.name === 'head') name = profile.asset;
      if (node.parent === 'head') {
        const offset = profile.earOffsets[node.name === 'earLeft' ? 0 : 1];
        const head = rig.nodes.find(item => item.name === 'head');
        position = position.map((value, index) => value + head.position[index] + offset[index]);
      }
      content += sprite(name, position, rotation);
    }
    for (const position of profile.eyes) {
      content += sprite(expression.eyeAsset, position, 0, expression.eyeScale);
      if (expression.pupilScale > 0) {
        content += sprite('kitten_pupil', position.map((value, index) => value + profile.gaze[index]), 0, expression.pupilScale);
      }
    }
    const [px, py] = rig.rootPivot;
    return `<g transform="translate(${pose.x} 0) translate(${px} ${py}) rotate(${pose.rotation}) scale(${motion.breath || 1}) translate(${-px} ${-py})">${content}</g>`;
  }

  function poseSheet() {
    let content = '<rect width="1800" height="1120" fill="{{PAPER}}"/>';
    content += label(35, 50, 'COPYCAT / D3 / THE UNPREPARED ONE', 34);
    content += label(35, 85, 'Separate ears, paws, head and eyes / normal, panic and squint / both directions');
    const poses = ['idle', 'lean_left', 'lean_right', 'panic_left', 'panic_right', 'squint_left', 'squint_right'];
    poses.forEach((name, index) => {
      const x = 30 + index % 4 * 445;
      const y = 115 + Math.floor(index / 4) * 490;
      content += `<rect x="${x}" y="${y}" width="420" height="465" rx="20" fill="{{WALL_LIGHT}}"/>`;
      content += `<g transform="translate(${x + 65 - rig.poses[name].x * .6} ${y + 80}) scale(.6)">${kitten(name)}</g>`;
      content += label(x + 22, y + 437, name.replaceAll('_', ' ').toUpperCase(), 24);
    });
    content += label(1390, 680, 'CUT-OUT READY', 28);
    content += label(1390, 728, '12 sprites / PNG @2x');
    content += label(1390, 765, 'Pivots and poses: rig.json');
    content += label(1390, 802, 'No trim / PPU 200');
    content += label(1390, 839, 'Unity import: D11');
    return frame(1800, 1120, content);
  }

  function layerSheet() {
    let content = '<rect width="1600" height="1420" fill="{{PAPER}}"/>';
    content += label(30, 50, 'D3 / SPRITES AND PIVOTS', 32);
    rig.assets.forEach((asset, index) => {
      const x = 25 + index % 4 * 395;
      const y = 85 + Math.floor(index / 4) * 440;
      const scale = Math.min(350 / asset.size[0], 320 / asset.size[1], 2);
      const left = x + (375 - asset.size[0] * scale) / 2;
      content += `<rect x="${x}" y="${y}" width="375" height="415" rx="14" fill="{{WALL_LIGHT}}"/>`;
      content += `<g transform="translate(${left} ${y + 10}) scale(${scale})">${body(sources[asset.name])}
        <circle cx="${asset.pivotPixels[0]}" cy="${asset.pivotPixels[1]}" r="5" fill="{{DANGER}}"/>
      </g>`;
      content += label(x + 10, y + 355, asset.name, 18);
      content += label(x + 10, y + 388, `${asset.size.join(' x ')} / pivot ${asset.pivotPixels.join(', ')}`, 17);
    });
    return frame(1600, 1420, content);
  }

  function motionSheet() {
    let content = '<rect width="1800" height="640" fill="{{PAPER}}"/>';
    content += label(30, 48, 'D3 / JOINT CHECK / typing + tail + breathing', 30);
    [-15, -7.5, 0, -7.5, -15].forEach((angle, index) => {
      const x = index * 355 + 10;
      content += `<g transform="translate(${x} 100) scale(.72)">${kitten('idle', { paw: angle, tail: -10 + index * 5, breath: index === 2 ? 1.02 : 1 })}</g>`;
      content += label(x + 25, 470, `paw ${angle} / tail ${-10 + index * 5}`, 20);
    });
    content += label(30, 580, 'Shoulder pivots stay inside the body. Runtime tween and scene integration follow in E2 / D11.');
    return frame(1800, 640, content);
  }

  const empty = classroomOutputs(root)['previews/d2'].find(asset => asset.name === 'classroom_empty').markup;
  function scene(poseName) {
    let content = body(empty);
    content += `<g transform="translate(976 354) scale(.4)">${body(readFileSync(join(root, 'src/d1/teacher_watching.svg'), 'utf8'))}</g>`;
    content += '<path d="M1090 780 L1440 790 L1470 1020 L1050 1010Z" fill="{{PAPER}}" stroke="{{INK}}" stroke-width="6"/>';
    content += `<g transform="translate(${rig.sceneOrigin.join(' ')})">${kitten(poseName)}</g>`;
    return frame(1920, 1080, content);
  }

  for (const asset of rig.assets) {
    const [, width, height] = sources[asset.name].match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (+width !== asset.size[0] || +height !== asset.size[1]) throw new Error(`D3 size mismatch: ${asset.name}`);
    if (asset.pivotPixels.some((value, index) => value < 0 || value > asset.size[index])) throw new Error(`D3 invalid pivot: ${asset.name}`);
    if (/<text\b/.test(sources[asset.name])) throw new Error(`D3 sprite contains text: ${asset.name}`);
  }
  if (new Set(rig.assets.map(asset => asset.name)).size !== 12) throw new Error('D3 requires 12 unique sprites');
  return {
    'exports/d3': rig.assets.map(asset => ({ name: asset.name, markup: sources[asset.name], scale: rig.textureScale })),
    'previews/d3': [
      { name: 'pose_sheet', markup: poseSheet(), scale: 1 },
      { name: 'layer_sheet', markup: layerSheet(), scale: 1 },
      { name: 'motion_sheet', markup: motionSheet(), scale: 1 },
      ...['idle', 'panic_left', 'panic_right'].map(name => ({ name: `classroom_${name}`, markup: scene(name), scale: 1 })),
      { name: 'classroom_thumbnail', markup: scene('panic_left'), scale: .25 }
    ]
  };
}
