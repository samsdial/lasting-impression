# Aporte de Mariana — Diseño del nivel (Nivel como mapa de texto)

> **Rama:** Nivel de Guardianes del Humedal
> **Archivo editado:** `guardianesDelHumedal/Assets/Scripts/Level/LevelData.cs`
> **Enunciado (objetivo):** diseñar el nivel como mapa de texto según las métricas de jugabilidad.

---

## ¿Qué hice?

Diseñé el mapa del nivel en `LevelData.cs` en forma de **mapa de texto** en el que cada carácter representa un bloque. El nivel se construye por código desde `LevelBuilder`, así que **no hace falta armar la escena manualmente** en Unity: solo se cambia la cuadrícula de texto y el juego la convierte en plataformas, suelo, coleccionables y meta.

### Leyenda usada en el mapa

| Símbolo | Significado |
|---|---|
| `.` | Vacío |
| `#` | Suelo o plataforma (sólido) |
| `P` | Punto de inicio del jugador (uno solo) |
| `T` | Basura recolectable (8 en total) |
| `S` | Semilla nativa (5 en total) |
| `F` | Meta / punto de restauración (una sola) |

*(El agua contaminada `~` y el retamo `R` quedan listos para el bloque 2; el mapa ya puede incluir sus posiciones porque el constructor los ignora hasta implementarlos.)*

### Mapa

```
............................................................
.....................................................T.SF...
.................................................T..########
.........................................ST...####..########
...................................T....####..####..........
...........................S.....####...####................
......................T...####...####.......................
...........S........####..####..............................
.......S..###.......####....................................
...P.T###T###....T..........................................
############....##############....##############....########
```

---

## 📊 Tabla de métricas (decisión de diseño)

Estos valores salen de la física del jugador en `PlayerController.cs` y definen el diseño del nivel.

| Métrica | Valor del enunciado | Valor implementado |
|---|---|---|
| Filas del nivel | 10 a 12 filas | **11 filas** |
| Largo de cada fila | Todas iguales | **60 columnas** (todas idénticas) |
| Altura máxima del salto | ~3 bloques | `jumpHeight = 3.2` → plataformas suben **máx. 3 bloques** |
| Largo del salto | ~4–5 bloques | Hueco horizontal máximo de **4 bloques** |
| Basuras (`T`) | 8 | **8** |
| Semillas (`S`) | 5 | **5** |
| Inicio (`P`) | 1 | **1** |
| Meta (`F`) | 1 | **1** |
| Dificultad creciente | De izquierda a derecha | **Sí**: plataformas bajas y anchas cerca del inicio, más altas y cortas hacia la meta |

### Cómo se cumplió cada regla

- **Plataformas a máximo 3 bloques:** cada plataforma del recorrido sube como máximo 1–3 bloques respecto a la anterior, dentro del alcance del salto (`jumpHeight = 3.2`).
- **Huecos de máximo 4:** los fosos entre secciones de suelo miden hasta 4 bloques, justo en el límite del largo de salto (4–5).
- **Todas las filas del mismo largo, 10–12 filas:** 11 filas × 60 columnas, sin filas desparejas (evita bordes rotos en el `LevelBuilder`).
- **Dificultad creciente:** el tramo inicial es suelo amplio con plataformas bajas; hacia el final las plataformas se vuelven más altas y cortas, exigiendo saltos más precisos, hasta llegar a la meta en la meseta superior derecha.
- **Coleccionables distribuidos por el recorrido:** 8 basuras y 5 semillas repartidas a lo largo del camino, de modo que se recogen mientras se avanza hacia la meta.

> Esta tabla va como **decisión de diseño** dentro de la sección "Decisiones técnicas" del documento técnico.

---

## Verificación

- El mapa se **validó por reglas** (conteos exactos, filas del mismo largo, unicidad de `P` y `F`, alcance del salto y huecos ≤ 4).
- La **meta es alcanzable** desde el punto de inicio siguiendo plataformas dentro del rango del salto.
- Todo coleccionable queda **sobre una superficie alcanzable** del recorrido.
- **Pendiente de probar en Unity (manual):** dar Play y afinar el gameplay de los saltos. Las basuras, el agua y demás se activan cuando esos bloques estén implementados, pero ya es posible probar los saltos.

---

## Stage 4 contribution — Integration of the moving-platform level update

**Branch / file:** `guardianesDelHumedal/Assets/Scripts/Level/LevelData.cs`
**Assignment goal:** incorporate the Stage 4 "machine mechanic" (moving platform) into the level map, on top of the design delivered in Stage 3.

### What I actually did

I reviewed the level-map update proposed for the Stage 4 machine mechanic — a new map
symbol (`M`) representing a moving platform that travels across one of the polluted-water
gaps — and integrated it into the project's `LevelData.cs`, together with the updated
legend and design-decision comments that document where the platform sits and why.

I am stating plainly, for accuracy, that **the placement of the platform (row 9, column 47)
and the written rationale for it were produced with AI assistance** (Claude), as part of a
set of Stage 4 scripts requested by a teammate on behalf of the whole group. My own
contribution at this point is the review and integration of that change into the
repository's `LevelData.cs`, not the original design of the placement itself. This is
recorded for transparency in `docs/declaracion-ia-etapa4.md`.

### What the updated map changes

- Adds the symbol `M` to the legend, for a moving platform that is not controlled by the
  player.
- Places `M` in row 9 (the row with the ground and hazards), column 47, right before the
  third and last polluted-water gap (columns 48-51).
- Keeps every other row of the Stage 3 map unchanged: still 11 rows of 60 characters each,
  the same counts of trash (8), seeds (5), spawn (1) and finish (1).
- The water gap it crosses is still 4 tiles wide, so it remains jumpable on its own — the
  platform is an additional way across the same obstacle, not the only way.

### What is still pending (not yet done by me)

- **Play-testing in the Unity Editor.** I have not yet opened Play mode to confirm that the
  platform appears, that the player can stand on it, or that it actually carries the player
  across the gap. This has to happen before the mechanic can be marked as verified.
- **Confirming the design reasoning against the jump metrics.** The Stage 3 table above
  (jump height 3.2 tiles, gaps ≤ 4 tiles) already covers the rest of the level; I still need
  to confirm, by testing, that the platform's speed and travel distance (set in the proposed
  `MovingPlatform.cs`) feel right in practice and not just on paper.
- **Any further adjustment** to the platform's position, speed or travel distance, if testing
  shows it needs one, will be documented here once done.

I am leaving this section separate from the Stage 3 "Verificación" above, because that one
describes rules that were actually checked for the Stage 3 map, and I do not want to imply
the same level of verification applies to this new element yet.
