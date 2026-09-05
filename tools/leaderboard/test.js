const rows = [];
const fakeSheet = {
  getLastRow: () => rows.length,
  appendRow: r => rows.push(r.slice()),
  setFrozenRows: () => {},
  getName: () => 'scores',
  getDataRange: () => ({ getValues: () => rows.map(r => r.slice()) })
};
globalThis.SpreadsheetApp = {
  openById: () => ({ getSheetByName: () => (rows.length ? fakeSheet : null), insertSheet: () => fakeSheet }),
  flush: () => {}
};
globalThis.LockService = { getScriptLock: () => ({ waitLock: () => {}, releaseLock: () => {} }) };
globalThis.ContentService = { MimeType: { JSON: 'json' }, createTextOutput: t => ({ text: t, setMimeType: function () { return this; } }) };
globalThis.Logger = { log: () => {} };
const fs = require('fs');
eval(fs.readFileSync(__dirname + '/Code.gs', 'utf8'));

const post = b => JSON.parse(doPost({ postData: { contents: JSON.stringify(b) } }).text);
const get = p => JSON.parse(doGet({ parameter: p }).text);
let failed = 0;
const check = (label, actual, expected) => {
  const ok = JSON.stringify(actual) === JSON.stringify(expected);
  if (!ok) failed++;
  console.log((ok ? 'PASS ' : 'FAIL ') + label + (ok ? '' : `\n  got      ${JSON.stringify(actual)}\n  expected ${JSON.stringify(expected)}`));
};

check('empty board', get({}), { top: [], rank: 0, total: 0 });

const r1 = post({ name: 'Egor', answers: 12, timeSeconds: 91.374, grade: 'A+' });
check('first post rank', [r1.rank, r1.total, r1.top[0]], [1, 1, { name: 'Egor', answers: 12, timeSeconds: 91.37, grade: 'A+' }]);

post({ name: '  Мяу🐱<script>Fluffy  ', answers: 12, timeSeconds: 80, grade: 'a+' });
check('name whitelist strips non-ascii/markup chars, keeps word chars', rows[2].slice(0, 4), ['scriptFluffy', 12, 80, 'A+']);

post({ name: '🐱🐱🐱', answers: 5, timeSeconds: 20, grade: 'D' });
check('empty after filter -> Anonymous', rows[3][0], 'Anonymous');

post({ name: 'ClampMe', answers: 999, timeSeconds: -5, grade: 'Z9' });
check('clamped answers/time/grade', rows[4].slice(0, 4), ['ClampMe', 12, 0, 'F']);

post({});
check('missing body fields', rows[5].slice(0, 4), ['Anonymous', 0, 0, 'F']);

check('sorted: answers desc then time asc', get({}).top.map(r => [r.name, r.answers, r.timeSeconds]),
  [['ClampMe', 12, 0], ['scriptFluffy', 12, 80], ['Egor', 12, 91.37], ['Anonymous', 5, 20], ['Anonymous', 0, 0]]);

check('rank lookup by GET', get({ name: 'Egor', answers: '12', timeSeconds: '91.37' }).rank, 3);
check('rank 0 when absent', get({ name: 'Ghost', answers: '1', timeSeconds: '1' }).rank, 0);
check('top clamped to 50', get({ top: '900' }).top.length, 5);
check('top default 10 on garbage', get({ top: 'abc' }).top.length, 5);
check('top=1', get({ top: '1' }).top.length, 1);

const broken = JSON.parse(doPost({ postData: { contents: '{not json' } }).text);
check('malformed json returns json error', [broken.top, broken.rank, typeof broken.error], [[], 0, 'string']);
check('header row written', rows[0], ['name', 'answers', 'timeSeconds', 'grade', 'dateUtc']);
check('dateUtc is iso', /^\d{4}-\d{2}-\d{2}T/.test(rows[1][4]), true);

console.log(failed === 0 ? '\nALL PASS' : `\n${failed} FAILED`);
process.exit(failed === 0 ? 0 : 1);
