# 🗡️ SHOGUN'S LEGACY - ROADMAP DE DESARROLLO

**Roguelike de acción con ambientación de fantasía oscura feudal japonesa**
**Engine:** Unity 2022 LTS
**Plataformas:** PC/Mac (Steam) → Mobile → Switch
**Duración estimada:** 10-11 meses (full-time)

---

## 📊 PROGRESO GENERAL

- [x] **STAGE 0:** Setup y Preparación (1-2 semanas) ✅ **COMPLETADO**
- [ ] **STAGE 1:** MVP Core - Combate Básico (3-4 semanas) ⏳ **EN PROGRESO**
- [ ] **STAGE 2:** Sistemas de Combate Ampliados (3-4 semanas)
- [ ] **STAGE 3:** Generación Procedural Básica (4-5 semanas)
- [ ] **STAGE 4:** Sistemas de Loot y Build Variety (3-4 semanas)
- [ ] **STAGE 5:** Meta-progresión y Hub (4-5 semanas)
- [ ] **STAGE 6:** Jefes y Biomas (5-6 semanas)
- [ ] **STAGE 7:** Polish, Audio y Juice (4-5 semanas)
- [ ] **STAGE 8:** Contenido Late-game y Replayability (3-4 semanas)
- [ ] **STAGE 9:** Optimización y Bug Fixing (3-4 semanas)
- [ ] **STAGE 10:** Lanzamiento PC/Mac (2-3 semanas)

**Última actualización:** 2025-11-12

---

## 📋 STAGE 0: SETUP Y PREPARACIÓN
**Duración:** 1-2 semanas
**Objetivo:** Configurar el entorno de desarrollo y generar assets base con IA

### Tareas de Programación

- [x] **0.1 Configuración Unity** ✅
  - [x] Crear proyecto Unity 2022 LTS (2D URP) - Documentado en SETUP.md
  - [x] Configurar Input System (New Input System) - Documentado
  - [x] Instalar paquetes: Cinemachine, 2D Animation, Shader Graph, TextMeshPro - Guía en configure-unity-packages.md
  - [x] Configurar resoluciones objetivo (1920x1080 base) - Configurado en scripts
  - [x] Setup de capas (Player, Enemy, Projectile, Ground, etc.) - Documentado en SETUP.md

- [x] **0.2 Estructura de carpetas y arquitectura** ✅
  - [x] Crear estructura MVC o sistema modular - Scripts base creados
  - [x] Carpetas: Scripts/{Player, Enemy, Combat, Generation, UI, Audio, Managers} - setup-unity-project.sh
  - [x] Carpetas: Assets/{Sprites, Animations, Prefabs, Audio, Shaders} - setup-unity-project.sh
  - [x] Configurar Git/control de versiones (.gitignore para Unity) - .gitignore creado

### Assets IA necesarios

- [ ] **0.3 Generar Player Sprite Sheet**
  - [ ] Prompt: "pixel art samurai character sprite sheet, 32x32 pixels, 8-directional movement, idle/walk/attack animations, dark red hakama, katana, feudal japan, dark fantasy style, black background"
  - [ ] Herramienta: Stable Diffusion + pixel art LoRA, o MidJourney + Aseprite
  - [ ] Necesitas: idle (4 frames), walk (8 frames x 8 direcciones), attack (6 frames x 8 direcciones), death (5 frames)

- [ ] **0.4 Generar Enemy Base (Oni simple)**
  - [ ] Prompt: "pixel art oni demon sprite sheet, 32x32 pixels, red skin, horns, japanese demon, idle and attack animation, dark fantasy, black background"
  - [ ] Necesitas: 1 enemigo básico melee para MVP

- [ ] **0.5 Generar Tileset entorno básico**
  - [ ] Prompt: "pixel art tileset, japanese temple floor tiles, stone ground, 16x16 pixels, dark atmosphere, seamless pattern"
  - [ ] Necesitas: 10-15 tiles básicos (suelo, paredes, esquinas)

- [ ] **0.6 Generar VFX básicos**
  - [ ] Prompt: "pixel art effect sprite sheet, sword slash effect, sparks, hit impact, 16x16 to 32x32, transparent background"
  - [ ] Necesitas: slash (3 frames), hit spark (4 frames), sangre (3 frames)

### ✅ Testeable al final del Stage 0:
- [ ] Proyecto Unity abierto y configurado
- [ ] Todos los sprites base importados en Unity y cortados (Sprite Editor)
- [ ] 1 escena vacía con tilemap del suelo visible

---

## 🎮 STAGE 1: MVP CORE - COMBATE BÁSICO
**Duración:** 3-4 semanas
**Objetivo:** Jugador controlable + 1 enemigo + combate funcional en 1 habitación

