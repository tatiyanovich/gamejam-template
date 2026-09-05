import { readFileSync, writeFileSync, mkdirSync, readdirSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';
import { createHash } from 'node:crypto';
import { classroomOutputs } from './classroom.mjs';
import { kittenOutputs } from './kitten.mjs';
import { teacherOutputs } from './teacher.mjs';
import { neighbourOutputs } from './neighbours.mjs';
import { paperOutputs } from './papers.mjs';
import { duckOutputs } from './duck.mjs';
import { uiOutputs } from './ui.mjs';

const root = dirname(fileURLToPath(import.meta.url));
const require = createRequire(import.meta.url);
const { Resvg } = require(process.env.COPYCAT_RESVG_MODULE || '@resvg/resvg-js');
const check = process.argv.includes('--check');
const palette = JSON.parse(readFileSync(join(root, 'palette.json'), 'utf8'));
const sourceRoot = join(root, 'src/d1');
const outputRoot = join(root, 'previews/d1');
const fontFiles = ['LuckiestGuy.ttf', 'PatrickHand-Regular.ttf', 'Nunito-Bold.ttf']
  .map(name => join(root, 'fonts', name));
const sources = Object.fromEntries(readdirSync(sourceRoot).filter(name => name.endsWith('.svg'))
  .map(name => [name.slice(0, -4), readFileSync(join(sourceRoot, name), 'utf8')]));
const generated = [];
let instance = 0;

function colors(svg) {
  return svg.replace(/\{\{([A-Z_]+)\}\}/g, (token, key) => {
    if (!(key in palette)) throw new Error(`Unresolved token ${token}`);
    return palette[key];
  });
}

function artwork(name, x, y, scale = 1) {
  const prefix = `art-${instance++}-`;
  const svg = sources[name].replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
    .replace(/\bid="([^"]+)"/g, (_, id) => `id="${prefix}${id}"`)
    .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}${id}"`)
    .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}${id})`);
  return `<g transform="translate(${x} ${y}) scale(${scale})">${svg}</g>`;
}

function svg(width, height, content) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">${content}</svg>`;
}

function text(x, y, content, size = 24, font = 'Nunito', fill = '{{INK}}', extra = '') {
  return `<text x="${x}" y="${y}" font-family="${font}" font-size="${size}" fill="${fill}" ${extra}>${content}</text>`;
}

function paper(x, y, width, height) {
  return `<path d="M${x + 15} ${y + 8} L${x + width - 8} ${y + 3} Q${x + width + 3} ${y + 4} ${x + width} ${y + 19} L${x + width - 3} ${y + height - 14} Q${x + width - 5} ${y + height + 1} ${x + width - 20} ${y + height} L${x + 8} ${y + height - 4} Q${x - 5} ${y + height - 6} ${x} ${y + height - 20} L${x + 2} ${y + 24} Q${x + 2} ${y + 8} ${x + 15} ${y + 8}Z" fill="{{PAPER}}" stroke="{{INK}}" stroke-width="7" stroke-linejoin="round"/>`;
}

function scene(watching) {
  return sources.classroom_test
    .replace('{{SLOT_TEACHER_SHADOW}}', watching
      ? '<ellipse cx="1052" cy="602" rx="90" ry="16" fill="{{INK}}" opacity=".12" stroke="none"/>'
      : '')
    .replace('{{SLOT_TEACHER_BACK}}', watching ? '' : artwork('teacher_writing', 976, 235, .4))
    .replace('{{SLOT_TEACHER_FRONT}}', watching ? artwork('teacher_watching', 976, 364, .4) : '')
    .replace('{{SLOT_DESK}}', artwork('desk', 0, 733))
    .replace('{{SLOT_DUCK}}', artwork('duck', 1575, 769))
    .replace('{{SLOT_KITTEN}}', watching
      ? `<g transform="rotate(-9 888 1058)">${artwork('kitten_lean', 593, 628)}</g>`
      : artwork('kitten_back', 653, 628));
}

