import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { classroomOutputs } from './classroom.mjs';

export function neighbourComposer(root) {
  const rig = JSON.parse(readFileSync(join(root, 'src/d5/rig.json'), 'utf8'));
  const layout = JSON.parse(readFileSync(join(root, 'src/d2/layout.json'), 'utf8'));
  const sources = Object.fromEntries(rig.assets.map(asset => [asset.name,
    readFileSync(join(root, `src/d5/${asset.name}.svg`), 'utf8')]));
  let instance = 0;
  const frame = (width, height, content) => `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">${content}</svg>`;
  const label = (x, y, value, size = 22) => `<text x="${x}" y="${y}" font-family="Nunito" font-weight="700" font-size="${size}" fill="{{INK}}">${value}</text>`;
  function body(markup) {
    const prefix = `d5-${instance++}-`;
    return markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  }
  function sprite(name, position, rotation = 0, scale = [1, 1]) {
    const asset = rig.assets.find(item => item.name === name);
    const [px, py] = asset.pivotPixels;
    return `<g transform="translate(${position.join(' ')}) rotate(${rotation}) scale(${scale.join(' ')}) translate(${-px} ${-py})">${body(sources[name])}</g>`;
  }
  function character(name, progress, parts) {
    const profile = rig.characters[name];
    const pose = rig.poses.lifted;
    const layers = {
      body: sprite(`${name}_body`, rig.placements.body),
      paw: sprite(`${name}_paw_cover`, [rig.placements.paw[0], rig.placements.paw[1] + pose.offsetY * progress], pose.rotation * progress),
      head: sprite(`${name}_head`, rig.placements.head),
      eyes: profile.eyes.map(point => sprite('eye_white', point, 0, profile.eyeScale)).join(''),
      pupils: profile.eyes.map(point => sprite('pupil', point.map((value, index) => value + profile.pupilOffset[index]), 0, [1, 1])).join('')
    };
    return parts.map(part => layers[part] || '').join('');
  }
  function inScene(name, progress, parts) {
    const content = `<g transform="translate(${rig.sceneOrigin.join(' ')}) scale(${rig.sceneScale})">${character(name, progress, parts)}</g>`;
    return rig.characters[name].mirror ? `<g transform="translate(1920 0) scale(-1 1)">${content}</g>` : content;
  }
  const empty = classroomOutputs(root)['previews/d2'].find(asset => asset.name === 'classroom_empty').markup;
  function deskTransform(layer, content) {
    const [px, py] = layer.pivotPixels;
    return `<g transform="translate(${layer.x} ${layer.y}) translate(${px} ${py}) rotate(${layer.rotation}) translate(${-px} ${-py})">${content}</g>`;
  }
  function papers() {
    return layout.neighbourPaperSlots.map(slot => {
      const desk = layout.layers.find(layer => layer.name === slot.desk);
      const [x, y, width, height] = slot.bounds;
      return deskTransform(desk, `<g transform="translate(${x} ${y})"><path d="M12 5 L${width - 25} 0 L${width} ${height - 8} L0 ${height}Z" fill="{{PAPER}}" stroke="{{INK}}" stroke-width="6"/>
        <path d="M22 25 L130 24 M22 40 L115 39" fill="none" stroke="{{PENCIL_INK}}" stroke-width="3"/>
        <path d="M143 97 V67 M129 81 L143 67 L157 81 M181 81 H211 M197 67 L211 81 L197 95" fill="none" stroke="{{PENCIL_INK}}" stroke-width="7" stroke-linejoin="round" stroke-linecap="round"/></g>`);
    }).join('');
  }
  function scene(progress, options = {}) {
    const paperMarkup = options.papers || papers;
    const lift = name => typeof progress === 'object' ? progress[name] : progress;
    let content = body(empty);
    const teacherDesk = layout.layers.find(layer => layer.name === 'teacher_desk');
    const teacherMarker = `<g transform="translate(${teacherDesk.x} ${teacherDesk.y})`;
    content = content.replace(teacherMarker, `<g transform="translate(976 235) scale(.4)">${body(readFileSync(join(root, 'src/d1/teacher_writing.svg'), 'utf8'))}</g>` + teacherMarker);
    const leftDesk = layout.layers.find(layer => layer.name === 'desk_neighbour_left');
    const marker = `<g transform="translate(${leftDesk.x} ${leftDesk.y})`;
    const bodies = Object.keys(rig.characters).map(name => inScene(name, lift(name), ['body'])).join('');
    if (!content.includes(marker)) throw new Error('D5 neighbour desk insertion point missing');
    content = content.replace(marker, bodies + marker);
    const playerDesk = layout.layers.find(layer => layer.name === 'desk_player');
    const front = `<g transform="translate(${playerDesk.x} ${playerDesk.y})`;
    const upper = paperMarkup() + Object.keys(rig.characters).map(name => inScene(name, lift(name), ['paw', 'head', 'eyes', 'pupils'])).join('');
    if (!content.includes(front)) throw new Error('D5 player desk insertion point missing');
    content = content.replace(front, upper + front);
    content += options.beforeKitten ? options.beforeKitten() : '';
    content += `<g transform="translate(653 628)">${body(readFileSync(join(root, 'src/d1/kitten_back.svg'), 'utf8'))}</g>`;
    content += options.afterKitten ? options.afterKitten() : '';
    return frame(1920, 1080, content);
  }
  return { rig, layout, sources, frame, label, body, sprite, character, inScene, deskTransform, papers, scene };
}

