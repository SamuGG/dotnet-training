#!/usr/bin/env bash
# https://commitlint.js.org/

bun add --no-progress --dev @commitlint/cli @commitlint/config-conventional conventional-changelog-conventionalcommits

echo "make lint-commit-msg GIT_COMMIT_EDITMSG_FILE=\$1" > .husky/commit-msg
git add .husky/commit-msg
