# Review checklist for teammates' Pull Requests

How to use it with Claude Code:

```
Review PR #<N> following docs/REVIEW_CHECKLIST.md. Use `gh pr view <N>` and `gh pr diff <N> --name-only`.
Report each item as OK / FAIL with the fix the author must make. Do not merge or commit.
```

## 1. Git and structure
- [ ] Source branch is the author's own branch (not `main`), and the commits are under the author's GitHub user.
- [ ] Files are only in the allowed folders: `guardianesDelHumedal/Assets/Resources/Sprites/`, `.../Resources/Audio/` or `docs/`.
- [ ] No changes to `.unity`, `ProjectSettings/`, `Packages/` or other people's scripts.
- [ ] No `Library/`, `Temp/`, `Logs/` or `.vs/` files.

## 2. Art
- [ ] File names match the table in `docs/GUIA-APORTES-ETAPA3.md` exactly (lowercase, `.png`).
- [ ] PNG with a transparent background, 32×32 per tile (backgrounds 640×360).
- [ ] Trash vs seed, and hazards vs collectibles, are distinguishable by shape (check in grayscale).
- [ ] Gray and green versions keep the same silhouette (only the palette changes).

## 3. Audio
- [ ] Names match `docs/GUIA-APORTES-ETAPA3.md`; format OGG or WAV.
- [ ] Effects shorter than 2 s; no clipping; similar loudness between files.
- [ ] Music loops without a noticeable cut.
- [ ] Every important sound has a subtitle in the PR table.
- [ ] Source and license stated if not original.

## 4. AI usage declaration (graded requirement)
- [ ] Every AI asset has: tool and version, prompt **in English**, and manual modifications.
- [ ] The manual modification is real post-processing (crop, palette, cleanup, editing), not "none".

## 5. Code (only if the PR includes scripts)
- [ ] Identifiers and comments in English; functions, conditions and loops are commented.
- [ ] Follows `CLAUDE.md` rules (built from code, colors in `GameConfig`, namespaces).
- [ ] Compiles with no errors in the Unity Console.

## 6. After merging (Sergio)
1. `git checkout main && git pull`
2. Open Unity and wait for the import (the import processor sets sprite and audio settings automatically).
3. Press Play and confirm the new assets replace the placeholders.
4. Commit the `.meta` files Unity generated (assets uploaded from the web have none):
   `git add guardianesDelHumedal/Assets && git commit -m "chore: add meta files for imported assets"` → `git push`
