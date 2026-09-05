import { readFileSync } from 'node:fs';
import { join } from 'node:path';

export function classroomOutputs(root) {
  const layout = JSON.parse(readFileSync(join(root, 'src/d2/layout.json'), 'utf8'));
  const sources = Object.fromEntries(layout.layers.map(layer => [layer.name,
    readFileSync(join(root, `src/d2/${layer.name}.svg`), 'utf8')]));
  let instance = 0;

  function frame(width, height, content) {
    return `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">${content}</svg>`;
  }

  function body(markup) {
    const prefix = `d2-${instance++}-`;
    const [, width, height] = markup.match(/viewBox="0 0 (\d+) (\d+)"/);
    const content = markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
    return `<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" overflow="hidden">${content}</svg>`;
  }

  function transform(layer, content) {
    const [pivotX, pivotY] = layer.pivotPixels;
    return `<g transform="translate(${layer.x} ${layer.y}) translate(${pivotX} ${pivotY}) rotate(${layer.rotation}) scale(${layer.scale}) translate(${-pivotX} ${-pivotY})">${content}</g>`;
  }

  function label(x, y, value, size = 24, color = '{{INK}}', extra = '') {
    return `<text x="${x}" y="${y}" font-family="Nunito" font-weight="700" font-size="${size}" fill="${color}" ${extra}>${value}</text>`;
  }

  function boardText() {
    const [x, y] = layout.boardText.origin;
    return `<g transform="translate(${x} ${y})" fill="{{${layout.boardText.color}}}" text-anchor="middle">`
      + layout.boardText.lines.map(line => `<text x="${line.x}" y="${line.baseline}" font-family="${line.font}" font-size="${line.fontSize}">${line.text}</text>`).join('')
      + '<path d="M278 198 Q456 202 630 197" fill="none" stroke="{{PAPER}}" stroke-width="3" stroke-linecap="round"/></g>';
  }

  function pose(name, x, y, scale) {
    return `<g transform="translate(${x} ${y}) scale(${scale})">${body(readFileSync(join(root, `src/d1/${name}.svg`), 'utf8'))}</g>`;
  }

  function paper(width, height) {
    return `<path d="M12 5 L${width - 25} 0 L${width} ${height - 8} L0 ${height}Z" fill="{{PAPER}}" stroke="{{INK}}" stroke-width="6" stroke-linejoin="round"/>`;
  }

  function neighbourPapers() {
    return layout.neighbourPaperSlots.map(slot => {
      const desk = layout.layers.find(layer => layer.name === slot.desk);
      const [x, y, width, height] = slot.bounds;
      return transform(desk, `<g transform="translate(${x} ${y})">${paper(width, height)}
        <path d="M22 32 Q72 28 119 33 M22 47 L105 46" fill="none" stroke="{{PENCIL_INK}}" stroke-width="3"/>
        <path d="M60 100 V72 M46 86 L60 72 L74 86 M113 86 H144 M130 72 L144 86 L130 100" fill="none" stroke="{{PENCIL_INK}}" stroke-width="7" stroke-linejoin="round" stroke-linecap="round"/>
      </g>`);
    }).join('');
  }

  function scene(mode = 'empty') {
    const proof = mode === 'calm' || mode === 'risk';
    let content = '';
    for (const layer of layout.layers) {
      if (proof && layer.name === 'teacher_desk' && mode === 'calm') content += pose('teacher_writing', 976, 235, .4);
      content += transform(layer, body(sources[layer.name]));
      if (layer.name === 'blackboard') content += boardText();
      if (proof && layer.name === 'teacher_desk' && mode === 'risk') {
        content += '<ellipse cx="1052" cy="592" rx="90" ry="16" fill="{{INK}}" opacity=".12"/>';
        content += pose('teacher_watching', 976, 354, .4);
      }
      if (proof && layer.name === 'desk_neighbour_right') content += neighbourPapers();
    }
    if (proof) {
      content += `<g transform="translate(1040 770) rotate(4)">${paper(415, 244)}
        <g fill="{{PENCIL_INK}}" font-family="Patrick Hand">
          <text x="24" y="40" font-size="25">CAT ACADEMY — FINAL EXAM</text>
          <text x="24" y="76" font-size="24">Student: __________</text>
          <text x="24" y="119" font-size="26">How many lives does a cat</text>
          <text x="24" y="151" font-size="26">claim to have?</text>
          <text x="24" y="206" font-size="30">Answer: ____</text>
        </g></g>`;
      content += '<ellipse cx="1652" cy="900" rx="83" ry="18" fill="{{INK}}" opacity=".12"/>';
      content += pose('duck', 1575, 769, 1);
      content += mode === 'risk'
        ? `<g transform="rotate(-9 888 1058)">${pose('kitten_lean', 593, 628, 1)}</g>`
        : pose('kitten_back', 653, 628, 1);
    }
    if (mode === 'layout') {
      const [x, y] = layout.vanishingPoint;
      content += `<path d="M0 ${y} H1920 M${x} 0 V1080" stroke="{{DANGER}}" stroke-width="3" stroke-dasharray="12 10"/>`;
      content += `<circle cx="${x}" cy="${y}" r="10" fill="{{DANGER}}"/>`;
      content += label(x + 18, y + 28, `VP ${x}, ${y}`, 20, '{{INK}}');
      for (const slot of layout.slots) {
        const [left, top, width, height] = slot.bounds;
        content += `<rect x="${left}" y="${top}" width="${width}" height="${height}" rx="12" fill="{{PAPER}}" fill-opacity=".6" stroke="{{PENCIL_INK}}" stroke-width="3" stroke-dasharray="10 7"/>`;
        content += label(left + 8, top + 24, slot.name, 17, '{{PENCIL_INK}}');
      }
      content += neighbourPapers();
    }
    return frame(...layout.canvas, content);
  }

  function layerSheet() {
    let content = '<rect width="1600" height="1300" fill="{{PAPER}}"/>';
    content += label(36, 58, 'COPYCAT / D2 / CLASSROOM LAYERS', 36);
    content += label(36, 95, '9 separate sprites / PNG @2x / layout and pivots in src/d2/layout.json', 22);
    for (let index = 0; index < layout.layers.length; index++) {
      const layer = layout.layers[index];
      const x = 36 + index % 3 * 522;
      const y = 125 + Math.floor(index / 3) * 365;
      const [width, height] = layer.size;
      const scale = Math.min(480 / width, 246 / height, 2);
      content += `<rect x="${x}" y="${y}" width="490" height="330" rx="16" fill="{{WALL_LIGHT}}" stroke="{{INK}}" stroke-width="3"/>`;
      content += `<g transform="translate(${x + (490 - width * scale) / 2} ${y + 12 + (246 - height * scale) / 2}) scale(${scale})">${body(sources[layer.name])}</g>`;
      content += label(x + 15, y + 283, layer.name, 21);
      content += label(x + 15, y + 312, `${width} × ${height} @1x / ${width * 2} × ${height * 2} PNG`, 18);
    }
    content += label(36, 1260, 'Board text is separate. Clock face has no hands. Desks contain no characters, paper or duck.', 21);
    return frame(1600, 1300, content);
  }

  const names = new Set();
  for (const layer of layout.layers) {
    if (names.has(layer.name)) throw new Error(`D2 duplicate layer: ${layer.name}`);
    names.add(layer.name);
    const [, width, height] = sources[layer.name].match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (Number(width) !== layer.size[0] || Number(height) !== layer.size[1]) throw new Error(`D2 size mismatch: ${layer.name}`);
    if (/<text\b/.test(sources[layer.name])) throw new Error(`D2 text must remain separate: ${layer.name}`);
    if (layer.pivotPixels.some((value, index) => value < 0 || value > layer.size[index])) throw new Error(`D2 invalid pivot: ${layer.name}`);
  }
  const floor = layout.layers.find(layer => layer.name === 'classroom_floor');
  if (floor.y !== layout.vanishingPoint[1] || floor.y + floor.size[1] !== layout.canvas[1]) throw new Error('D2 floor must cover horizon to bottom');
  for (const hand of layout.layers.filter(layer => layer.name.startsWith('clock_hand_'))) {
    if (hand.x + hand.pivotPixels[0] !== 1676 || hand.y + hand.pivotPixels[1] !== 161) throw new Error('D2 clock hands must share the face center');
  }
  return {
    'exports/d2': layout.layers.map(layer => ({ name: layer.name, markup: sources[layer.name], scale: layout.textureScale })),
    'previews/d2': [
      { name: 'classroom_empty', markup: scene(), scale: 1 },
      { name: 'classroom_calm', markup: scene('calm'), scale: 1 },
      { name: 'classroom_risk', markup: scene('risk'), scale: 1 },
      { name: 'classroom_thumbnail', markup: scene('risk'), scale: .25 },
      { name: 'classroom_layout', markup: scene('layout'), scale: 1 },
      { name: 'layer_sheet', markup: layerSheet(), scale: 1 }
    ]
  };
}
