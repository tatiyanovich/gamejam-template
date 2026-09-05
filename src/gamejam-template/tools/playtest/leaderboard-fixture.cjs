const http = require('node:http');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const rows = [];
const sheet = {
  getLastRow: () => rows.length,
  appendRow: row => rows.push(row.slice()),
  getDataRange: () => ({getValues: () => rows.map(row => row.slice())})
};
const context = vm.createContext({
  SpreadsheetApp: {openById: () => ({getSheetByName: () => sheet}), flush() {}},
  LockService: {getScriptLock: () => ({waitLock() {}, releaseLock() {}})},
  ContentService: {MimeType: {JSON: 'json'}, createTextOutput: text => ({text, setMimeType() {return this;}})}
});
vm.runInContext(fs.readFileSync(path.resolve(__dirname, '../../../../tools/leaderboard/Code.gs'), 'utf8'), context);
const server = http.createServer((request, response) => {
  let body = '';
  request.on('data', chunk => body += chunk);
  request.on('end', () => {
    response.setHeader('Content-Type', 'application/json');
    if (request.url === '/slow') return setTimeout(() => response.end('{"top":[],"rank":1,"total":1}'), 12000);
    if (request.url === '/error') {response.statusCode = 503; return response.end('unavailable');}
    if (request.url === '/invalid') return response.end('not json');
    if (request.url === '/empty') return response.end('{"top":[],"rank":0,"total":0}');
    if (request.url === '/negative') return response.end('{"top":[],"rank":-1,"total":-1}');
    if (request.url === '/null') return response.end('{"top":[null],"rank":1,"total":1}');
    if (request.url === '/failure') return response.end('{"top":[],"rank":0,"total":0,"error":"locked"}');
    if (request.url === '/redirect') {response.statusCode = 302; response.setHeader('Location', '/board'); return response.end();}
    if (request.url === '/reset') {rows.length = 0; return response.end('{}');}
    const output = request.method === 'POST' ? context.doPost({postData: {contents: body}}) : context.doGet({parameter: {}});
    response.end(output.text);
  });
});
server.listen(18764, '127.0.0.1', () => console.log('COPYCAT C3 fixture listening on 127.0.0.1:18764; no live sheet writes'));
