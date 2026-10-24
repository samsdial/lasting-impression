# Cómo aportar a Guardianes del Humedal (Etapa 3)

El juego se construye **todo por código**. Ustedes no necesitan armar escenas en Unity: solo entregan **archivos** (imágenes, audios, textos) con el **nombre exacto** de las tablas de abajo, y el juego los carga solo.

## 1. Flujo de trabajo

1. Pide a Sergio que te agregue como colaborador del repo (acepta la invitación que llega al correo).
2. En GitHub, crea tu rama desde `main` con este formato: `arte-tunombre`, `audio-tunombre` o `docs-tunombre`.
3. Sube tus archivos a la carpeta indicada. Se puede hacer desde la web: entra a la carpeta → **Add file → Upload files** → elige **tu rama** → **Commit changes**.
4. Abre un **Pull Request** hacia `main` y llena la plantilla (checklist, declaración de IA, subtítulos).
5. Sergio revisa y hace el merge.

**Reglas:**
- Nunca subas cambios directo a `main`.
- No modifiques archivos `.unity`, `ProjectSettings/` ni los scripts de otros.
- Haz los commits con **tu propio usuario**: la rúbrica evalúa los commits de cada integrante.

## 2. Arte → `guardianesDelHumedal/Assets/Resources/Sprites/`

Formato: **PNG con fondo transparente**. Un tile = **32 × 32 px**.

| Archivo | Qué es | Tamaño |
|---|---|---|
| `player_idle.png` | Guardián quieto | 32×32 |
| `player_walk_1.png`, `player_walk_2.png` | Guardián caminando (2 cuadros) | 32×32 |
| `player_jump.png` | Guardián saltando | 32×32 |
| `player_hurt.png` | Guardián recibiendo daño | 32×32 |
| `ground_top.png` / `ground_fill.png` | Borde con pasto y tierra, versión **gris (contaminada)** | 32×32 |
| `ground_top_restored.png` / `ground_fill_restored.png` | Los mismos tiles en versión **verde (restaurada)** | 32×32 |
| `water_polluted.png` | Agua contaminada (con residuos visibles) | 32×32 |
| `gorse.png` | Retamo espinoso | 32×32 |
| `trash.png` | Basura recolectable | 32×32 |
| `seed.png` | Semilla nativa (**forma distinta** a la basura, no solo color) | 32×32 |
| `finish.png` | Meta / punto de restauración | 32×32 |
| `species.png` | Especie rescatada definida en el GDD | 32×32 o 64×64 |
| `background_polluted.png` / `background_restored.png` | Fondo gris y fondo verde | 640×360 |

**Accesibilidad:** lo peligroso (agua, retamo) y lo que se recoge (basura, semilla) se debe poder distinguir **por forma**, aunque se vea en blanco y negro.

## 3. Audio → `guardianesDelHumedal/Assets/Resources/Audio/`

Formato: **OGG o WAV**. Los efectos deben durar menos de 2 segundos y no pueden sonar saturados.

| Archivo | Cuándo suena | Subtítulo sugerido |
|---|---|---|
| `music_tension.ogg` | Música del nivel contaminado (en loop) | — |
| `music_calm.ogg` | Música cuando el sector se restaura | — |
| `sfx_trash.wav` | Recoger basura | [Basura recogida] |
| `sfx_seed.wav` | Recoger semilla | [Semilla recogida] |
| `sfx_damage.wav` | Tocar el retamo | [Espinas: pierdes una gota] |
| `sfx_water.wav` | Caer al agua contaminada | [Agua contaminada] |
| `sfx_restore.wav` | El sector pasa de gris a verde | [El humedal se restaura] |
| `sfx_species.wav` | Canto o sonido de la especie rescatada | [Canto de la especie rescatada] |

Si el audio no es tuyo ni generado por IA, indica en el PR la **fuente y la licencia** (por ejemplo CC0 de freesound.org).

## 4. Si usas Inteligencia Artificial (obligatorio documentarlo)

Por cada archivo generado o asistido con IA, completa esto en el PR:

| Archivo | Herramienta y versión | Prompt (**en inglés**) | Qué modificaste a mano |
|---|---|---|---|
| `trash.png` | Leonardo.ai (Phoenix 1.0) | "pixel art plastic bottle, 32x32, transparent background, muted colors" | Recorté a 32×32, bajé la saturación, limpié el contorno |

La guía exige que el resultado se **posprocese**: no se puede subir tal cual sale de la IA. Guarda una captura del resultado original para el documento técnico.

## 5. Documentación → `docs/`

- Reporte de accesibilidad: `docs/accesibilidad.md`
- Notas de la sesión de crítica: `docs/critica.md`
- Cada integrante también escribe su sección del documento técnico (qué hizo, decisiones y aprendizajes) en `docs/aportes/tunombre.md`.
