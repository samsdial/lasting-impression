# Guardians of the Wetland (Guardianes del Humedal)

2D platformer minigame about the wetlands of the Sabana de Bogotá (environmental education).
Course: UNAD 213027A – Programación para Videojuegos, Etapa 3 (art and audio implementation).
Team: Lasting Impression. Repo: samsdial/lasting-impression.

## Stack
- Unity 2D (compatible with Unity 2022.3 LTS and Unity 6), C#.
- Unity project lives in `guardianesDelHumedal/`. Folders from earlier stages (member folders, root README) must not be touched.
- Unity Test Framework for EditMode tests.

## Project rules
- **The scene is built 100% from code.** `GameBootstrap` creates everything at Play via `[RuntimeInitializeOnLoadMethod]`. Never require manual scene or Inspector setup. Never edit `.unity` scene files.
- **All identifiers and comments in English** (mandatory grading condition). Comments explain functions, conditions and loops. Descriptive names, classes, clean indentation.
- Namespaces: `GuardiansOfTheWetland.Core`, `.Level`, `.Player`, `.Gameplay`, `.UI`, `.Audio`, `.EditorTools`.
- Art is loaded by name from `Assets/Resources/Sprites/` through `SpriteFactory.Get(name, placeholderColor)`; a colored placeholder is used while the file is missing. Audio is loaded from `Assets/Resources/Audio/`. File names are defined in `docs/GUIA-APORTES-ETAPA3.md`.
- Import settings for sprites and audio are applied automatically by `GameAssetImportProcessor` (Editor script). Sprites: 32 pixels per unit, point filter.
- All colors live in `GameConfig`. HUD text always sits on a dark panel (contrast ≥ 4.5:1, WCAG 2.2 AA).
- Accessibility (WCAG 2.2 / UDL): every important event has visual AND audio feedback; important sounds have subtitles; collectibles differ by shape, not only color; controls: WASD, arrow keys and gamepad.
- Keep Unity 2022/6 compatibility (`#if UNITY_6000_0_OR_NEWER` for `linearVelocity`) and support both input systems (`ENABLE_INPUT_SYSTEM`).
- The level layout is the text map in `LevelData`; new tile types are new symbols handled in `LevelBuilder`.

## Testing
- Only pure logic gets automated tests (counters, health, map parsing, state changes) in `Assets/Tests/EditMode/`. Visual and audio criteria are **manual verification**.
- Game code needs an assembly definition so the test assembly can reference it (add `Unity.InputSystem` to its references if the Input System package is installed).
- Unity locks the project while it is open, so the human runs tests in the Test Runner window and reports red/green. Ask for the result; do not assume it.

## Workflow
- hola-ai skills: `/backlog` → `/story` → `/pair`. One user story (HU) per branch: `hu-NN-short-name`.
- The AI never commits; it suggests the commit message.
- Teammates contribute art, audio and docs through Pull Requests following `docs/GUIA-APORTES-ETAPA3.md`. Reviews follow `docs/REVIEW_CHECKLIST.md`.
- Final delivery: everything merged to `main`, README updated, annotated tag `v2.0`.
