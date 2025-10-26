#!/usr/bin/env bash
# https://typicode.github.io/husky/get-started.html

bun add --no-progress --dev husky
bun pm pkg set scripts.prepare="husky"
