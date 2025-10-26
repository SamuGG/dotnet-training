#!/usr/bin/env bash
set -euo pipefail

REPO_SRC=$(jq -r '.repository.url' package.json)
REPO_DIR=$(mktemp -d)

echo "🔗 Cloning repository from $REPO_SRC to $REPO_DIR..."
rm -rf "$REPO_DIR"
git clone --depth=1 "git@github.com:SamuGG/repository-template.git" "$REPO_DIR"

echo "🔧 Copying files..."
cp "$REPO_DIR"/dirs.proj "$REPO_DIR"/Directory.{Build,Packages}.* .
cp "$REPO_DIR"/backend/src/Directory.Build.* backend/src/
cp "$REPO_DIR"/backend/test/Directory.*.props backend/test/
cp "$REPO_DIR"/bunfig.toml .
cp "$REPO_DIR"/Makefile .
cp "$REPO_DIR"/package.json .
cp -r "$REPO_DIR"/dep/* dep/
cp -r "$REPO_DIR"/.config/* .config/

echo "🗑️ Cleaning temp files..."
rm -rf "$REPO_DIR"

echo "✔ Done"
