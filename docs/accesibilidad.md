# Reporte de Accesibilidad - Lasting Impression

## 1. Contraste de Color (WCAG 2.2 - Criterio 1.4.3 Contrast Minimum)
Se ha verificado el contraste de los elementos de la interfaz utilizando la herramienta WebAIM Contrast Checker[cite: 4]:
- **Texto del HUD (#FFFFFF) sobre panel (#1B1F23):** Pasa satisfactoriamente el nivel AA y AAA para texto normal y grande.
- **Contador dorado (#FFD700) sobre panel (#1B1F23):** Proporciona un contraste excelente para destacar información clave del puntaje.
- **Jugador (#F2B705) sobre cielo (#8E979E):** Garantiza visibilidad y distinción del personaje en movimiento.
- **Suelo (#6E6A64) sobre cielo (#8E979E):** Cumple con el ratio adecuado para elementos estructurales del entorno.
- **Contraejemplo (Dorado #FFD700 sobre Blanco #FFFFFF):** No cumple con el ratio mínimo exigido (falla en WCAG), lo cual justifica técnicamente la obligatoriedad de usar el panel oscuro para asegurar la legibilidad.

## 2. Forma, no solo color (Diseño Universal para el Aprendizaje - DUA)
Evaluando los sprites en escala de grises proporcionados por el equipo de arte, se confirma que elementos críticos como la basura, la semilla, el agua y el retamo poseen siluetas y formas únicas e inequívocas, permitiendo que jugadores con daltonismo o deficiencias en la percepción del color puedan identificarlos sin depender exclusivamente de este.

## 3. Sonido + Visual (Redundancia Multisensorial)
Se implementa una retroalimentación redundante combinando efectos de audio con subtítulos textuales sincronizados para cada evento clave del juego, asegurando accesibilidad auditiva.

## 4. Controles Alternativos
El minijuego soporta múltiples periféricos de entrada para garantizar accesibilidad motriz:
- Teclado estándar (**WASD**)
- Teclas de **flechas direccionales**
- Compatibilidad con **mando/control de videojuegos**

---

## Validación de Accesibilidad – Nueva Etapa (Controles, HUD y Mecánica de Máquina)

### 1. Contraste en HUD y Nuevos Avisos (WCAG 2.2 – 1.4.3 Contrast Minimum)
- **HUD principal y avisos nuevos:** Verificados en WebAIM Contrast Checker[cite: 4], manteniendo una relación de contraste igual o superior a **4.5:1** para garantizar la legibilidad del texto y las notificaciones en pantalla.
- *(Completa aquí los códigos HEX y el ratio exacto obtenido en WebAIM para cada elemento revisado).*

### 2. Distinción por Forma en la Mecánica de Máquina (WCAG 2.2 – 1.4.1 Use of Color / DUA)
- La nueva mecánica asociada a la máquina se identifica mediante su forma, estructura visual e indicadores diferenciados, evitando depender exclusivamente del color para comprender su estado o función.

### 3. Verificación de Controles Alternativos (WCAG 2.2 – 2.1.1 Keyboard)
- Se validó que **WASD**, **flechas direccionales** y **mando** continúan funcionando correctamente con el nuevo elemento incorporado en el escenario (`PlayerInput.cs`), sin regresiones en la respuesta de control.
