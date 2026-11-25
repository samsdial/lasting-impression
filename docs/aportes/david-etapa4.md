# Stage 4 Contribution Report: Audio

**Project:** Guardianes del Humedal
**Author:** David Arana
**Role:** Sound design (audio)
**Branch:** `audio-david`
**Date:** 2026-11-25

---

## 1. Summary

My work in this stage covers the audio of the game and its accessibility. I created the sound effect for the new mechanic, documented the AI tools used to generate it, and kept every audio cue paired with a subtitle so that no important information depends on hearing alone.

## 2. Audio assets

All files are in `guardianesDelHumedal/Assets/Resources/Audio/`.

| File | Plays when | Subtitle |
|---|---|---|
| `music_tension.ogg` | Polluted level music (loop) | [Música ambiente (inquieta, oscura)] |
| `music_calm.ogg` | Music when a sector is restored | [Música ambiente (tranquila, natural)] |
| `sfx_trash.wav` | Picking up trash | [Basura recogida] |
| `sfx_seed.wav` | Picking up a seed | [Semilla recogida] |
| `sfx_damage.wav` | Touching the gorse (retamo) | [Espinas: pierdes una gota] |
| `sfx_water.wav` | Falling into polluted water | [Agua contaminada] |
| `sfx_restore.wav` | A sector turns from gray to green | [Humedal restaurado] |
| `sfx_species.wav` | Call of the rescued species (Bogota rail) | [Canto Tingua] |
| `sfx_platform.wav` | Stepping onto the moving platform (new mechanic) | [Plataforma en movimiento] |

> Note: the new mechanic is a moving platform. If the team decides it is an enemy instead, `sfx_platform.wav` will be replaced by `sfx_enemy.wav` with its own subtitle.

## 3. Technical decisions

- **Formats:** music in `.ogg` (looping, smaller size); sound effects in `.wav`.
- **Duration:** every sound effect is under 2 seconds, as agreed by the team.
- **Levels:** both music tracks were forced through a limiter at -6 dB to avoid sudden peaks. [Sound effects: describe any level adjustment, or write "none".]
- **Editing:** [describe trimming, fades and loop points you made, and the tool you used, e.g. Audacity].
- **Integration:** [describe how the sounds are triggered in Unity, e.g. which script or `AudioSource` plays each one].

## 4. Accessibility

- Every sound effect that carries gameplay information has a subtitle in brackets, so players who cannot hear it still get the information.
- Sound reinforces the visuals and does not replace them: the music changes when a sector is restored, at the same moment the sector turns from gray to green.
- Volumes were balanced so that effects do not hide the music and no sound is abruptly loud.
- The call of the rescued species is soft by nature, so its volume was set slightly above the calm music to keep it audible. It also has a subtitle.
- Criteria followed: WCAG 2.2 and Universal Design for Learning (redundant visual and audio feedback).

## 5. AI use declaration

**Tools:** Suno (music) and ElevenLabs (sound effects). Version or model: [Suno version], [ElevenLabs model].

| Asset | Tool | Prompt used | Result | Manual changes |
|---|---|---|---|---|
| `music_tension.ogg` | Suno | *Looping instrumental game soundtrack, slow tempo around 80 BPM. Dark, uneasy atmosphere of a polluted swamp: low drone pads, sparse muted marimba pulses, distant metallic scrapes, subtle dissonant strings. Moderate tension, steady rhythm that works for platformer jumping, no vocals, seamless loop.* | Instrumental loop, saved as `music_tension.ogg` | Limiter forced to -6 dB |
| `music_calm.ogg` | Suno | *Looping instrumental game soundtrack, gentle tempo around 70 BPM. Hopeful and peaceful atmosphere of a recovering wetland: warm soft piano, light acoustic guitar plucks, airy flute, soft strings, faint nature ambience. Calm and uplifting, gradually brightening, no vocals, seamless loop.* | Instrumental loop, saved as `music_calm.ogg` | Limiter forced to -6 dB |
| `sfx_trash.wav` | ElevenLabs | *Short 0.5 second sound effect, collecting a piece of trash in a 2D game: crinkly plastic and metal can rustle followed by a quick soft "pop" pickup chime. Clean, no background noise, no music.* | Saved as `sfx_trash.wav` | [none / describe] |
| `sfx_seed.wav` | ElevenLabs | *Short 0.6 second sound effect, collecting a seed in a 2D game: tiny soft wooden "tick" plus a gentle rising sparkle of two notes, natural and warm. Clean, no background noise, no music.* | Saved as `sfx_seed.wav` | [none / describe] |
| `sfx_damage.wav` | ElevenLabs | *Short 0.5 second sound effect, character pricked by thorny bushes in a 2D game: sharp scratchy thorn snap with a short low "ouch" thud, quick and clear, not too loud or harsh. No music.* | Saved as `sfx_damage.wav` | [none / describe] |
| `sfx_water.wav` | ElevenLabs | *1 second sound effect, character falling into thick polluted water: heavy low splash, murky gurgle and a sticky bubbling tail, deep and final-sounding. No music.* | Saved as `sfx_water.wav` | [none / describe] |
| `sfx_restore.wav` | ElevenLabs | *2 second sound effect, a wetland being restored in a 2D game: a soft magical whoosh that blooms into bright shimmering chimes, a rush of clean flowing water and a few light bird chirps at the end. Uplifting and rewarding. No music.* | Saved as `sfx_restore.wav` | [none / describe] |
| `sfx_species.wav` | ElevenLabs | *1.5 second sound effect, the call of a Bogota rail (Rallus semiplumbeus), a small marsh bird: a short, high-pitched call, brief and quiet, not loud or harsh, repeated once or twice with a pause between. Soft, natural wildlife recording in a calm wetland, very light reeds rustling and gentle water ambience far in the background. Realistic, no music.* | Saved as `sfx_species.wav` | [none / describe] |
| `sfx_platform.wav` | ElevenLabs | *Short 0.6 second sound effect, a single wooden creak when a character steps onto a moving platform in a 2D game: an old wooden plank flexing under weight, a brief low-pitched "creeeak" with a small soft thud at the start. Dry and clean, subtle swamp-wood feel, no echo, no background noise, no music.* | Saved as `sfx_platform.wav` | [none / describe] |

Prompts are in English, as required. If a real recording from Xeno-canto was used for the species call, cite the recordist, the XC number and the license here.

## 6. Git workflow

- Work was done on branch `audio-david`, with commits identifiable as mine.
- A pull request to `main` was opened with the subtitle table filled in: [PR link or number].

## 7. Lessons learned

[Write 3 or 4 sentences in your own words: what was hardest about the audio, what you would change, and what you learned.]
