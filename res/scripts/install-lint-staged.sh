#!/usr/bin/env bash
# https://github.com/okonet/lint-staged

bun add --no-progress --dev lint-staged

echo "bun lint-staged --config .config/.lintstagedrc" > .husky/pre-commit
echo "make lint-commit-msg GIT_COMMIT_EDITMSG_FILE=\$1" > .husky/commit-msg
git add .husky/pre-commit
