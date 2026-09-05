import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';

const root = dirname(fileURLToPath(import.meta.url));
const require = createRequire(import.meta.url);
const { Resvg } = require(process.env.COPYCAT_RESVG_MODULE || '@resvg/resvg-js');
const layout = JSON.parse(readFileSync(join(root, 'src/d8/layout.json'), 'utf8'));
const rendered = {};
for (const asset of layout.assets) {
  const markup = readFileSync(join(root, `exports/d8/${asset.name}.svg`), 'utf8');
  const image = new Resvg(markup).render();
  const pixels = image.pixels;
  const pixel = (x, y) => [...pixels.subarray((y * image.width + x) * 4, (y * image.width + x) * 4 + 4)];
  const alpha = (x, y) => pixel(x, y)[3];
  rendered[asset.name] = { ...asset, pixel, alpha };
  assert.deepEqual([image.width, image.height], asset.size, `${asset.name}: source size`);
  assert.ok(!/<text\b|\{\{/.test(markup), `${asset.name}: exported SVG must be standalone paths`);
  const [w, h] = asset.size;
  if (!['icon_app','vignette_radial','bar_fill','meow_fill','keycap_space','keycap_m','keycap_1','keycap_2','keycap_3','keycap_4'].includes(asset.name)) {
    for (let x = 0; x < w; x++) assert.ok(alpha(x, 0) === 0 && alpha(x, h - 1) === 0, `${asset.name}: clipped top/bottom outline`);
    for (let y = 0; y < h; y++) assert.ok(alpha(0, y) === 0 && alpha(w - 1, y) === 0, `${asset.name}: clipped left/right outline`);
  }
  if (asset.borderPixels) {
    const [left, bottom, right, top] = asset.borderPixels;
    // Stretch bands must be constant. Curves crossing a slice cause visible kinks when resized.
    for (let y = 0; y < h; y++) for (let x = left + 2; x < w - right - 2; x++) {
      assert.deepEqual(pixel(x, y), pixel(left + 2, y), `${asset.name}: curved horizontal stretch band at ${x},${y}`);
    }
    for (let x = 0; x < w; x++) for (let y = top + 2; y < h - bottom - 2; y++) {
      assert.deepEqual(pixel(x, y), pixel(x, top + 2), `${asset.name}: curved vertical stretch band at ${x},${y}`);
    }
  }
}
const icon = rendered.icon_app;
for (let y = 0; y < 1024; y++) for (let x = 0; x < 1024; x++) assert.equal(icon.alpha(x, y), 255, 'App icon must be fully opaque');
const vignette = rendered.vignette_radial;
assert.equal(vignette.alpha(256, 256), 0, 'Vignette center stays clear');
assert.ok(vignette.alpha(0, 0) > 250 && vignette.alpha(0, 256) > 250, 'Vignette covers edges');
assert.deepEqual(vignette.pixel(0, 0).slice(0, 3), [255, 255, 255], 'Vignette supports runtime tint');
const fill = rendered.meow_fill;
assert.equal(fill.alpha(0, 0), 0, 'Meow fill must be circular');
assert.deepEqual(fill.pixel(96, 96), [255, 255, 255, 255], 'Meow fill supports runtime tint');
const bar = rendered.bar_fill;
assert.ok(bar.pixel(0, 18)[1] > bar.pixel(389, 18)[1] + 100, 'Suspicion gradient runs yellow to red');
// The Space adapter must preserve a solid key face right across the widened canvas.
const space = rendered.keycap_space;
for (let x = 14; x <= 145; x++) assert.equal(space.alpha(x, 20), 255, `Space key face broken at x=${x}`);
for (const side of ['keycap_m','keycap_space']) {
  assert.deepEqual(rendered[side].pixel(8, 30), rendered.keycap_1.pixel(8, 30), `${side}: shared left edge`);
}
const normal = rendered.button_yellow_9slice.pixel(64, 64);
const hover = rendered.button_yellow_9slice_hover.pixel(64, 64);
const pressed = rendered.button_yellow_9slice_pressed.pixel(64, 64);
assert.ok(hover.slice(0, 3).reduce((sum, n, i) => sum + Math.abs(n - normal[i]), 0) > 60, 'Hover must be visibly different');
assert.ok(pressed[0] < normal[0] - 30, 'Pressed state must be visibly darker');
console.log('D8: 27 standalone sprites; unclipped outlines, constant slice bands, opaque icon, tintable masks, gradient and widened Space verified.');