### Tareas de Programación

- [ ] **1.1 Movimiento del Jugador**
  - [ ] PlayerController con Input System (WASD + mouse aim)
  - [ ] Movimiento 8 direcciones con Rigidbody2D
  - [ ] Animaciones según dirección de movimiento
  - [ ] Flip sprite según dirección del ratón

- [ ] **1.2 Sistema de Combate Base - Melee**
  - [ ] Ataque con click izquierdo (cooldown 0.5s)
  - [ ] Hitbox con Collider2D (trigger) que aparece durante frames de ataque
  - [ ] Sistema de daño: DamageDealer y IDamageable interface
  - [ ] Animación de ataque (6 frames, 0.3s duración)
  - [ ] VFX de slash spawneado en posición del ataque

- [ ] **1.3 Sistema de Salud**
  - [ ] HealthComponent genérico (HP actual, HP máximo)
  - [ ] Barra de vida UI con Image.fillAmount
  - [ ] Muerte: animación + Destroy del GameObject
  - [ ] Invulnerabilidad temporal (0.5s) al recibir daño (iframe)

- [ ] **1.4 IA Enemigo Básico (Oni Melee)**
  - [ ] Estado: Idle, Chase, Attack, Dead
  - [ ] Navegar hacia jugador cuando esté en rango (8 unidades)
  - [ ] Atacar cuando esté cerca (1.5 unidades)
  - [ ] Usar Pathfinding simple o NavMeshAgent 2D
  - [ ] Daño al jugador por contacto durante ataque

- [ ] **1.5 Habitación de prueba**
  - [ ] Tilemap 20x15 con paredes Collider2D
  - [ ] Spawn de 3 enemigos en posiciones fijas
  - [ ] Cámara Cinemachine siguiendo al jugador

### Assets IA adicionales necesarios

- [ ] **1.6 Refinar animaciones player**
  - [ ] Si las IA no son perfectas, usar herramienta de interpolación
  - [ ] Generar missing frames si hace falta

- [ ] **1.7 Generar UI básico**
  - [ ] Prompt: "pixel art UI health bar frame, japanese style, ornate borders, 200x30 pixels"
  - [ ] Barra de vida decorativa
  - [ ] Panel de pausa simple

### ✅ Testeable al final del Stage 1:
- [ ] Controlar samurái con WASD, apuntar con ratón
- [ ] Matar 3 onis con la katana
- [ ] Recibir daño y ver barra de vida bajar
- [ ] Morir y ver animación de muerte
- [ ] **GAMEPLAY LOOP MÍNIMO FUNCIONAL**

---

## 🔫 STAGE 2: SISTEMAS DE COMBATE AMPLIADOS
**Duración:** 3-4 semanas
**Objetivo:** Añadir profundidad al combate - proyectiles, combos, más enemigos

### Tareas de Programación

- [ ] **2.1 Sistema de Proyectiles**
  - [ ] ProjectileController genérico (velocidad, daño, lifetime)
  - [ ] Disparo de shuriken con click derecho
  - [ ] Pooling de proyectiles (ObjectPool para performance)
  - [ ] Colisión con enemigos y destrucción

- [ ] **2.2 Sistema de Combos**
  - [ ] Detectar 3 ataques consecutivos en 1.5s = combo
  - [ ] Tercer golpe hace más daño y knockback
  - [ ] VFX especial en combo finisher

- [ ] **2.3 Sistema de Parry/Block**
  - [ ] Tecla Shift = block (reduce daño 80%)
  - [ ] Si bloqueas justo antes de recibir hit (0.2s window) = parry perfecto
  - [ ] Parry perfecto: reflejar proyectiles, aturdir enemigo melee

- [ ] **2.4 Sistema de Posturas (Stances)**
  - [ ] ScriptableObject para cada postura (Water, Earth)
  - [ ] Water: +20% velocidad movimiento, ataques rápidos
  - [ ] Earth: -20% velocidad, +50% daño, super armor en ataques
  - [ ] Cambiar con tecla Q
  - [ ] UI indicador de postura activa

- [ ] **2.5 Enemigo a distancia (Oni Archer)**
  - [ ] IA: mantener distancia (5-8 unidades) del jugador
  - [ ] Disparar flechas cada 2s
  - [ ] Huir si jugador se acerca mucho (3 unidades)

- [ ] **2.6 Enemigo rápido (Yokai Runner)**
  - [ ] Dash rápido hacia el jugador
  - [ ] Ataque dash-through que atraviesa
  - [ ] HP bajo pero difícil de golpear

