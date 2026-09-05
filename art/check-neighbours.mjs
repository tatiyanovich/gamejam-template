import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';

const root = dirname(fileURLToPath(import.meta.url));
const require = createRequire(import.meta.url);
const { Resvg } = require(process.env.COPYCAT_RESVG_MODULE || '@resvg/resvg-js');
const rig = JSON.parse(readFileSync(join(root, 'src/d5/rig.json'), 'utf8'));
const palette = JSON.parse(readFileSync(join(root, 'palette.json'), 'utf8'));
const source = name => readFileSync(join(root, `src/d5/${name}.svg`), 'utf8')
  .replace(/^<svg[^>]*>/, '').replace(/<\/svg>\s*$/, '')
  .replace(/\{\{([A-Z_]+)\}\}/g, (_, key) => palette[key]);

for (const name of Object.keys(rig.characters)) {
  for (let step = 0; step <= 110; step++) {
    const progress = step / 100;
    const [px, py] = rig.placements.paw;
    const pose = rig.poses.lifted;
    const markup = `<svg xmlns="http://www.w3.org/2000/svg" width="540" height="760" viewBox="0 0 540 760">${source(`${name}_body`)}<g transform="translate(${px} ${py + pose.offsetY * progress}) rotate(${pose.rotation * progress}) translate(${-px} ${-py})">${source(`${name}_paw_cover`)}</g></svg>`;
    const rendered = new Resvg(markup, {fitTo: {mode: 'zoom', value: .5}}).render();
    const { width, height, pixels } = rendered;
    const occupied = new Uint8Array(width * height);
    let total = 0;
    for (let index = 0; index < occupied.length; index++) {
      occupied[index] = pixels[index * 4 + 3] >= 128 ? 1 : 0;
      total += occupied[index];
    }
    const start = occupied.indexOf(1);
    assert.ok(start >= 0);
    const queue = [start];
    occupied[start] = 0;
    for (let cursor = 0; cursor < queue.length; cursor++) {
      const index = queue[cursor];
      const x = index % width;
      const neighbours = [index - width, index + width];
      if (x > 0) neighbours.push(index - 1);
      if (x + 1 < width) neighbours.push(index + 1);
      for (const next of neighbours) {
        if (next >= 0 && next < occupied.length && occupied[next]) {
          occupied[next] = 0;
          queue.push(next);
        }
      }
    }
    assert.equal(queue.length, total, `${name}: detached silhouette at ${step}% lift`);
  }
}
console.log('D5: both body/paw silhouettes remain connected at 111 samples each, including OutBack overshoot.');
