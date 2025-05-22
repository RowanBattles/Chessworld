k6 run ./k6/matchmaking.js --summary-export=summary.json

if [ ! -f summary.json ]; then
  echo "Error: summary.json not found. k6 test might have failed."
  exit 1
fi

iterations=$(jq '.metrics.iterations.count' summary.json)
failedRequests=$(jq '.metrics.http_req_failed.passes' summary.json)

if [ -z "$iterations" ]; then
  echo "Error: Failed to extract iterations from summary.json."
  exit 1
fi

if [ -z "$failedRequests" ]; then
  echo "Error: Failed to extract failed http_requests from summary.json."
  exit 1
fi

node ./validation/validation.js "$iterations" "$failedRequests"