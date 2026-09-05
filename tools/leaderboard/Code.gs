/**
 * COPYCAT leaderboard — Google Apps Script web app.
 * Spreadsheet: https://docs.google.com/spreadsheets/d/16S1OR1MXPm18jIZbqPbqVxVEikHrkhA_pou0qC3AXH0/edit
 * Deploy steps and smoke test: tools/leaderboard/README.md
 */

const SPREADSHEET_ID = '16S1OR1MXPm18jIZbqPbqVxVEikHrkhA_pou0qC3AXH0';
const SHEET_NAME = 'scores';
const HEADER = ['name', 'answers', 'timeSeconds', 'grade', 'dateUtc'];

const DEFAULT_NAME = 'Anonymous';
const MAX_NAME_LENGTH = 12;
const MAX_ANSWERS = 12;
const MAX_TIME_SECONDS = 999;
const DEFAULT_TOP = 10;
const MAX_TOP = 50;
const GRADES = ['F', 'D', 'C', 'B', 'A', 'A+'];
const LOCK_TIMEOUT_MS = 10000;

/** Run once from the Apps Script editor to create the sheet and its header row. */
function setup() {
  const target = sheet();
  target.setFrozenRows(1);
  Logger.log('Ready: ' + target.getName() + ', rows: ' + target.getLastRow());
}

function doPost(e) {
  try {
    const body = JSON.parse((e && e.postData && e.postData.contents) || '{}');
    const entry = {
      name: sanitizeName(body.name),
      answers: clampInt(body.answers, 0, MAX_ANSWERS),
      timeSeconds: clampTime(body.timeSeconds),
      grade: sanitizeGrade(body.grade)
    };

    const lock = LockService.getScriptLock();
    lock.waitLock(LOCK_TIMEOUT_MS);
    try {
      sheet().appendRow([entry.name, entry.answers, entry.timeSeconds, entry.grade, new Date().toISOString()]);
      SpreadsheetApp.flush();
      return json(board(DEFAULT_TOP, entry));
    } finally {
      lock.releaseLock();
    }
  } catch (error) {
    return json(failure(error));
  }
}

function doGet(e) {
  try {
    const parameters = (e && e.parameter) || {};
    const entry = parameters.name === undefined ? null : {
      name: sanitizeName(parameters.name),
      answers: clampInt(parameters.answers, 0, MAX_ANSWERS),
      timeSeconds: clampTime(parameters.timeSeconds)
    };
    return json(board(readTop(parameters.top), entry));
  } catch (error) {
    return json(failure(error));
  }
}

/** { top: [{ name, answers, timeSeconds, grade }], rank, total }. rank is 0 when the entry is not found. */
function board(top, entry) {
  const rows = readRows();
  const rank = entry === null ? 0 : 1 + rows.findIndex(row => isSameEntry(row, entry));
  return { top: rows.slice(0, top), rank: rank, total: rows.length };
}

function readRows() {
  return sheet().getDataRange().getValues().slice(1)
    .filter(row => String(row[0]).trim() !== '')
    .map(row => ({
      name: String(row[0]),
      answers: Number(row[1]) || 0,
      timeSeconds: Number(row[2]) || 0,
      grade: String(row[3])
    }))
    .sort((left, right) => right.answers - left.answers || left.timeSeconds - right.timeSeconds);
}

function isSameEntry(row, entry) {
  return row.name === entry.name && row.answers === entry.answers && row.timeSeconds === entry.timeSeconds;
}

function sheet() {
  const book = SpreadsheetApp.openById(SPREADSHEET_ID);
  const target = book.getSheetByName(SHEET_NAME) || book.insertSheet(SHEET_NAME);
  if (target.getLastRow() === 0) {
    target.appendRow(HEADER);
  }
  return target;
}

function sanitizeName(value) {
  const cleaned = String(value === undefined || value === null ? '' : value)
    .replace(/[^\w \-]/g, '')
    .trim()
    .slice(0, MAX_NAME_LENGTH)
    .trim();
  return cleaned.length > 0 ? cleaned : DEFAULT_NAME;
}

function sanitizeGrade(value) {
  const grade = String(value === undefined || value === null ? '' : value).toUpperCase().slice(0, 2);
  return GRADES.indexOf(grade) >= 0 ? grade : 'F';
}

function readTop(value) {
  const requested = Math.round(Number(value));
  return isFinite(requested) && requested > 0 ? Math.min(MAX_TOP, requested) : DEFAULT_TOP;
}

function clampInt(value, minimum, maximum) {
  const number = Math.round(Number(value));
  return isFinite(number) ? Math.max(minimum, Math.min(maximum, number)) : minimum;
}

function clampTime(value) {
  const number = Number(value);
  return isFinite(number) ? Math.max(0, Math.min(MAX_TIME_SECONDS, Math.round(number * 100) / 100)) : 0;
}

function failure(error) {
  return { top: [], rank: 0, total: 0, error: String(error) };
}

function json(payload) {
  return ContentService.createTextOutput(JSON.stringify(payload)).setMimeType(ContentService.MimeType.JSON);
}