function styleSheet() {
  const swatches = ['INK', 'PAPER', 'WALL', 'BOARD', 'WOOD', 'KITTEN', 'TEACHER', 'TEACHER_DRESS', 'GLASSES_RED', 'DUCK', 'OK', 'DANGER'];
  let content = `<rect width="1920" height="1500" fill="{{WALL}}"/>`;
  content += `<path d="M0 159 H1920 M1286 185 V900 M0 923 H1920" stroke="{{INK}}" stroke-width="3" opacity=".25"/>`;
  content += text(48, 116, 'COPYCAT', 110, 'Luckiest Guy', '{{UI_ACCENT}}', 'stroke="{{INK}}" stroke-width="7" paint-order="stroke fill" transform="rotate(-2 48 116)"');
  content += text(623, 64, 'CHEAT TO WIN.  MEOW TO SURVIVE.', 29, 'Luckiest Guy');
  content += text(623, 104, 'Chunky shapes. Guilty eyes. One very useful duck.', 27, 'Patrick Hand');
  content += text(1866,  60, 'D1 / STYLE STUDY', 22, 'Nunito', '{{INK}}', 'text-anchor="end" font-weight="800"');
  content += text(1866, 105, '05 SEPT 2026', 22, 'Patrick Hand', '{{INK}}', 'text-anchor="end"');
  content += text(48, 198, '01 / THE CLASSROOM TEST', 23, 'Nunito', '{{INK}}', 'font-weight="800"');
  const sceneBody = scene(true).replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '');
  content += `<svg x="48" y="220" width="1216" height="684" viewBox="0 0 1920 1080">${sceneBody}</svg>`;
  content += `<rect x="48" y="220" width="1216" height="684" rx="4" fill="none" stroke="{{INK}}" stroke-width="7"/>`;
  content += text(1315, 198, '02 / THE PALETTE', 23, 'Nunito', '{{INK}}', 'font-weight="800"');
  swatches.forEach((name, index) => {
    const x = 1315 + index % 3 * 188;
    const y = 224 + Math.floor(index / 3) * 132;
    content += `<path d="M${x + 7} ${y} L${x + 157} ${y + 2} Q${x + 166} ${y + 2} ${x + 166} ${y + 13} L${x + 163} ${y +  60} Q${x + 164} ${y + 71} ${x + 152} ${y + 70} L${x + 5} ${y + 68} Q${x - 1} ${y + 68} ${x} ${y + 57} L${x + 1} ${y + 10} Q${x + 1} ${y} ${x + 7} ${y}Z" fill="${palette[name]}" stroke="{{INK}}" stroke-width="5"/>`;
    content += text(x, y + 94, name.replaceAll('_', ' '), 17, 'Nunito', '{{INK}}', 'font-weight="800"');
    content += text(x, y + 117, palette[name], 18, 'Nunito');
  });
  content += text(1315, 802, 'One shadow. No texture debt.', 30, 'Patrick Hand');
  content += text(1315, 846, 'Warm cats / cool classroom', 26, 'Patrick Hand');
  content += text(1315, 884, 'Yellow belongs to the escape plan.', 26, 'Patrick Hand');
  content += text(48, 966, '03 / READ THE POSE BEFORE THE HUD', 23, 'Nunito', '{{INK}}', 'font-weight="800"');
  content += paper(48, 987, 704, 454);
  content += text( 80, 1033, 'THE UNPREPARED ONE', 28, 'Luckiest Guy');
  content += artwork('kitten_back', 76, 1062, .58);
  content += artwork('kitten_lean', 401, 1062, .58);
  content += text(220, 1370, 'Innocent. Allegedly.', 25, 'Patrick Hand', '{{PENCIL_INK}}', 'text-anchor="middle"');
  content += text(540, 1370, 'Just checking something.', 25, 'Patrick Hand', '{{PENCIL_INK}}', 'text-anchor="middle"');
  content += text(80, 1413, 'Back view / sideways lean. Eyes follow the answer.', 21, 'Nunito');
  content += paper(780, 987, 548, 454);
  content += text(812, 1033, 'MRS. HISSKINS', 28, 'Luckiest Guy');
  content += artwork('teacher_writing', 810, 1052, .47);
  content += artwork('teacher_watching', 1090, 1052, .47);
  content += text(902, 1370, 'Writing', 25, 'Patrick Hand', '{{PENCIL_INK}}', 'text-anchor="middle"');
  content += text(1180, 1370, 'Watching', 25, 'Patrick Hand', '{{PENCIL_INK}}', 'text-anchor="middle"');
  content += text(812, 1413, 'Red glasses + pointer = immediate authority.', 21, 'Nunito');
  content += paper(1356, 987, 515, 454);
  content += text(1388, 1033, '04 / MARKS &amp; TYPE', 26, 'Luckiest Guy');
  [[1082, 10, '10 px / character'], [1123, 6, '6 px / small prop'], [1167, 14, '14 px / UI panel']].forEach(([y, width, label]) => {
    content += `<path d="M1393 ${y} Q1430 ${y - 7} 1473 ${y}" fill="none" stroke="{{INK}}" stroke-width="${width}" stroke-linecap="round"/>`;
    content += text(1500, y + 7, label, 23, 'Patrick Hand');
  });
  content += text(1388, 1230, 'COPIED!', 43, 'Luckiest Guy', '{{PENCIL_INK}}');
  content += text(1388, 1270, 'Luckiest Guy / titles &amp; numbers', 21, 'Nunito');
  content += text(1388, 1326, 'Answer: definitely tuna.',  30, 'Patrick Hand', '{{PENCIL_INK}}');
  content += text(1388, 1365, 'Patrick Hand / paper &amp; speech', 21, 'Nunito');
  content += text(1388, 1412, 'Nunito Bold / readable UI', 24, 'Nunito', '{{INK}}', 'font-weight="800"');
  content += text(48, 1479, 'ART DIRECTION REFERENCE / Pose studies and composition mockup; production layers follow in D2–D11.', 20, 'Nunito');
  return svg(1920, 1500, content);
}

