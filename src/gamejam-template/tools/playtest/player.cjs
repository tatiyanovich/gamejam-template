const fs = require('node:fs');
const path = require('node:path');
const {spawnSync} = require('node:child_process');
const project = path.resolve(__dirname, '../..');
const statePath = path.join(project, 'Temp/Playtest/state.txt');
const binary = path.join(project, 'Temp/Playtest/input-macos');
const pid = process.argv[2];
if (!/^\d+$/.test(pid || '')) throw new Error('Usage: node tools/playtest/player.cjs UNITY_PID');
const answers = [[126,124],[123,126,124],[125,124,126],[18],[126,123,125],[11,34,15,2],[125,123,126,124],[19],[123,126,124,125,123],[35,32,15,15,1],[8,0,17,45,34,35],[123,124,126,125,123,126]];
function input(...actions) {
  const result = spawnSync(binary, [pid, ...actions], {encoding: 'utf8'});
  if (result.status !== 0) throw new Error(result.error || result.stdout + result.stderr);
}
const tap = key => [`down:${key}`, 'wait:0.12', `up:${key}`, 'wait:0.27'];
const field = (line, name) => Number(line?.match(new RegExp(name + '=\\{"Value":([^}]+)'))?.[1]);
const deadline = Date.now() + 125000;
let previous = '';
try {
  while (Date.now() < deadline) {
    if (Date.now() - fs.statSync(statePath).mtimeMs > 3000) throw new Error('Recording stopped or stale state file');
    const lines = fs.readFileSync(statePath, 'utf8').split('\n');
    const run = lines.find(x => x.includes('ExamRun={}'));
    if (run?.includes('ExamFinished={}')) {
      console.log('FINISH', run);
      process.exitCode = field(run, 'ExamOutcomeComponent') === 1 ? 0 : 1;
      break;
    }
    const question = lines.find(x => x.includes('Question={}'));
    const teacher = lines.find(x => x.includes('Teacher={}'));
    const duck = lines.find(x => x.includes('Duck={}'));
    if (!run || !question || !teacher || !duck) {input('up:49', 'wait:0.2'); continue;}
    const index = field(question, 'QuestionIndex');
    const progress = field(question, 'AnswerProgress');
    const attention = field(teacher, 'TeacherAttentionComponent');
    const suspicion = field(run, 'SuspicionLevel');
    const signature = `${index + 1}:${progress} teacher=${attention} suspicion=${suspicion.toFixed(0)}`;
    if (signature !== previous) {console.log(signature); previous = signature;}
    if (!answers[index]) throw new Error('Question deck changed; update expected key sequences');
    if (question.includes('AnswerCopied={}') || progress >= answers[index].length) {input('up:49', 'wait:0.2'); continue;}
    if ([1, 2, 3].includes(attention) || attention === 4 && field(teacher, 'TeacherAttentionTimeLeft') < 0.4) {
      if (field(duck, 'DuckStateComponent') === 0 && index >= 5) input('up:49', ...tap(12));
      else input('up:49', 'wait:0.2');
      continue;
    }
    const side = field(question, 'AnswerNeighbourSide');
    const paw = lines.find(x => x.includes('Neighbour={}') && field(x, 'NeighbourSideComponent') === side);
    if (!paw?.includes('PawLifted={}')) {input('up:49', ...tap(46)); continue;}
    input('down:49', ...tap(answers[index][progress]));
  }
  if (Date.now() >= deadline) throw new Error('Player exceeded 125 seconds');
} finally {
  input('up:49', 'wait:0.2');
}
