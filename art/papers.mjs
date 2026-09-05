import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { neighbourComposer } from './neighbours.mjs';

export function paperOutputs(root) {
  const layout = JSON.parse(readFileSync(join(root, 'src/d6/layout.json'), 'utf8'));
  const exam = JSON.parse(readFileSync(join(root, 'src/d6/exam_samples.json'), 'utf8'));
  const read = name => readFileSync(join(root, `src/d6/${name}.svg`), 'utf8');
  const glyphTemplate = read('glyph_arrow');
  const sources = Object.fromEntries(layout.assets.map(asset => [asset.name, read(asset.name)]));
  const glyphs = [];
  for (const [direction, angle] of Object.entries(layout.glyphDirections)) {
    for (const [state, colors] of Object.entries(layout.glyphStates)) {
      const name = `glyph_arrow_${direction}_${state}`;
      sources[name] = glyphTemplate
        .replace('{{GLYPH_ANGLE}}', String(angle))
        .replace('{{GLYPH_FILL}}', `{{${colors.fill}}}`)
        .replace('{{GLYPH_INK}}', `{{${colors.ink}}}`);
      glyphs.push({ name, ...layout.glyphAsset });
    }
  }
  const assets = [...layout.assets.slice(0, 2), ...glyphs, ...layout.assets.slice(2)];
  const neighbours = neighbourComposer(root);
  const { frame, label, deskTransform } = neighbours;
  let instance = 0;
  const player = layout.playerPaper;
  const sheet = layout.neighbourPaper;
  const paperFont = layout.fonts.paper;
  const ink = `{{${layout.inkColor}}}`;
  const wrong = `{{${layout.wrongColor}}}`;

  function body(markup) {
    const prefix = `d6-${instance++}-`;
    return markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  }
  function sprite(name, x, y, scale = 1, rotation = 0) {
    const asset = assets.find(item => item.name === name);
    const [px, py] = asset.pivotPixels;
    return `<g transform="translate(${x} ${y}) rotate(${rotation}) scale(${scale}) translate(${-px} ${-py})">${body(sources[name])}</g>`;
  }
  function hand(x, y, value, size, extra = '', color = ink) {
    return `<text x="${x}" y="${y}" font-family="${paperFont}" font-size="${size}" fill="${color}" ${extra}>${value}</text>`;
  }
  function strokeState(index, progress, wrongIndex) {
    if (index === wrongIndex) return 'wrong';
    return index < progress ? 'done' : 'normal';
  }
  function playerPaper(question, progress, options = {}) {
    const text = player.text;
    let content = body(sources.paper_player);
    content += hand(text.header.x, text.header.baseline, text.header.value, text.header.fontSize);
    content += hand(text.student.x, text.student.baseline, text.student.value.replace('{name}', options.name || 'Whiskerless'), text.student.fontSize);
    question.lines.forEach((line, index) => { content += hand(text.question.x, text.question.baselines[index], line, text.question.fontSize); });
    content += hand(text.answerLabel.x, text.answerLabel.baseline, text.answerLabel.value, text.answerLabel.fontSize);
    const row = player.answerRow;
    if (progress === 0 && !options.copied) content += hand(row.origin[0] + 6, row.letterBaseline, row.blank, text.answerLabel.fontSize);
    if (question.type === 'strokes') {
      question.strokes.slice(0, progress).forEach((direction, index) => {
        content += sprite(`glyph_arrow_${direction}_done`, row.origin[0] + 30 + index * row.advance, row.origin[1] + 30, row.glyphScale);
      });
    } else if (question.type === 'word') {
      [...question.word.slice(0, progress)].forEach((letter, index) => {
        const color = options.wrongLetter === index ? wrong : ink;
        content += hand(row.origin[0] + 8 + index * row.letterAdvance, row.letterBaseline, letter, row.letterSize, '', color);
      });
    } else if (question.type === 'pick' && progress > 0) {
      content += hand(row.origin[0] + 8, row.letterBaseline, question.options[question.correct], row.letterSize);
    }
    if (options.copied) content += sprite('stamp_copied', player.stamp.center[0], player.stamp.center[1], player.stamp.scale, player.stamp.rotation);
    return content;
  }
  function neighbourPaper(question, progress, options = {}) {
    let content = body(sources.paper_neighbour);
    content += hand(sheet.name.x, sheet.name.baseline, question.neighbour, sheet.name.fontSize);
    content += sprite('scribble_2', 300, 108, .5);
    content += sprite('scribble_4', 74, 162, .6);
    if (question.type === 'strokes') {
      const total = question.strokes.length;
      const start = 240 - (total - 1) * sheet.strokes.advance / 2;
      question.strokes.forEach((direction, index) => {
        content += sprite(`glyph_arrow_${direction}_${strokeState(index, progress, options.wrongIndex)}`, start + index * sheet.strokes.advance, sheet.strokes.centerY, sheet.strokes.glyphScale);
      });
    } else if (question.type === 'pick') {
      question.options.forEach((option, index) => {
        const [x, y] = sheet.pick.cells[index];
        content += hand(x, y + 10, `${'ABCD'[index]}  ${option}`, sheet.pick.fontSize, 'text-anchor="middle"');
      });
      if (!options.hideCircle) {
        const [x, y] = sheet.pick.cells[question.correct];
        content += `<g opacity="${options.faint ? .35 : 1}">${sprite('glyph_pick_circle', x, y, sheet.pick.circleScale)}</g>`;
      }
    } else if (question.type === 'word') {
      const letters = [...question.word];
      const advance = sheet.word.fontSize * .62 + sheet.word.letterSpacing;
      const start = sheet.word.centerX - (letters.length - 1) * advance / 2;
      letters.forEach((letter, index) => {
        content += hand(start + index * advance, sheet.word.baseline, letter, sheet.word.fontSize, 'text-anchor="middle"', index < progress ? '{{OK}}' : ink);
      });
    }
    return content;
  }
  function placedNeighbourPaper(side, question, progress, options) {
    const placement = sheet.placements[side];
    const desk = neighbours.layout.layers.find(layer => layer.name === placement.desk);
    return deskTransform(desk, `<g transform="translate(${placement.x} ${placement.y}) scale(${placement.scale})">${neighbourPaper(question, progress, options)}</g>`);
  }
  function placedPlayerPaper(question, progress, options) {
    const { x, y, rotation, scale } = player.placement;
    return `<g transform="translate(${x} ${y}) rotate(${rotation}) scale(${scale})">${playerPaper(question, progress, options)}</g>`;
  }
  function ringTimer(side, remaining) {
    const placement = layout.ringTimer.placements[side];
    const angle = remaining * 360;
    const large = angle > 180 ? 1 : 0;
    const end = [80 + 90 * Math.sin(angle * Math.PI / 180), 80 - 90 * Math.cos(angle * Math.PI / 180)];
    const wedgeId = `d6-wedge-${instance++}`;
    const wedge = remaining >= 1 ? '' : `clip-path="url(#${wedgeId})"`;
    const clip = remaining >= 1 ? '' : `<defs><clipPath id="${wedgeId}"><path d="M80 80 L80 -10 A90 90 0 ${large} 1 ${end[0].toFixed(2)} ${end[1].toFixed(2)}Z"/></clipPath></defs>`;
    return `<g transform="translate(${placement.x} ${placement.y}) scale(${placement.scale}) translate(-80 -80)">${clip}${body(sources.ring_timer_track)}<g ${wedge}>${body(sources.ring_timer)}</g></g>`;
  }
  function otherSide(side) { return side === 'left' ? 'right' : 'left'; }
  function scene(question, progress, options = {}) {
    const side = question.side;
    const idle = exam.idle[otherSide(side)];
    const lifted = options.lift === undefined ? 1 : options.lift;
    const lift = { nerd: side === 'left' ? lifted : 0, fluffy: side === 'right' ? lifted : 0 };
    return neighbours.scene(lift, {
      papers: () => placedNeighbourPaper(side, question, progress, options) + placedNeighbourPaper(otherSide(side), idle, 0, { hideCircle: true }),
      beforeKitten: () => placedPlayerPaper(question, options.copied ? progress : progress, options),
      afterKitten: () => ringTimer(side, options.remaining === undefined ? .6 : options.remaining)
    });
  }
  function stateSheet() {
    let content = '<rect width="1920" height="1400" fill="{{WALL}}"/>' + label(35, 52, 'COPYCAT / D6 / PAPERS AND INPUT', 34);
    content += label(35, 92, 'Own paper: header, student, question, answer row, COPIED stamp. Neighbour paper: strokes / pick / word. Text is TMP in Unity.');
    const q1 = exam.questions.find(question => question.id === 1);
    const q9 = exam.questions.find(question => question.id === 9);
    const q4 = exam.questions.find(question => question.id === 4);
    const q11 = exam.questions.find(question => question.id === 11);
    const cell = (x, y, caption, markup, scale = .68) => `<rect x="${x}" y="${y}" width="470" height="380" rx="20" fill="{{WALL_LIGHT}}"/>` + label(x + 16, y + 34, caption, 20) + `<g transform="translate(${x + 14} ${y + 52}) scale(${scale})">${markup}</g>`;
    content += cell(35, 120, 'Q1 own paper / empty answer', playerPaper(q1, 0));
    content += cell(525, 120, 'Q1 own paper / 1 of 2 strokes', playerPaper(q1, 1));
    content += cell(1015, 120, 'Q1 own paper / COPIED', playerPaper(q1, 2, { copied: true }));
    content += cell(1505, 120, 'Q11 own paper / CATN + wrong I', playerPaper(q11, 5, { wrongLetter: 4 }), .68);
    content += cell(35, 520, 'Q9 Whiskerstein / strokes 2 done', neighbourPaper(q9, 2), .9);
    content += cell(525, 520, 'Q9 Whiskerstein / wrong at 3rd', neighbourPaper(q9, 2, { wrongIndex: 2 }), .9);
    content += cell(1015, 520, 'Q4 Fluffy / pick circle', neighbourPaper(q4, 0), .9);
    content += cell(1505, 520, 'Q4 Fluffy / faint circle (phase 3+)', neighbourPaper(q4, 0, { faint: true }), .9);
    content += cell(35, 920, 'Q11 Whiskerstein / word 3 typed', neighbourPaper(q11, 3), .9);
    let rings = '';
    [1, .75, .5, .2].forEach((remaining, index) => {
      rings += `<g transform="translate(${60 + index * 110} 90)">${ringTimer('left', remaining).replace(/translate\([^)]*\) scale\([^)]*\) translate\(-80 -80\)/, 'translate(-40 -40) scale(.5)')}</g>`;
      rings += label(38 + index * 110, 165, `${Math.round(remaining * 100)}%`, 18);
    });
    content += cell(525, 920, 'ring_timer over ring_timer_track / radial fill', rings + `<g transform="translate(20 200)">${sprite('scribble_1', 0, 20)}${sprite('scribble_2', 0, 60)}${sprite('scribble_3', 240, 20)}${sprite('scribble_4', 240, 60)}</g>` + label(20, 300, 'scribble_1..4', 20), 1);
    let glyphRow = '';
    Object.keys(layout.glyphDirections).forEach((direction, column) => {
      Object.keys(layout.glyphStates).forEach((state, row) => {
        glyphRow += sprite(`glyph_arrow_${direction}_${state}`, 100 + column * 105, 60 + row * 100);
      });
      glyphRow += label(80 + column * 105, 335, direction, 20);
    });
    ['normal', 'done', 'wrong'].forEach((state, row) => { glyphRow += label(0, 66 + row * 100, state, 18); });
    content += cell(1015, 920, '12 arrow glyphs / normal, done, wrong', glyphRow, 1);
    content += cell(1505, 920, 'stamp_copied + glyph_pick_circle', `${sprite('stamp_copied', 230, 90, 1.2, -8)}${sprite('glyph_pick_circle', 230, 240, 1.4)}` + hand(230, 250, 'B  Betrayal', 40, 'text-anchor="middle"'), 1);
    content += label(35, 1370, 'Glyphs 84x84, papers 640x460 / 480x360, stamp 300x120, ring 160x160, scribbles 240x40 - all PNG @2x, PPU 200, no trim. Import D11, gameplay binding A5/B3.', 21);
    return frame(1920, 1400, content);
  }
  function layerSheet() {
    let content = '<rect width="1920" height="1000" fill="{{PAPER}}"/>' + label(30, 52, 'D6 / SPRITES AND PIVOTS', 32);
    assets.forEach((asset, index) => {
      const column = index % 8;
      const row = Math.floor(index / 8);
      const x = 25 + column * 236;
      const y = 85 + row * 300;
      const scale = Math.min(200 / asset.size[0], 180 / asset.size[1], 2);
      content += `<rect x="${x}" y="${y}" width="220" height="280" rx="15" fill="{{WALL_LIGHT}}"/>`;
      content += `<g transform="translate(${x + (220 - asset.size[0] * scale) / 2} ${y + 20 + (180 - asset.size[1] * scale) / 2}) scale(${scale})">${body(sources[asset.name])}<circle cx="${asset.pivotPixels[0]}" cy="${asset.pivotPixels[1]}" r="${5 / scale}" fill="{{DANGER}}"/></g>`;
      content += label(x + 10, y + 236, asset.name, 15) + label(x + 10, y + 262, `${asset.size.join(' x ')} / pivot ${asset.pivotPixels.join(', ')}`, 13);
    });
    return frame(1920, 1000, content);
  }
  for (const asset of assets) {
    const [, width, height] = sources[asset.name].match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (+width !== asset.size[0] || +height !== asset.size[1]) throw new Error(`D6 size mismatch: ${asset.name}`);
    if (/\{\{GLYPH_/.test(sources[asset.name])) throw new Error(`D6 unresolved glyph token: ${asset.name}`);
    if (asset.name !== 'stamp_copied' && /<text\b/.test(sources[asset.name])) throw new Error(`D6 sprite contains text: ${asset.name}`);
  }
  if (new Set(assets.map(asset => asset.name)).size !== 22) throw new Error('D6 requires 22 unique sprites');
  const q1 = exam.questions.find(question => question.id === 1);
  const q6 = exam.questions.find(question => question.id === 6);
  const q8 = exam.questions.find(question => question.id === 8);
  const q9 = exam.questions.find(question => question.id === 9);
  return {
    'exports/d6': assets.map(asset => ({ name: asset.name, markup: sources[asset.name], scale: layout.textureScale })),
    'previews/d6': [
      { name: 'state_sheet', markup: stateSheet(), scale: 1 },
      { name: 'layer_sheet', markup: layerSheet(), scale: 1 },
      { name: 'classroom_strokes', markup: scene(q1, 1, { remaining: .6 }), scale: 1 },
      { name: 'classroom_word', markup: scene(q6, 2, { remaining: .3 }), scale: 1 },
      { name: 'classroom_pick', markup: scene(q8, 0, { remaining: .9 }), scale: 1 },
      { name: 'classroom_copied', markup: scene(q1, 2, { copied: true, remaining: 0 }), scale: 1 },
      { name: 'classroom_covered', markup: scene(q9, 0, { remaining: 1, lift: 0 }), scale: 1 },
      { name: 'classroom_thumbnail', markup: scene(q1, 1, { remaining: .6 }), scale: .25 }
    ]
  };
}
