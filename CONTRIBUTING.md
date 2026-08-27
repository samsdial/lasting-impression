# Contributing to `lasting-impression`

This document explains how each team member contributes to the repository. The workflow is designed to satisfy Stage 1's rubric criteria: individual commits, ordered version flow, and English usage in labels and descriptions.

## Prerequisites

1. A GitHub account.
2. Ask the repo owner (@samsdial) to add you as a collaborator.
3. Git installed locally (`git --version` to verify) — or GitHub Desktop if you prefer a UI.

## One-time setup

```bash
git clone https://github.com/samsdial/lasting-impression.git
cd lasting-impression
```

## Workflow for every contribution

### 1. Sync `main`

Always start from an up-to-date `main`:

```bash
git checkout main
git pull origin main
```

### 2. Create your branch

Use your first name in lowercase, no accents or spaces:

```bash
git checkout -b [firstname]
```

### 3. Create your folder and content

Inside the repo root, create a folder named after your first name and add:

- `plato-favorito.jpg` — an image of your favorite dish
- Any other personal asset you want to include

Then add your profile picture to `assets/profiles/[firstname].jpg` and update your section in the root `README.md` (replace the corresponding `[BRACKETS]` placeholders with your real information).

### 4. Commit in English

Small, descriptive commits. **Always in English** — the rubric requires it.

Convention:

- `feat:` — new content (folder, section, asset)
- `docs:` — documentation changes
- `fix:` — corrections

Examples:

```bash
git add [firstname]/
git commit -m "feat: add [firstname] folder with favorite dish image"

git add README.md assets/profiles/[firstname].jpg
git commit -m "docs: add [firstname] profile section and picture"
```

### 5. Push your branch

```bash
git push origin [firstname]
```

### 6. Open a Pull Request

1. Go to https://github.com/samsdial/lasting-impression
2. Click "Compare & pull request" on the yellow banner.
3. Title (in English), e.g. `Add [First Name] profile and folder`
4. Description (in English): summarize what you added.
5. Assign @samsdial as reviewer.
6. Create the PR.

### 7. Wait for merge

The repo owner reviews and merges into `main`. **Do not merge your own PR** — this keeps the version flow ordered and auditable, which the rubric rewards.

## Ground rules

- Never commit directly to `main`.
- Before each work session, pull `main` and rebase or merge into your branch to stay current.
- Every commit and PR must be in English.
- Each teammate must push their own commits from their own GitHub account — the rubric verifies individual participation.

## Tag `v1.0`

Once all three PRs are merged into `main`, the repo owner creates the release tag:

```bash
git checkout main
git pull origin main
git tag -a v1.0 -m "Stage 1 delivery: team formation and VCS setup"
git push origin v1.0
```

The tag URL is what gets submitted in the course's evaluation form.
