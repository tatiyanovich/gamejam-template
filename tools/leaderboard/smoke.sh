#!/usr/bin/env bash
# Smoke test for the deployed COPYCAT leaderboard web app.
# Usage: ./smoke.sh "https://script.google.com/macros/s/<deployment-id>/exec"
set -euo pipefail

URL="${1:?usage: smoke.sh <apps-script-exec-url>}"

# Apps Script answers with a 302 to script.googleusercontent.com; curl -L must NOT keep the
# method on redirect (do not pass -X POST) — the redirected GET returns the JSON body.
echo "== GET top 10 =="
curl -sSL "${URL}?top=10"
echo

echo "== POST a run =="
curl -sSL -H 'Content-Type: application/json' \
  -d '{"name":"smoke test","answers":7,"timeSeconds":83.5,"grade":"C"}' "$URL"
echo

echo "== GET rank of that run =="
curl -sSLG --data-urlencode 'name=smoke test' --data-urlencode 'answers=7' \
  --data-urlencode 'timeSeconds=83.5' --data-urlencode 'top=10' "$URL"
echo

echo "== garbage input is sanitized, not fatal =="
curl -sSL -H 'Content-Type: application/json' \
  -d '{"name":"🐱🐱🐱","answers":999,"timeSeconds":-5,"grade":"Z9"}' "$URL"
echo
echo "Expect: JSON every time, no HTML. Delete the smoke rows from the sheet afterwards."
