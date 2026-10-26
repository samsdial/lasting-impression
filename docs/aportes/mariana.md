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