### Assets IA necesarios

- [ ] **2.7 Generar Oni Archer sprites**
  - [ ] Prompt: "pixel art oni archer sprite sheet, 32x32, holding bow, feudal japan demon, shooting animation"

- [ ] **2.8 Generar Yokai Runner sprites**
  - [ ] Prompt: "pixel art yokai spirit sprite sheet, 24x24, fast movement, blur effect, translucent ghost"

- [ ] **2.9 Generar armas y proyectiles**
  - [ ] Shuriken (8x8 pixel, rotating sprite)
  - [ ] Flechas (16x4 pixel)
  - [ ] Katana mejorada (sprite weapon para inventory)

- [ ] **2.10 VFX avanzados**
  - [ ] Parry spark (explosion dorada)
  - [ ] Combo trail (slash trail con particles)
  - [ ] Block shield effect

### ✅ Testeable al final del Stage 2:
- [ ] Matar enemigos con shuriken a distancia
- [ ] Hacer combos de 3 golpes
- [ ] Parry perfecto reflejando proyectil de archer
- [ ] Cambiar de postura y notar diferencia
- [ ] **COMBATE SE SIENTE PROFUNDO Y SATISFACTORIO**

---

## 🗺️ STAGE 3: GENERACIÓN PROCEDURAL BÁSICA
**Duración:** 4-5 semanas
**Objetivo:** Niveles procedurales con múltiples habitaciones

### Tareas de Programación

- [ ] **3.1 Sistema de Habitaciones (Rooms)**
  - [ ] RoomController: clase para cada habitación (16x12 tiles)
  - [ ] Puertas en N/S/E/W (collider trigger)
  - [ ] Spawn points para enemigos (Transform[] en inspector)
  - [ ] Cerrar puertas cuando entras, abrir cuando limpias enemigos

- [ ] **3.2 Generador de Niveles (Level Generator)**
  - [ ] Algoritmo de generación: Grid-based o Binary Space Partition
  - [ ] Generar 5-8 habitaciones conectadas
  - [ ] 1 habitación start, 1 habitación boss, rest normal
  - [ ] Validar que todas estén conectadas (pathfinding check)

- [ ] **3.3 Sistema de Carga de Habitaciones**
  - [ ] Cargar/descargar habitaciones dinámicamente
  - [ ] Transición entre habitaciones (fade o slide camera)
  - [ ] Cinemachine confiner para cada habitación

