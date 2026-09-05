import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { classroomOutputs } from './classroom.mjs';

export function teacherOutputs(root) {
  const rig = JSON.parse(readFileSync(join(root, 'src/d4/rig.json'), 'utf8'));
  const sources = Object.fromEntries(rig.assets.map(asset => [asset.name,
    readFileSync(join(root, `src/d4/${asset.name}.svg`), 'utf8')]));
  let instance = 0;
  const frame = (width, height, content) => `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">${content}</svg>`;
  const label = (x, y, text, size = 22) => `<text x="${x}" y="${y}" font-family="Nunito" font-weight="700" font-size="${size}" fill="{{INK}}">${text}</text>`;
  function body(markup) {
    const prefix = `d4-${instance++}-`;
    return markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  }
  function sprite(name, position, rotation = 0, scale = [1, 1]) {
    const asset = rig.assets.find(item => item.name === name);
    if (!asset) throw new Error(`D4 missing asset: ${name}`);
    const [px, py] = asset.pivotPixels;
    return `<g transform="translate(${position.join(' ')}) rotate(${rotation}) scale(${scale.join(' ')}) translate(${-px} ${-py})">${body(sources[name])}</g>`;
  }
  function teacher(poseName, { arm = 0, gaze = 0 } = {}) {
    const pose = rig.poses[poseName];
    const profile = rig.profiles[pose.profile];
    const layers = { body: sprite(profile.body, rig.placements.body) };
    if (profile.pointer) {
      const pointer = sprite('teacher_pointer', rig.placements.pointer);
      layers.pointer = `<g transform="translate(${profile.pointerOffsetX ?? profile.faceOffsetX} 0) scale(${profile.pointerScaleX ?? profile.faceScaleX} 1)">${pointer}</g>`;
    }
    if (profile.chalk) layers.armChalk = sprite('teacher_arm_chalk', rig.placements.armChalk, pose.armRotation ?? arm);
    layers.head = sprite(profile.head, rig.placements.head);
    profile.eyes.forEach((position, index) => {
      const suffix = index === 0 ? 'Left' : 'Right';
      layers[`eye${suffix}`] = sprite('teacher_eye_white', position, 0, [profile.faceScaleX, profile.eyeScaleY || 1]);
      layers[`pupil${suffix}`] = sprite('teacher_pupil', [position[0] + (gaze + profile.pupilOffset[0]) * profile.faceScaleX, position[1] + profile.pupilOffset[1]], 0, [profile.faceScaleX, profile.eyeScaleY || 1]);
    });
    if (profile.glasses) {
      layers.glasses = `<g transform="translate(${profile.faceOffsetX} 0) scale(${profile.faceScaleX} 1)">${sprite('teacher_glasses', rig.placements.glasses)}</g>`;
    }
    const [px, py] = rig.rootPivot;
    return `<g transform="translate(${px} ${py}) rotate(${pose.rotation}) scale(${pose.scale}) translate(${-px} ${-py})">${rig.drawOrder.map(name => layers[name] || '').join('')}</g>`;
  }
  function poseSheet() {
    let content = '<rect width="1800" height="850" fill="{{PAPER}}"/>';
    content += label(35, 50, 'COPYCAT / D4 / MRS. HISSKINS', 34);
    content += label(35, 90, 'Back / half turn / watching / narrow angry eyes / shared glasses and moving pupils');
    Object.keys(rig.poses).forEach((name, index) => {
      const x = 15 + index * 298;
      content += `<rect x="${x}" y="125" width="280" height="650" rx="20" fill="{{WALL_LIGHT}}"/>`;
      content += `<g transform="translate(${x + 5} 175) scale(.7)">${teacher(name, { gaze: name === 'turning' ? 6 : 0 })}</g>`;
      content += label(x + 18, 690, name.toUpperCase(), 23);
    });
    content += label(35, 820, '12 sprites / PNG @2x / rig.json / Unity import D11 / animation E1');
    return frame(1800, 850, content);
  }
  function layerSheet() {
    let content = '<rect width="1600" height="1490" fill="{{PAPER}}"/>' + label(30, 50, 'D4 / SEPARATE SPRITES AND PIVOTS', 32);
    rig.assets.forEach((asset, index) => {
      const x = 25 + index % 4 * 395;
      const y = 85 + Math.floor(index / 4) * 460;
      const scale = Math.min(350 / asset.size[0], 350 / asset.size[1], 2);
      const left = x + (375 - asset.size[0] * scale) / 2;
      content += `<rect x="${x}" y="${y}" width="375" height="440" rx="14" fill="{{WALL_LIGHT}}"/>`;
      content += `<g transform="translate(${left} ${y + 10}) scale(${scale})">${body(sources[asset.name])}<circle cx="${asset.pivotPixels[0]}" cy="${asset.pivotPixels[1]}" r="5" fill="{{DANGER}}"/></g>`;
      content += label(x + 10, y + 386, asset.name, 18);
      content += label(x + 10, y + 418, `${asset.size.join(' x ')} / pivot ${asset.pivotPixels.join(', ')}`, 17);
    });
    return frame(1600, 1490, content);
  }
  function motionSheet() {
    let content = '<rect width="1800" height="1100" fill="{{PAPER}}"/>' + label(30, 48, 'D4 / CHALK SHOULDER AND GAZE CHECK', 30);
    [-8, -4, 0, 4, 8].forEach((arm, index) => {
      const x = index * 355 + 35;
      content += `<g transform="translate(${x} 100) scale(.65)">${teacher('writing', { arm })}</g>`;
      content += label(x, 530, `Writing / arm ${arm} degrees`, 20);
      const gaze = -6 + index * 3;
      content += `<g transform="translate(${x} 585) scale(.65)">${teacher(index === 4 ? 'staring' : 'watching', { gaze })}</g>`;
      content += label(x, 1050, `${index === 4 ? 'Staring' : 'Watching'} / gaze ${gaze} px`, 20);
    });
    return frame(1800, 1100, content);
  }
  const classroom = classroomOutputs(root)['previews/d2'].find(asset => asset.name === 'classroom_empty').markup;
  function scene(poseName) {
    let content = body(classroom);
    const figure = `<g transform="translate(${rig.sceneOrigins[poseName].join(' ')}) scale(${rig.sceneScale})">${teacher(poseName)}</g>`;
    if (poseName === 'writing' || poseName === 'turning') {
      const layout = JSON.parse(readFileSync(join(root, 'src/d2/layout.json'), 'utf8'));
      const desk = layout.layers.find(layer => layer.name === 'teacher_desk');
      const marker = `<g transform="translate(${desk.x} ${desk.y})`;
      if (!content.includes(marker)) throw new Error('D4 teacher desk insertion point missing');
      content = content.replace(marker, figure + marker);
    } else content += figure;
    content += `<g transform="translate(653 628)">${body(readFileSync(join(root, 'src/d1/kitten_back.svg'), 'utf8'))}</g>`;
    return frame(1920, 1080, content);
  }
  for (const asset of rig.assets) {
    const [, width, height] = sources[asset.name].match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (+width !== asset.size[0] || +height !== asset.size[1]) throw new Error(`D4 size mismatch: ${asset.name}`);
    if (asset.pivotPixels.some((value, index) => value < 0 || value > asset.size[index])) throw new Error(`D4 invalid pivot: ${asset.name}`);
    if (/<text\b/.test(sources[asset.name])) throw new Error(`D4 sprite contains text: ${asset.name}`);
  }
  if (new Set(rig.assets.map(asset => asset.name)).size !== 12) throw new Error('D4 requires 12 unique sprites');
  return {
    'exports/d4': rig.assets.map(asset => ({ name: asset.name, markup: sources[asset.name], scale: rig.textureScale })),
    'previews/d4': [
      { name: 'pose_sheet', markup: poseSheet(), scale: 1 },
      { name: 'layer_sheet', markup: layerSheet(), scale: 1 },
      { name: 'motion_sheet', markup: motionSheet(), scale: 1 },
      ...['writing', 'turning', 'watching', 'staring'].map(name => ({ name: `classroom_${name}`, markup: scene(name), scale: 1 })),
      { name: 'classroom_thumbnail', markup: scene('staring'), scale: .25 }
    ]
  };
}