export function neighbourOutputs(root) {
  const { rig, sources, frame, label, body, character, scene } = neighbourComposer(root);
  function poseSheet() {
    let content = '<rect width="1800" height="1060" fill="{{PAPER}}"/>' + label(35, 52, 'COPYCAT / D5 / WHISKERSTEIN + FLUFFY', 34);
    content += label(35, 92, 'Separate body / head / cover paw / shared eyes and pupils');
    Object.entries(rig.characters).forEach(([name, profile], row) => {
      [0, 1].forEach((progress, column) => {
        const x = 30 + (row * 2 + column) * 440;
        content += `<rect x="${x}" y="128" width="420" height="815" rx="20" fill="{{WALL_LIGHT}}"/>`;
        const figure = character(name, progress, ['body', 'paw', 'head', 'eyes', 'pupils']);
        content += `<g transform="translate(${x + 22} 175) scale(.69)">${profile.mirror ? `<g transform="translate(540 0) scale(-1 1)">${figure}</g>` : figure}</g>`;
        content += label(x + 20, 835, profile.displayName, 28) + label(x + 20, 884, progress ? 'LIFTED' : 'COVERED');
      });
    });
    content += label(35, 1008, '8 sprites / shoulder pivots / PNG @2x / Unity import D11 / runtime E3');
    return frame(1800, 1060, content);
  }
  function layerSheet() {
    let content = '<rect width="1600" height="1140" fill="{{PAPER}}"/>' + label(30, 52, 'D5 / SPRITES AND SHOULDER PIVOTS', 32);
    rig.assets.forEach((asset, index) => {
      const x = 25 + index % 4 * 395;
      const y = 85 + Math.floor(index / 4) * 520;
      const scale = Math.min(350 / asset.size[0], 390 / asset.size[1], 2);
      content += `<rect x="${x}" y="${y}" width="375" height="490" rx="15" fill="{{WALL_LIGHT}}"/>`;
      content += `<g transform="translate(${x + (375 - asset.size[0] * scale) / 2} ${y + 10}) scale(${scale})">${body(sources[asset.name])}<circle cx="${asset.pivotPixels[0]}" cy="${asset.pivotPixels[1]}" r="6" fill="{{DANGER}}"/></g>`;
      content += label(x + 12, y + 437, asset.name, 21) + label(x + 12, y + 471, `${asset.size.join(' x ')} / pivot ${asset.pivotPixels.join(', ')}`, 17);
    });
    return frame(1600, 1140, content);
  }
  function motionSheet() {
    let content = '<rect width="1800" height="1210" fill="{{PAPER}}"/>' + label(30, 48, 'D5 / B-002 / SHOULDER + DESK + OUTBACK OVERSHOOT', 29);
    [0, .25, .5, 1, 1.1].forEach((progress, index) => {
      const x = 15 + index * 356;
      const markup = body(scene(progress));
      content += `<svg x="${x}" y="90" width="340" height="460" viewBox="160 300 570 470">${markup}</svg>`;
      content += `<svg x="${x}" y="635" width="340" height="460" viewBox="1190 300 570 470">${body(scene(progress))}</svg>`;
      content += label(x + 8, 580, `Whiskerstein / ${Math.round(progress * 100)}%`, 19);
      content += label(x + 8, 1125, `Fluffy / ${Math.round(progress * 100)}%`, 19);
    });
    content += label(30, 1183, 'Closed / intermediate / raised / 10% overshoot. Answer arrows are D6 placeholders.', 21);
    return frame(1800, 1210, content);
  }
  for (const asset of rig.assets) {
    const [, width, height] = sources[asset.name].match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (+width !== asset.size[0] || +height !== asset.size[1]) throw new Error(`D5 size mismatch: ${asset.name}`);
    if (asset.pivotPixels.some((value, index) => value < 0 || value > asset.size[index])) throw new Error(`D5 invalid pivot: ${asset.name}`);
    if (/<text\b/.test(sources[asset.name])) throw new Error(`D5 sprite contains text: ${asset.name}`);
  }
  if (new Set(rig.assets.map(asset => asset.name)).size !== 8) throw new Error('D5 requires 8 unique sprites');
  return {
    'exports/d5': rig.assets.map(asset => ({name: asset.name, markup: sources[asset.name], scale: rig.textureScale})),
    'previews/d5': [
      {name: 'pose_sheet', markup: poseSheet(), scale: 1},
      {name: 'layer_sheet', markup: layerSheet(), scale: 1},
      {name: 'motion_sheet', markup: motionSheet(), scale: 1},
      ...Object.keys(rig.poses).map(name => ({name: `classroom_${name}`, markup: scene(name === 'lifted' ? 1 : 0), scale: 1})),
      {name: 'classroom_thumbnail', markup: scene(1), scale: .25}
    ]
  };
}
