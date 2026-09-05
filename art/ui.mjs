import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { paperOutputs } from './papers.mjs';
import { duckOutputs } from './duck.mjs';

// D8 is an asset pack. Runtime strings, meter values and button actions belong to Unity.
export function uiOutputs(root) {
  const layout = JSON.parse(readFileSync(join(root, 'src/d8/layout.json'), 'utf8'));
  const read = (name, task = 'd8') => readFileSync(join(root, `src/${task}/${name}.svg`), 'utf8');
  const sources = {};
  for (const asset of layout.assets) {
    let markup;
    if (asset.template === 'stamp_grade') {
      markup = read('stamp_grade').replaceAll('{{GRADE_COLOR}}', `{{${asset.color}}}`)
        .replace('{{GRADE_LABEL}}', asset.label).replace('{{GRADE_SIZE}}', asset.fontSize);
    } else if (asset.template === '../d7/keycap') {
      markup = read('keycap', 'd7').replace('{{KEYCAP_LABEL}}', asset.label);
      if (asset.label === 'Space') {
        // Extend only the right corners and straight bands of the shared D7 template.
        // Left corners and stroke widths stay unchanged; the word gets its own optical size.
        markup = markup.replace('width="64"', 'width="160"').replace('viewBox="0 0 64 64"', 'viewBox="0 0 160 64"')
          .replace(/(?<!\d)(51|52|57|58)(?!\d)/g, x => String(Number(x) + 96))
          .replace('x="32"', 'x="80"').replace('font-size="34"', 'font-size="24"').replace('y="41"', 'y="37"');
      }
    } else markup = read(asset.name);
    const [, w, h] = markup.match(/viewBox="0 0 (\d+) (\d+)"/) || [];
    if (+w !== asset.size[0] || +h !== asset.size[1]) throw new Error(`D8 invalid size: ${asset.name}`);
    if (asset.borderPixels && asset.borderPixels.some((n, i) => n * asset.textureScale !== asset.unityBorderPixels[i])) throw new Error(`D8 invalid border: ${asset.name}`);
    if (!/^(keycap_|stamp_grade_|logo_copycat)/.test(asset.name) && /<text\b/.test(markup)) throw new Error(`D8 baked runtime text: ${asset.name}`);
    sources[asset.name] = markup;
  }
  if (new Set(layout.assets.map(asset => asset.name)).size !== 27) throw new Error('D8 requires 27 unique sprites');
  let instance = 0;
  const frame = (w, h, body) => `<svg xmlns="http://www.w3.org/2000/svg" width="${w}" height="${h}" viewBox="0 0 ${w} ${h}">${body}</svg>`;
  function body(markup) {
    const prefix = `d8-${instance++}-`;
    return markup.replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
      .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
      .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
      .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  }
  const text = (x, y, value, size = 28, font = 'Nunito', color = '{{INK}}', anchor = 'start') =>
    `<text x="${x}" y="${y}" font-family="${font}" font-weight="700" font-size="${size}" fill="${color}" text-anchor="${anchor}">${value}</text>`;
  const title = (x, y, value, size = 38, color = '{{INK}}') => text(x, y, value, size, 'Luckiest Guy', color, 'middle');
  function sprite(name, x, y, w, h) {
    const [sw, sh] = layout.assets.find(asset => asset.name === name).size;
    return `<svg x="${x}" y="${y}" width="${w ?? sw}" height="${h ?? sh}" viewBox="0 0 ${sw} ${sh}" preserveAspectRatio="none">${body(sources[name])}</svg>`;
  }
  // Actual nine-patch composition: fixed corners, stretched edges and center.
  // This makes the preview exercise exactly the borders documented for Image.Type.Sliced.
  function sliced(name, x, y, w, h) {
    const asset = layout.assets.find(item => item.name === name);
    const [sw, sh] = asset.size;
    const [l, b, r, t] = asset.borderPixels;
    if (w < l + r || h < t + b) throw new Error(`D8 slice smaller than borders: ${name}`);
    const sx = [0, l, sw - r, sw], sy = [0, t, sh - b, sh];
    const dx = [0, l, w - r, w], dy = [0, t, h - b, h];
    let result = '';
    for (let row = 0; row < 3; row++) for (let col = 0; col < 3; col++) {
      const scaleX = (dx[col + 1] - dx[col]) / (sx[col + 1] - sx[col]);
      const scaleY = (dy[row + 1] - dy[row]) / (sy[row + 1] - sy[row]);
      const id = `slice-${instance++}`;
      // Overlap only inside constant stretch bands to avoid clip antialias seams at 480x270.
      // Unity draws the nine slices in one mesh and does not need this SVG-only overlap.
      const ox = col > 0 ? 2 : 0, oy = row > 0 ? 2 : 0;
      result += `<defs><clipPath id="${id}"><rect x="${x + dx[col] - ox}" y="${y + dy[row] - oy}" width="${dx[col + 1] - dx[col] + ox + (col < 2 ? 2 : 0)}" height="${dy[row + 1] - dy[row] + oy + (row < 2 ? 2 : 0)}"/></clipPath></defs>`
        + `<g clip-path="url(#${id})"><g transform="translate(${x + dx[col]} ${y + dy[row]}) scale(${scaleX} ${scaleY}) translate(${-sx[col]} ${-sy[row]})">${body(sources[name])}</g></g>`;
    }
    return result;
  }
  function button(x, y, w, h, label, state = 'normal') {
    return sliced('button_yellow_9slice' + (state === 'normal' ? '' : '_' + state), x, y, w, h)
      + title(x + w / 2, y + h / 2 + 13 + (state === 'pressed' ? 4 : 0), label, 36);
  }
  function clipRect(x, y, w, h, content) {
    const id = `clip-${instance++}`;
    return `<defs><clipPath id="${id}"><rect x="${x}" y="${y}" width="${w}" height="${h}"/></clipPath></defs><g clip-path="url(#${id})">${content}</g>`;
  }
  function suspicion(x, y, value) {
    return `<rect x="${x + 15}" y="${y + 14}" width="390" height="36" rx="18" fill="{{PAPER_SHADE}}"/>`
      + clipRect(x + 15, y + 14, 390 * value, 36, sprite('bar_fill', x + 15, y + 14))
      + sprite('bar_frame', x, y) + title(x + 210, y - 11, 'SUSPICION', 28).replace('<text ', '<text stroke="{{PAPER}}" stroke-width="4" stroke-linejoin="round" paint-order="stroke fill" ');
  }
  function meow(x, y, value, threshold = layout.preview.threshold01) {
    const ty = 24 + 192 * (1 - threshold);
    const width = 2 * Math.sqrt(Math.max(0, 96 ** 2 - (192 * (.5 - threshold)) ** 2));
    const fill = `<g color="{{OK}}">${sprite('meow_fill', x + 24, y + 24).replace('fill="#FFFFFF"', 'fill="{{OK}}"')}</g>`;
    return sprite('meow_circle', x, y)
      + clipRect(x + 24, y + 24 + 192 * (1 - value), 192, 192 * value, fill)
      + sprite('meow_threshold_line', x + 120 - width / 2, y + ty - 6, width, 12)
      + title(x + 120, y + 144, 'MEOW', 44);
  }
  const paperPreview = paperOutputs(root)['previews/d6'].find(asset => asset.name === 'classroom_strokes').markup;
  const duck = duckOutputs(root)['exports/d7'];
  function classroom() {
    return body(paperPreview)
      + `<g transform="translate(1580 785)">${body(duck.find(a => a.name === 'duck_idle').markup)}</g>`
      + `<g transform="translate(1618 918)">${body(duck.find(a => a.name === 'keycap_q').markup)}</g>`;
  }
  function hud(risk = false) {
    const { hud, suspicion: bar, meow: mic } = layout;
    let content = classroom();
    if (risk) content += `<g opacity=".45">${sprite('vignette_radial', 0, 0, 1920, 1080).replaceAll('#FFFFFF', '{{DANGER}}')}</g>`;
    content += `<g transform="translate(${hud.logo[0]} ${hud.logo[1]}) scale(${1 / 1.5})">${sliced('chip_hud', 0, 0, hud.logo[2] * 1.5, hud.logo[3] * 1.5)}</g>` + title(182, 48, 'COPYCAT', 28);
    content += `<g transform="translate(${hud.answers[0]} ${hud.answers[1]}) scale(${1 / 1.5})">${sliced('chip_hud', 0, 0, hud.answers[2] * 1.5, hud.answers[3] * 1.5)}</g>` + title(960, 48, 'ANSWERS 7 / 12', 28);
    content += title(...hud.clockText, '0:44', 48, risk ? '{{DANGER}}' : '#C4EFAB');
    content += `<g transform="translate(${bar.rect[0]} ${bar.rect[1]}) scale(${bar.hudScale})">${suspicion(0, 0, risk ? .88 : .18)}</g>`;
    content += meow(mic.rect[0], mic.rect[1], risk ? .86 : .22);
    content += text(...mic.fallback, '[M] if no mic', 27, 'Nunito', '{{INK}}', 'middle');
    return frame(1920, 1080, content);
  }
  function menu() {
    let content = classroom() + '<rect width="1920" height="1080" fill="{{BOARD}}" opacity=".68"/>';
    content += sprite('logo_copycat', ...layout.menu.logo);
    content += text(960, 473, 'Cheat to win.', 44, 'Patrick Hand', '{{PAPER}}', 'middle');
    content += sliced('panel_paper_9slice', ...layout.menu.panel);
    content += button(...layout.menu.play, 'PLAY') + button(...layout.menu.quit, 'QUIT');
    content += text(960, 982, 'Microphone required. You will have to meow out loud.', 32, 'Nunito', '{{PAPER}}', 'middle');
    return frame(1920, 1080, content);
  }
  function report() {
    let content = classroom() + '<rect width="1920" height="1080" fill="{{INK}}" opacity=".55"/>';
    content += sliced('panel_paper_9slice', ...layout.report.panel);
    content += title(960, 196, 'EXAM PASSED!', 70);
    content += text(960, 244, '...You all passed? Suspicious.', 34, 'Patrick Hand', '{{PENCIL_INK}}', 'middle');
    content += '<path d="M1018 292 V772 M270 285 H1650" stroke="{{PAPER_SHADE}}" stroke-width="4"/>';
    content += sprite('stamp_grade_A+', ...layout.report.grade);
    for (let i = 0; i < 3; i++) content += sprite('star_filled', 266 + i * 96, 590);
    const stats = [['Answers copied','12/12'],['Time','2:14'],['Meows','8'],['Almost caught','1'],['Ducks thrown','0']];
    stats.forEach(([label, value], i) => {
      content += text(590, 380 + i * 57, label, 27) + title(925, 380 + i * 57, value, 34);
    });
    content += text(630, 739, 'Purrfect crime. No duck, no evidence.', 30, 'Patrick Hand', '{{PENCIL_INK}}', 'middle');
    content += title(1340, 330, 'TOP COPYCATS', 42);
    const columns = [[1080,'#'],[1120,'NAME'],[1335,'ANSWERS'],[1458,'TIME'],[1545,'GRADE']];
    columns.forEach(([x, label]) => { content += text(x, 378, label, 18); });
    const names = ['Mittens','Tuna','YOU','Beans','Noodle','Mochi','Paws','Luna','Milo','Boots'];
    for (let i = 0; i < 10; i++) {
      const y = 412 + i * 29;
      if (i === 2) content += `<rect x="1067" y="${y - 23}" width="550" height="29" rx="8" fill="{{UI_ACCENT}}"/>`;
      [String(i + 1), names[i], String(12 - Math.floor(i / 3)), i < 3 ? ['1:58','2:09','2:14'][i] : '3:00', i < 3 ? 'A+' : i < 6 ? 'A' : 'B']
        .forEach((value, col) => { content += text(columns[col][0], y, value, 21); });
    }
    content += text(1340, 745, 'Your best: 12 answers · 2:14', 23, 'Nunito', '{{INK}}', 'middle');
    content += button(...layout.report.retake, 'RETAKE EXAM [R]') + button(...layout.report.menu, 'MAIN MENU');
    return frame(1920, 1080, content);
  }
  function stateSheet() {
    let content = '<rect width="1920" height="1420" fill="{{WALL}}"/>';
    content += title(960, 68, 'COPYCAT / D8 / UI KIT', 48);
    content += text(45, 115, 'Paper panels, yellow buttons, suspicion, microphone, grades and controls. Text stays live in Unity.', 26);
    ['normal','hover','pressed'].forEach((state, i) => {
      content += button(40 + i * 625, 150, 585, 100, 'START EXAM', state);
      content += title(332 + i * 625, 285, state.toUpperCase(), 24);
    });
    layout.preview.suspicion.forEach((value, i) => {
      const x = 40 + i * 470;
      content += suspicion(x, 363, value) + title(x + 210, 459, String(Math.round(value * 100)) + '%', 26);
      content += meow(x + 90, 490, layout.preview.mic[i]);
      content += title(x + 210, 772, ['SILENT','LOUDER!','THRESHOLD','LOUD ENOUGH!'][i], 26);
    });
    ['F','D','C','B','A','A+'].forEach((grade, i) => { content += sprite('stamp_grade_' + grade, 60 + i * 310, 815); });
    for (let i = 0; i < 3; i++) content += sprite(i < 2 ? 'star_filled' : 'star_empty', 72 + i * 104, 1120);
    [['space',160],['m',64],['1',64],['2',64],['3',64],['4',64]].reduce((x, [key,w]) => {
      content += sprite('keycap_' + key, x, 1140); return x + w + 28;
    }, 490);
    content += sprite('icon_app', 1450, 1080, 220, 220) + sprite('icon_app', 1710, 1100, 64, 64) + sprite('icon_app', 1810, 1100, 32, 32);
    content += text(45, 1358, '27 assets / @2x except icon 1024 and vignette 512 / 32px slices -> 64px texture borders / PPU 200 / no trim', 24);
    return frame(1920, 1420, content);
  }
  function sliceSheet() {
    let content = '<rect width="1920" height="1120" fill="{{WALL}}"/>' + title(960, 68, 'D8 / FIXED CORNERS, FLEXIBLE PANELS', 44);
    [[128,128],[420,160],[720,280]].forEach(([w,h], i) => {
      const x = [45,245,760][i];
      content += sliced('panel_paper_9slice', x, 130, w, h) + text(x, 110, `${w} x ${h}`, 24);
    });
    content += sliced('panel_paper_9slice', 1515, 130, 355, 580) + title(1692, 213, 'TALL PANEL', 28);
    ['normal','hover','pressed'].forEach((state, i) => {
      content += button(45, 470 + i * 155, 600, 108, 'RETAKE EXAM [R]', state);
      content += button(710, 470 + i * 155, 320, 108, 'PLAY', state);
      content += sliced('chip_hud', 1090, 470 + i * 155, 380, 88) + title(1280, 529 + i * 155, 'ANSWERS 12 / 12', 32);
    });
    content += text(50, 1010, 'Slices are drawn as nine independent source rectangles. Corners and outline keep the same size.', 28);
    content += text(50, 1052, 'Unity borders L/B/R/T = 64/64/64/64 on @2x textures. Canvas reference PPU 100, sprite PPU 200.', 26);
    return frame(1920, 1120, content);
  }
  return {
    'exports/d8': layout.assets.map(asset => ({ name: asset.name, markup: sources[asset.name], scale: asset.textureScale })),
    'previews/d8': [
      { name: 'state_sheet', markup: stateSheet(), scale: 1 },
      { name: 'slice_sheet', markup: sliceSheet(), scale: 1 },
      { name: 'main_menu', markup: menu(), scale: 1 },
      { name: 'hud_calm', markup: hud(), scale: 1 },
      { name: 'hud_risk', markup: hud(true), scale: 1 },
      { name: 'report_card', markup: report(), scale: 1 },
      { name: 'menu_thumbnail', markup: menu(), scale: .25 },
      { name: 'hud_thumbnail', markup: hud(true), scale: .25 },
      { name: 'report_thumbnail', markup: report(), scale: .25 }
    ]
  };
}