- [ ] **3.4 Enemy Spawning System**
  - [ ] SpawnManager por habitación
  - [ ] Leer dificultad (habitación #) y spawnear enemigos apropiados
  - [ ] Balance: 3-6 enemigos por habitación
  - [ ] Mix de tipos: 60% melee, 30% archer, 10% runner

- [ ] **3.5 Minimap básico**
  - [ ] Render Texture con cámara ortográfica top-down
  - [ ] Mostrar habitaciones visitadas
  - [ ] Indicador de posición del jugador

### Assets IA necesarios

- [ ] **3.6 Generar múltiples room templates**
  - [ ] Prompt: "pixel art japanese temple room layout, top-down view, 256x192 pixels, variations: empty hall, courtyard with pillars, corridor, shrine room"
  - [ ] Necesitas: 8-10 layouts de habitaciones distintas

- [ ] **3.7 Generar puertas y transiciones**
  - [ ] Puertas estilo torii (cerradas/abiertas)
  - [ ] Transiciones de papel shoji

- [ ] **3.8 Decoraciones procedurales**
  - [ ] Props: faroles, estatuas, árboles, rocas
  - [ ] 20-30 sprites pequeños para decorar habitaciones

### ✅ Testeable al final del Stage 3:
- [ ] Cada run genera un nivel diferente
- [ ] Recorrer 5+ habitaciones limpiando enemigos
- [ ] Puertas se abren/cierran correctamente
- [ ] Minimap muestra dónde estás
- [ ] **ROGUELIKE LOOP BÁSICO FUNCIONA**

---

## 🎲 STAGE 4: SISTEMAS DE LOOT Y BUILD VARIETY
**Duración:** 3-4 semanas
**Objetivo:** Drops, ítems, power-ups, builds distintos

### Tareas de Programación

- [ ] **4.1 Sistema de Inventario**
  - [ ] Inventory class con List<Item>
  - [ ] ScriptableObject base para ítems (nombre, sprite, efecto)
  - [ ] UI de inventario simple (grid de íconos)
  - [ ] Equipar/desequipar armas

- [ ] **4.2 Sistema de Loot Drops**
  - [ ] Enemigos dropean monedas (100% chance)
  - [ ] 20% chance de drop de ítem (weapon/scroll)
  - [ ] Pickup animation (ítem flota, magnetismo hacia jugador)
  - [ ] LootTable por tipo de enemigo

- [ ] **4.3 Armas como ítems**
  - [ ] ItemWeapon: diferentes armas con stats
  - [ ] Kusarigama (rango medio, ataque circular)
  - [ ] Naginata (más rango, más lento)
  - [ ] Dual tantō (rápido, menos daño)
  - [ ] Sistema para cambiar arma activa (teclas 1-4)

- [ ] **4.4 Sistema de Scrolls Mágicos (Onmyōdō)**
  - [ ] ItemScroll: habilidades activables (tecla E, cooldown)
  - [ ] Katon no Jutsu (fireball que hace AoE)
  - [ ] Kaze (dash rápido que atraviesa enemigos)
  - [ ] Raiju (lightning chain que salta entre enemigos)
  - [ ] Sistema de mana o cooldowns

- [ ] **4.5 Passive Items (Reliquias)**
  - [ ] ItemPassive: modificadores permanentes durante run
  - [ ] +20% velocidad ataque, +1HP, vampirismo, etc.
  - [ ] Stackeable (múltiples pasivos activos)
  - [ ] UI para ver pasivos actuales

- [ ] **4.6 Sistema de Rareza**
  - [ ] Common (blanco), Rare (azul), Epic (morado), Legendary (dorado)
  - [ ] Stats escalados por rareza
  - [ ] VFX según rareza en drop

### Assets IA necesarios

- [ ] **4.7 Generar sprites de ítems (50+ items)**
  - [ ] Prompt para armas: "pixel art katana icon, 16x16, item icon, inventory, japanese sword"
  - [ ] Prompt para scrolls: "pixel art scroll icon, 16x16, magic scroll, flames, japanese talisman"
  - [ ] Prompt para pasivos: "pixel art amulet icon, 16x16, jade pendant, japanese ornament"
  - [ ] Necesitas: 10 armas, 10 scrolls, 30 pasivos

- [ ] **4.8 Generar VFX para magias**
  - [ ] Fireball explosion (32x32, 6 frames)
  - [ ] Lightning chain (sprite strips)
  - [ ] Wind dash trail

- [ ] **4.9 UI decorativo para inventario**
  - [ ] Panel estilo japonés con ornamentos
  - [ ] Slots con marcos dorados

### ✅ Testeable al final del Stage 4:
- [ ] Matar enemigos y ver drops
- [ ] Recoger arma diferente y cambiar el gameplay
- [ ] Usar scroll mágico y ver efecto
- [ ] Acumular múltiples pasivos y sentir power creep
- [ ] **CADA RUN SE SIENTE DIFERENTE**

---

## 🏛️ STAGE 5: META-PROGRESIÓN Y HUB
**Duración:** 4-5 semanas
**Objetivo:** Aldea hub, upgrades permanentes, persistencia

### Tareas de Programación

- [ ] **5.1 Sistema de Persistencia (Save/Load)**
  - [ ] SaveData class con JSON
  - [ ] Guardar: monedas totales, upgrades comprados, stats desbloqueados
  - [ ] PlayerPrefs o archivo JSON en PersistentDataPath
  - [ ] Cargar al inicio del juego

- [ ] **5.2 Escena Hub - Aldea**
  - [ ] Nueva escena separada (Village)
  - [ ] Movimiento del jugador sin combate
  - [ ] 5 edificios interactuables: Dojo, Forge, Temple, Shop, Altar

- [ ] **5.3 Sistema de Moneda Permanente**
  - [ ] Souls/Spirit Essence: moneda que persiste entre runs
  - [ ] Al morir, convertir coins del run en souls (50% ratio)
  - [ ] Currency UI en hub

- [ ] **5.4 Dojo - Stat Upgrades**
  - [ ] Árbol de upgrades simple
  - [ ] +10% HP (cost: 100 souls, 5 niveles)
  - [ ] +10% Daño (cost: 150 souls, 5 niveles)
  - [ ] +5% Velocidad (cost: 100 souls, 3 niveles)
  - [ ] UI de árbol con botones e indicadores de nivel

- [ ] **5.5 Forge - Unlock Weapons**
  - [ ] Lista de armas bloqueadas inicialmente
  - [ ] Pagar souls para desbloquear permanentemente
  - [ ] Las armas desbloqueadas aparecerán en runs futuros
  - [ ] UI de galería de armas

- [ ] **5.6 Temple - Unlock Scrolls/Blessings**
  - [ ] Desbloquear habilidades mágicas
  - [ ] Comprar "starting blessings" (empiezas run con 1 pasivo)

- [ ] **5.7 Shop - Cosmetics**
  - [ ] Cambiar color/skin del samurái
  - [ ] Puramente cosmético pero satisfactorio
  - [ ] 5-8 skins desbloqueables

- [ ] **5.8 Altar - Start Run**
  - [ ] Interactuar para comenzar nueva run
  - [ ] Pantalla de selección de bendición inicial (elegir 1 de 3)
  - [ ] Fade out y cargar escena de generación procedural

- [ ] **5.9 NPCs y diálogos**
  - [ ] Sistema de diálogo simple (TextMeshPro panels)
  - [ ] 3-4 NPCs que dan contexto lore
  - [ ] Diálogos desbloquean al progresar

### Assets IA necesarios

- [ ] **5.10 Generar Hub/Village assets**
  - [ ] Prompt: "pixel art japanese village, top-down view, peaceful atmosphere, cherry blossoms, temple, forge, dojo buildings, 512x512"
  - [ ] Edificios separados (64x64 cada uno)
  - [ ] Decoraciones: cerezos, faroles, caminos de piedra

- [ ] **5.11 Generar NPC sprites**
  - [ ] Herrero anciano
  - [ ] Sacerdotisa miko
  - [ ] Monje guerrero
  - [ ] Mercader

- [ ] **5.12 UI para menus del hub**
  - [ ] Paneles de upgrade con estética japonesa
  - [ ] Botones ornamentados
  - [ ] Icons para cada stat

### ✅ Testeable al final del Stage 5:
- [ ] Morir en run y regresar al hub con souls
- [ ] Comprar upgrade en dojo y ver stat mejorado
- [ ] Desbloquear arma en forge
- [ ] Empezar nueva run y encontrar esa arma
- [ ] **SENTIR PROGRESIÓN PERMANENTE**

---

## 👹 STAGE 6: JEFES Y BIOMAS
**Duración:** 5-6 semanas
**Objetivo:** Múltiples biomas, jefes únicos, más enemigos

### Tareas de Programación

- [ ] **6.1 Sistema de Biomas**
  - [ ] BiomeData ScriptableObject (tileset, enemy pool, music)
  - [ ] Generador ahora crea runs de 3 biomas secuenciales
  - [ ] Bioma 1: Bamboo Forest (5 rooms)
  - [ ] Bioma 2: Haunted Castle (6 rooms)
  - [ ] Bioma 3: Yomi Underworld (7 rooms)

- [ ] **6.2 Enemy Pool por Bioma**
  - [ ] Cada bioma tiene enemigos únicos
  - [ ] Forest: Oni básicos, Tengu (volador)
  - [ ] Castle: Samurai Zombie, Oni Archer
  - [ ] Yomi: Demon Dog, Wraith (intangible)

- [ ] **6.3 Boss Framework**
  - [ ] BossController base class
  - [ ] Fases de boss (Phase 1: pattern A, Phase 2: pattern B)
  - [ ] Boss room especial (más grande, decorada)
  - [ ] Health bar de boss en top de pantalla

- [ ] **6.4 Boss 1: Oni King (Forest)**
  - [ ] Fase 1: Melee combos lentos, ground pound AoE
  - [ ] Fase 2 (<50% HP): Enrage, invoca Oni minions, ataques más rápidos
  - [ ] Patterns predecibles pero requieren esquivar

- [ ] **6.5 Boss 2: Corrupted Daimyo (Castle)**
  - [ ] Fase 1: Samurai moveset (ataques con katana, parry jugador)
  - [ ] Fase 2: Transformación oni, projectiles de fuego
  - [ ] Más técnico que boss 1

- [ ] **6.6 Boss 3: Shogun Maldito (Yomi) - FINAL**
  - [ ] Combina mecánicas de bosses previos
  - [ ] 3 fases distintas
  - [ ] Cinematics breves entre fases
  - [ ] Al derrotar: victoria run, recompensa grande

- [ ] **6.7 Enemigos adicionales (8 tipos más)**
  - [ ] Implementar IAs variadas usando state machine
  - [ ] Kitsune (teleport corto)
  - [ ] Gashadokuro (esqueleto gigante lento)
  - [ ] Kappa (dash acuático)
  - [ ] Y 5 tipos más variados

### Assets IA necesarios

- [ ] **6.8 Generar tilesets de 3 biomas**
  - [ ] Bamboo Forest: tiles de bambú, hojas, piedras, 16x16
  - [ ] Castle: tiles de castillo, madera, tatami, shoji doors
  - [ ] Yomi: tiles de infierno, lava, huesos, oscuridad

- [ ] **6.9 Generar sprites de bosses (GRANDES)**
  - [ ] Prompt: "pixel art oni king boss sprite, 64x64 pixels, huge demon, horns, club weapon, menacing, feudal japan"
  - [ ] Cada boss: idle, attack1, attack2, special, hurt, death
  - [ ] 3 bosses = ~60-90 frames totales

- [ ] **6.10 Generar 8 enemy types nuevos**
  - [ ] Usar referencias de yokai reales
  - [ ] Variedad visual y de comportamiento

- [ ] **6.11 Backgrounds por bioma**
  - [ ] Parallax backgrounds (3 layers)
  - [ ] Forest: árboles lejanos, montañas
  - [ ] Castle: murallas, cielo rojo
  - [ ] Yomi: caos dimensional, portal

- [ ] **6.12 VFX de bosses**
  - [ ] Ground pound shockwave
  - [ ] Fuego corruption
  - [ ] Transformaciones

### ✅ Testeable al final del Stage 6:
- [ ] Run completa atravesando 3 biomas distintos
- [ ] Pelear y derrotar a 3 bosses únicos
- [ ] Ver variedad visual entre biomas
- [ ] **GAME LOOP COMPLETO DE INICIO A FIN**

---

## ✨ STAGE 7: POLISH, AUDIO Y JUICE
**Duración:** 4-5 semanas
**Objetivo:** Hacer que el juego se sienta INCREÍBLE

### Tareas de Programación

- [ ] **7.1 Screen Shake y Camera Juice**
  - [ ] Cinemachine Impulse Source en hits
  - [ ] Shake distinto según intensidad
  - [ ] Camera zoom in/out en momentos clave

- [ ] **7.2 Hit Stop / Frame Freeze**
  - [ ] Time.timeScale = 0 por 0.05s en golpes críticos
  - [ ] Da sensación de impacto potente

- [ ] **7.3 Particle Systems**
  - [ ] Sangre en cada hit (3-5 particles)
  - [ ] Chispas al parry
  - [ ] Polvo al caminar
  - [ ] Fuego ambient en antorchas
  - [ ] Hojas cayendo en Forest biome

- [ ] **7.4 Post-Processing**
  - [ ] URP Post-Processing: Bloom, Vignette, Color Grading
  - [ ] Chromatic aberration leve en boss fights
  - [ ] Screen flash blanco en death

- [ ] **7.5 Animaciones UI**
  - [ ] Tweening (DOTween) para menus
  - [ ] Health bar smooth lerp
  - [ ] Pop-in de damage numbers
  - [ ] Coin collect animation

- [ ] **7.6 Tutorial Integration**
  - [ ] Primera run tiene tooltips
  - [ ] "Press SPACE to dash", "Click to attack", etc.
  - [ ] Desaparece después de usarlo una vez

- [ ] **7.7 Damage Numbers**
  - [ ] Floaty text sobre enemigos al hacer daño
  - [ ] Color según tipo (normal blanco, crítico amarillo)
  - [ ] Animación: spawn, rise, fade out

- [ ] **7.8 Death Screen y Victory Screen**
  - [ ] Stats de la run (kills, time, items found)
  - [ ] Botón retry / return to hub
  - [ ] Slow-mo death cinematic

### Assets IA necesarios

- [ ] **7.9 Generar SFX con IA audio**
  - [ ] Herramientas: ElevenLabs, Soundraw, freesound.org
  - [ ] Necesitas (~50 SFX):
    - [ ] Katana slash (x3 variaciones)
    - [ ] Hit impact (metal, flesh)
    - [ ] Fireball, lightning, wind
    - [ ] Boss roar
    - [ ] UI click, coin pickup
    - [ ] Footsteps (stone, wood)
    - [ ] Door open/close
    - [ ] Death scream

- [ ] **7.10 Generar música con IA**
  - [ ] Herramientas: AIVA, Soundraw, Mubert
  - [ ] Necesitas (6-7 tracks):
    - [ ] Hub theme (peaceful, koto, shamisen)
    - [ ] Biome 1 theme (tension, bamboo forest)
    - [ ] Biome 2 theme (dark, castle drums)
    - [ ] Biome 3 theme (hell, intense taiko)
    - [ ] Boss theme (epic, orchestral + japanese)
    - [ ] Victory jingle
    - [ ] Death jingle

- [ ] **7.11 UI polish assets**
  - [ ] Cursor custom (katana pointer)
  - [ ] Icons pulidos HD
  - [ ] Transición screen (papel shoji)

### ✅ Testeable al final del Stage 7:
- [ ] Juego se SIENTE jugoso (screen shake, particles)
- [ ] Audio/música inmersivo
- [ ] UI responde con animaciones smooth
- [ ] **EXPERIENCIA PULIDA COMO JUEGO COMERCIAL**

---

## 🚀 STAGE 8: CONTENIDO LATE-GAME Y REPLAYABILITY
**Duración:** 3-4 semanas
**Objetivo:** Asegurar que los jugadores vuelvan

### Tareas de Programación

- [ ] **8.1 New Game+ (NG+)**
  - [ ] Al vencer boss final, desbloquear NG+
  - [ ] NG+ tiers: +1, +2, etc.
  - [ ] Cada tier: enemigos +50% HP/daño, pero +100% souls
  - [ ] Desbloquea ítems raros en tiers altos

- [ ] **8.2 Daily Run**
  - [ ] Seed fija del día (usa fecha como seed)
  - [ ] Leaderboard local (top 10 scores)
  - [ ] Scoring: kills x tiempo x items

- [ ] **8.3 Challenge Mode**
  - [ ] 5-8 desafíos custom
  - [ ] Ejemplos: "Solo arco", "Sin daño run"
  - [ ] Recompensas: skins, logros

- [ ] **8.4 Endless Mode (Dojo Survival)**
  - [ ] Oleadas infinitas en arena
  - [ ] Cada oleada más difícil
  - [ ] Leaderboard de oleada máxima

- [ ] **8.5 Personajes Alternativos**
  - [ ] Desbloqueable: Ninja (más rápido, menos HP)
  - [ ] Desbloqueable: Miko (magic focus, puede curar)
  - [ ] Cada uno con mecánicas únicas

- [ ] **8.6 Logros y Coleccionables**
  - [ ] Achievement system (Unity Achievement API)
  - [ ] 30-40 logros variados
  - [ ] Bestiary (enciclopedia de enemigos)

### Assets IA necesarios

- [ ] **8.7 Generar 2 personajes alternativos completos**
  - [ ] Ninja sprite sheets (all animations)
  - [ ] Miko sprite sheets (all animations)

- [ ] **8.8 Generar recompensas visuales**
  - [ ] Skins dorados, variantes de color
  - [ ] Efectos de arma (fuego, relámpago)

### ✅ Testeable al final del Stage 8:
- [ ] Vencer el juego y continuar en NG+
- [ ] Intentar daily run
- [ ] Desbloquear personaje alternativo y jugar con él
- [ ] **CONTENIDO PARA 50+ HORAS DE JUEGO**

---

## 🐛 STAGE 9: OPTIMIZACIÓN Y BUG FIXING
**Duración:** 3-4 semanas
**Objetivo:** Estabilidad, performance 60fps+, preparar para release

### Tareas de Programación

- [ ] **9.1 Profiling y Performance**
  - [ ] Unity Profiler: identificar spikes de CPU/GPU
  - [ ] Object Pooling para TODOS los projectiles/enemies/VFX
  - [ ] Reducir Draw Calls (Sprite Atlas, batching)
  - [ ] Target: 60fps estable en PC medio (GTX 1060)

- [ ] **9.2 Bug Fixing Pass**
  - [ ] Crear spreadsheet de bugs conocidos
  - [ ] Testar cada sistema exhaustivamente
  - [ ] Casos edge (exploits, bugs de items stackeados)

- [ ] **9.3 Balancing**
  - [ ] Spreadsheet de stats de enemies, armas, ítems
  - [ ] Balancear progresión (¿es muy fácil/difícil?)
  - [ ] Probar runs completas y ajustar números

- [ ] **9.4 Accesibilidad**
  - [ ] Opciones de volumen (master, SFX, music)
  - [ ] Opciones de controles (remappeable)
  - [ ] Opciones de visuales (reducir shake, flash)
  - [ ] Colorblind mode (opcional)

- [ ] **9.5 Menus Finales**
  - [ ] Main Menu pulido
  - [ ] Pause menu en run
  - [ ] Settings submenu
  - [ ] Credits screen

- [ ] **9.6 Build Testing**
  - [ ] Builds para Windows, Mac, Linux
  - [ ] Probar en múltiples resoluciones
  - [ ] Probar con gamepad + teclado

### ✅ Testeable al final del Stage 9:
- [ ] Juego corre a 60fps sin crashes
- [ ] 0 bugs game-breaking
- [ ] Builds funcionan en todas las plataformas
- [ ] **CANDIDATE PARA RELEASE**

---

## 🎉 STAGE 10: LANZAMIENTO PC/MAC
**Duración:** 2-3 semanas
**Objetivo:** Lanzar en Steam/Itch.io

### Tareas

- [ ] **10.1 Steamworks Integration**
  - [ ] Configurar Steam SDK
  - [ ] Logros de Steam
  - [ ] Cloud saves con Steam Cloud
  - [ ] Steam Input API

- [ ] **10.2 Marketing Assets**
  - [ ] Trailer (1-2 min): capturar gameplay, editarlo
  - [ ] Screenshots (10-15): mejores momentos
  - [ ] Capsule art para Steam
  - [ ] Itch.io page

- [ ] **10.3 Store Page Setup**
  - [ ] Steam store page (descripción, tags, precio)
  - [ ] Press kit (EPK) para prensa indie
  - [ ] Discord server / community

- [ ] **10.4 Launch!**
  - [ ] Release en Steam + Itch.io
  - [ ] Post en redes sociales
  - [ ] Mandar keys a prensa/influencers

### Assets IA necesarios

- [ ] **10.5 Generar Key Art para marketing**
  - [ ] Prompt: "epic samurai warrior fighting oni demon, japanese art style, dark fantasy, official game art, detailed, red moon background"
  - [ ] Herramienta: Midjourney + Photoshop
  - [ ] Formatos de Steam: capsule, header, library art

### ✅ Testeable al final del Stage 10:
- [ ] **JUEGO LANZADO EN STEAM/ITCH.IO**
- [ ] **SHOGUN'S LEGACY DISPONIBLE PARA EL PÚBLICO**

---

## 📅 RESUMEN DE TIEMPOS

| Stage | Duración | Acumulado |
|-------|----------|-----------|
| 0. Setup | 1-2 sem | 2 sem |
| 1. MVP Combat | 3-4 sem | 6 sem |
| 2. Combat Ampliado | 3-4 sem | 10 sem |
| 3. Generación Procedural | 4-5 sem | 15 sem |
| 4. Loot & Builds | 3-4 sem | 19 sem |
| 5. Hub & Meta-progresión | 4-5 sem | 24 sem |
| 6. Bosses & Biomas | 5-6 sem | 30 sem |
| 7. Polish & Audio | 4-5 sem | 35 sem |
| 8. Late-game Content | 3-4 sem | 39 sem |
| 9. Optimización & Bugs | 3-4 sem | 43 sem |
| 10. Lanzamiento | 2-3 sem | **45-46 sem** |

**TOTAL ESTIMADO: ~10-11 meses** (full-time) | **18-24 meses** (part-time)

---

## 🎯 RUTA RÁPIDA: MVP MÍNIMO (2 meses)

Si quieres validar el concepto rápidamente:

**MVP Sprint = Stage 0 + Stage 1 + Stage 3 simplificado**
- [ ] Jugador controlable ✅
- [ ] Combate básico vs 2 tipos enemigos ✅
- [ ] 3 habitaciones procedurales ✅
- [ ] 1 arma, sin ítems
- [ ] Sin hub (sólo botón retry)

**Resultado:** Loop roguelike jugable en 8-10 semanas para validar el core gameplay

---

## 🤖 HERRAMIENTAS IA RECOMENDADAS

### Para Arte
- **Stable Diffusion** (local, gratis) + LoRA de pixel art
- **Midjourney** (mejor calidad, $10-30/mes)
- **Aseprite** ($20 one-time) - refinar sprites
- **Photopea** (gratis online) - edición

### Para Audio
- **AIVA** (música IA, free tier limitado)
- **Soundraw** (música IA, $20/mes)
- **ElevenLabs** (voces IA)
- **Freesound.org** (SFX gratis)
- **Audacity** (edición audio, gratis)

### Para Productividad
- **Este archivo ROADMAP.md** - tracking de tareas
- **GitHub Projects** - kanban board
- **Notion/Trello** - alternativas

---

## 📝 NOTAS DE USO

### Cómo usar este roadmap:
1. Ve completando las checkboxes `[ ]` → `[x]` conforme avanzas
2. Haz commit después de cada tarea importante
3. Push al final de cada sesión de trabajo
4. Usa los "Testeable al final del Stage" como milestones de validación

### Flexibilidad:
- Los tiempos son estimaciones, ajusta según tu ritmo
- Puedes reordenar tareas dentro de un mismo Stage
- Si un Stage se alarga, no te preocupes - mejor calidad que velocidad
- Siempre prioriza tener algo jugable y divertido

### MVP First:
- **Stages 0-3 son CRÍTICOS** para tener el core loop
- Stages 4-6 añaden profundidad
- Stages 7-10 son polish y contenido extra
- Si necesitas lanzar antes, puedes reducir alcance de Stages 6-8

---

**¡Buena suerte, desarrollador! 🗡️⚔️🎮**

*Last updated: 2025-11-12*