function validate(markup, name) {
  if (/\{\{/.test(markup)) throw new Error(`${name}: unresolved template`);
  for (const match of markup.matchAll(/\sd="([^"]*)"/g)) {
    if (/[^MmZzLlHhVvCcSsQqTtAaEe0-9+.,\s-]/.test(match[1])) {
      throw new Error(`${name}: invalid path ${match[1]}`);
    }
  }
  const ids = [...markup.matchAll(/\bid="([^"]+)"/g)].map(match => match[1]);
  if (new Set(ids).size !== ids.length) throw new Error(`${name}: duplicate SVG IDs`);
  for (const match of markup.matchAll(/(?:href="#|url\(#)([^"\)]+)/g)) {
    if (!ids.includes(match[1])) throw new Error(`${name}: unresolved SVG reference ${match[1]}`);
  }
}

function output(name, markup, scale = 1, directory = outputRoot, manifestEntries = generated) {
  const resolved = colors(markup);
  validate(resolved, name);
  const renderer = new Resvg(resolved, {
    fitTo: { mode: 'zoom', value: scale },
    font: { fontFiles, loadSystemFonts: false, defaultFontFamily: 'Nunito' }
  });
  const rendered = renderer.render();
  const png = rendered.asPng();
  const files = [[`${name}.svg`, Buffer.from(renderer.toString())], [`${name}.png`, png]];
  for (const [file, data] of files) {
    const path = join(directory, file);
    if (check) {
      if (!readFileSync(path).equals(data)) throw new Error(`${file}: rebuild required`);
    } else {
      writeFileSync(path, data);
    }
  }
  manifestEntries.push({ name, width: rendered.width, height: rendered.height, scale, sha256: createHash('sha256').update(png).digest('hex') });
  console.log(`${check ? 'Verified' : 'Rendered'} ${name}: ${rendered.width}×${rendered.height}`);
}

const classroom = { ...classroomOutputs(root), ...kittenOutputs(root), ...teacherOutputs(root), ...neighbourOutputs(root), ...paperOutputs(root), ...duckOutputs(root), ...uiOutputs(root) };
for (const [directory, assets] of Object.entries(classroom)) {
  const target = join(root, directory);
  const entries = [];
  if (!check) mkdirSync(target, { recursive: true });
  for (const asset of assets) output(asset.name, asset.markup, asset.scale, target, entries);
  const data = Buffer.from(`${JSON.stringify(entries, null, 2)}\n`);
  const path = join(target, 'manifest.json');
  if (check) {
    if (!readFileSync(path).equals(data)) throw new Error(`${directory}/manifest.json: rebuild required`);
  } else {
    writeFileSync(path, data);
  }
}

mkdirSync(outputRoot, { recursive: true });
for (const name of ['kitten_back', 'kitten_lean', 'teacher_writing', 'teacher_watching', 'desk', 'duck']) {
  output(name, sources[name], 2);
}
output('classroom_calm', scene(false));
output('classroom_risk', scene(true));
output('classroom_thumbnail', scene(true), .25);
output('style_sheet', styleSheet());
const manifest = Buffer.from(`${JSON.stringify(generated, null, 2)}\n`);
const manifestPath = join(outputRoot, 'manifest.json');
if (check) {
  if (!readFileSync(manifestPath).equals(manifest)) throw new Error('manifest.json: rebuild required');
} else {
  writeFileSync(manifestPath, manifest);
}
