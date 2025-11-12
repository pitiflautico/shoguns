# 🤖 AI PROMPTS FOR ASSET GENERATION

Complete guide for generating all initial game assets using AI tools.

---

## 🎨 STAGE 0 ASSETS - Initial Sprites

### 📦 Tools Recommended:
- **Stable Diffusion** with LoRA: "pixel-art-xl" or "pixelart-diffusion"
- **Midjourney** (v6 or higher) - Best quality but paid
- **DALL-E 3** - Good for concepts
- **Post-processing:** Aseprite, Photopea

---

## 🥷 PLAYER CHARACTER - Samurai

### Prompt 1: Idle Animation
```
pixel art samurai character sprite sheet, 32x32 pixels, idle animation, 4 frames,
dark red hakama pants, black armor plates, katana sheathed on hip,
traditional japanese warrior, feudal japan, dark fantasy aesthetic,
8-directional facing positions, clean pixel art style, black background,
no anti-aliasing, sharp pixels, game sprite
```

**Settings:**
- **Size:** 512x512 (then crop individual frames)
- **Style:** Pixel art, retro game, 16-bit
- **Negative prompt:** "blurry, smooth, anti-aliased, 3d, realistic, photograph"

### Prompt 2: Walk Cycle
```
pixel art samurai walk cycle sprite sheet, 32x32 pixels per frame,
8 frames walking animation, side view and front view,
dark red hakama, black armor, katana on hip,
clean pixel art, game sprite, black background, sharp pixels,
japanese warrior walking, feudal japan aesthetic
```

### Prompt 3: Attack Animation
```
pixel art samurai attack animation sprite sheet, 32x32 pixels,
katana slash sequence, 6 frames, dynamic sword swing motion,
action pose, dark red and black armor,
feudal japan warrior, game sprite, sharp pixels, black background,
multiple angles (forward, side, diagonal)
```

### Prompt 4: Death Animation
```
pixel art samurai death animation, 32x32 pixels, 5 frames,
falling backwards, dramatic death pose, katana dropping,
dark fantasy style, game sprite, black background, pixel art
```

**Expected Output:**
- Idle: 4 frames × 8 directions = 32 sprites (can simplify to 4 directions initially)
- Walk: 8 frames × 8 directions = 64 sprites (can simplify to 4 directions)
- Attack: 6 frames × 8 directions = 48 sprites (priority: 4 directions)
- Death: 5 frames

**Post-processing in Aseprite:**
1. Import AI-generated image
2. Use "Import Sprite Sheet" to separate frames
3. Clean up pixels manually (remove blur artifacts)
4. Adjust frame timing (idle: 150ms, walk: 100ms, attack: 50ms)
5. Export as PNG strips or individual files

---

## 👹 ENEMY - Oni Demon (Basic Melee)

### Prompt 5: Oni Idle
```
pixel art oni demon sprite sheet, 32x32 pixels, red skin,
two horns protruding from forehead, muscular build,
japanese folklore monster, idle animation 3 frames,
menacing pose, dark fantasy style, game enemy sprite,
black background, sharp pixel art, no blur
```

### Prompt 6: Oni Attack
```
pixel art oni demon attack animation, 32x32 pixels,
red oni with horns, claw swipe attack motion, 4 frames,
aggressive pose, japanese yokai monster,
game sprite, dark fantasy, black background, clean pixels
```

### Prompt 7: Oni Walk
```
pixel art oni demon walking sprite sheet, 32x32 pixels,
red skin, horns, heavy footsteps animation 4 frames,
japanese oni monster, side view, game enemy sprite,
black background, pixel art style
```

**Expected Output:**
- Idle: 3 frames
- Walk: 4 frames × 4 directions = 16 sprites
- Attack: 4 frames

---

## 🏯 ENVIRONMENT - Tileset (16x16)

### Prompt 8: Stone Floor Tiles
```
pixel art tileset, japanese temple stone floor, 16x16 pixel tiles,
seamless pattern, dark grey stone blocks, weathered texture,
feudal japan architecture, game tileset, black background,
modular tiles for floor, pixel art style, clean pixels
```

### Prompt 9: Wooden Wall Tiles
```
pixel art wooden wall tileset, 16x16 pixels,
japanese temple wood panels, dark brown color,
seamless tiles, feudal japan aesthetic, game tileset,
vertical and horizontal variations, black background
```

### Prompt 10: Corner and Edge Tiles
```
pixel art tileset corners and edges, 16x16 pixels,
stone floor meeting wooden walls, japanese temple style,
inner corners, outer corners, edges, transitions,
seamless modular tiles, dark fantasy aesthetic, pixel art
```

**Expected Output:**
- Floor tiles: 5-8 variations (flat stone, cracked, with moss)
- Wall tiles: 5-8 variations (solid, with ornaments, windows)
- Corners/Edges: 4 corner types, 4 edge types
- Special: Door frame (32x48 pixels)

**Post-processing:**
1. Verify all tiles are exactly 16x16 pixels
2. Test seamless tiling (place side by side)
3. Fix seams manually if needed
4. Create variations by palette swapping

---

## ⚔️ VFX - Combat Effects

### Prompt 11: Sword Slash Effect
```
pixel art sword slash effect animation, 32x32 pixels,
katana slash trail, white and blue energy, 3 frames,
arc motion blur effect, game VFX sprite, transparent background,
sharp pixel art, dramatic slash effect
```

### Prompt 12: Hit Impact Sparks
```
pixel art hit impact effect, 16x16 pixels, 4 frames,
orange and yellow sparks flying, collision effect,
game VFX animation, transparent background, pixel art style,
metal clashing sparks
```

### Prompt 13: Blood Splatter (Optional - keep stylized)
```
pixel art blood splatter effect, 16x16 pixels, 3 frames,
dark red droplets, stylized game effect, not realistic,
anime style blood, pixel art VFX, transparent background
```

**Expected Output:**
- Slash: 3 frames, directional (horizontal, vertical, diagonal)
- Hit: 4 frames
- Blood: 3 frames (optional)

---

## 🎨 UI ELEMENTS

### Prompt 14: Health Bar Frame
```
pixel art UI health bar frame, 200x30 pixels,
ornate japanese style border, gold and red colors,
decorative corners with japanese motifs, game UI,
empty frame for health bar, pixel art style, transparent background
```

### Prompt 15: UI Panel
```
pixel art UI panel, 300x200 pixels, japanese style window,
wooden frame with paper texture center, shoji screen inspired,
ornate corners, feudal japan aesthetic, game menu panel,
pixel art, transparent background
```

### Prompt 16: Coin Icon
```
pixel art gold coin icon, 16x16 pixels, japanese mon symbol,
shiny gold color, simple game currency icon, pixel art,
transparent background, clear readable design
```

**Expected Output:**
- Health bar frame: 200x30px
- UI panel: 300x200px (scalable)
- Icons: 16x16px each (coin, soul essence, etc.)

---

## 🎵 AUDIO GENERATION (STAGE 7, but plan ahead)

### Music Prompts (for AIVA, Soundraw, Mubert)

#### Track 1: Hub Theme
```
Peaceful japanese instrumental music, koto and shamisen,
calm atmosphere, cherry blossom serenity, traditional instruments,
2 minute loop, no drums, slow tempo (70 BPM),
safe haven theme, contemplative mood
```

#### Track 2: Combat Theme (Biome 1 - Forest)
```
Dark japanese action music, taiko drums, shamisen,
tense atmosphere, feudal japan battle, moderate tempo (120 BPM),
bamboo forest ambience, mysterious and dangerous,
2 minute loop, action combat theme
```

#### Track 3: Boss Battle
```
Epic japanese boss battle music, heavy taiko drums,
intense shamisen, dramatic koto, fast tempo (150 BPM),
orchestral strings mixed with traditional japanese instruments,
heroic and terrifying, climactic battle theme, 3 minute loop
```

### SFX Prompts (for ElevenLabs or manual search on Freesound)

**Needed SFX List:**
1. **Katana Slash** - "sword swoosh, blade cutting air" (3 variations)
2. **Hit Impact** - "metal clang, sword hitting armor" (3 variations)
3. **Footsteps** - "footsteps on stone, wooden floor" (4 variations)
4. **Dash/Whoosh** - "fast movement, air burst"
5. **Enemy Hurt** - "oni demon grunt, monster pain sound"
6. **Player Hurt** - "male grunt, samurai damage sound"
7. **Death Scream** - "dramatic death cry"
8. **Door Open** - "wooden sliding door, shoji screen"
9. **Item Pickup** - "coin collect, pickup chime"
10. **UI Click** - "button click, menu select"

**Source:** Freesound.org (search with keywords) or generate with AI

---

## 📊 ASSET GENERATION WORKFLOW

### Step-by-Step Process:

1. **Generate with AI** (Midjourney/Stable Diffusion)
   - Use prompts above
   - Generate multiple variations
   - Download highest quality

2. **Clean in Aseprite**
   - Import image
   - Remove blur/anti-aliasing
   - Perfect pixels manually
   - Separate sprite sheets into frames
   - Add missing frames by duplicating/editing

3. **Export for Unity**
   - Export as PNG (8-bit or 32-bit with alpha)
   - Name consistently: `Player_Idle_00.png`, `Player_Walk_00.png`, etc.
   - Or export as sprite sheet with JSON metadata

4. **Import to Unity**
   - Place in correct `Assets/Sprites/` subfolder
   - Set Texture Type: "Sprite (2D and UI)"
   - Set Pixels Per Unit: 16 (for 16x16) or 32 (for 32x32)
   - Set Filter Mode: "Point (no filter)" for crisp pixels
   - Set Compression: "None" for pixel art

5. **Slice Sprite Sheets**
   - Open Sprite Editor in Unity
   - Slice by Grid: 32x32 (or appropriate size)
   - Apply
   - Rename slices: frame_0, frame_1, etc.

---

## 🎯 PRIORITY ORDER FOR STAGE 0

Generate in this order to unblock development:

### ✅ Highest Priority (needed for Stage 1)
1. Player Idle (1 direction minimum)
2. Player Walk (1 direction)
3. Player Attack (1 direction)
4. Oni Idle
5. Oni Walk
6. Basic floor tiles (3-5 tiles)
7. Basic wall tiles (3-5 tiles)
8. Slash VFX

### ⚠️ Medium Priority (needed for Stage 1 polish)
9. Player animations (all 4 directions)
10. Oni Attack animation
11. Hit impact VFX
12. Health bar frame
13. More tileset variations

### 🔜 Lower Priority (Stage 2+)
14. Player Death animation
15. 8-directional sprites
16. Blood VFX
17. Full UI set
18. Audio (Stage 7)

---

## 💡 TIPS FOR BETTER AI GENERATION

### For Stable Diffusion:
- **Model:** Use "pixel-art-xl" LoRA or custom pixel art checkpoint
- **Sampler:** DPM++ 2M Karras or Euler A
- **Steps:** 20-30
- **CFG Scale:** 7-9
- **Resolution:** 512x512 minimum

### For Midjourney:
- Add `--style raw` for more literal interpretation
- Add `--no blur, smooth, antialiasing` to negative prompt
- Use `--tile` for seamless tilesets
- Use v6 model for best pixel art

### General Tips:
- Generate 4-6 variations, pick the best
- Lower resolution = more pixelated (good for pixel art)
- Always specify "black background" or "transparent background"
- Emphasize "sharp pixels", "no blur", "clean pixel art"
- Reference specific games: "like Hyper Light Drifter", "like Dead Cells"

---

## ✅ CHECKLIST FOR STAGE 0 ASSETS

- [ ] Player Idle sprite sheet (4 frames min)
- [ ] Player Walk sprite sheet (8 frames min)
- [ ] Player Attack sprite sheet (6 frames min)
- [ ] Oni Idle sprite (3 frames)
- [ ] Oni Walk sprite (4 frames)
- [ ] Oni Attack sprite (4 frames)
- [ ] Floor tileset (5-8 tiles, 16x16)
- [ ] Wall tileset (5-8 tiles, 16x16)
- [ ] Slash VFX (3 frames)
- [ ] Hit impact VFX (4 frames)
- [ ] Health bar UI frame
- [ ] All sprites imported to Unity
- [ ] All sprites sliced and configured
- [ ] Test animation created in Unity (Animator)

**Estimated Time:** 4-8 hours (including generation, cleanup, import)

---

## 📚 RESOURCES

### AI Tools:
- **Stable Diffusion:** https://stability.ai/
- **Midjourney:** https://midjourney.com/
- **Leonardo.ai:** https://leonardo.ai/ (alternative)
- **AIVA:** https://www.aiva.ai/
- **Soundraw:** https://soundraw.io/

### Asset Tools:
- **Aseprite:** https://www.aseprite.org/ ($20)
- **Libresprite:** https://libresprite.github.io/ (free Aseprite fork)
- **Photopea:** https://www.photopea.com/ (free online Photoshop)

### Audio:
- **Freesound:** https://freesound.org/
- **OpenGameArt:** https://opengameart.org/
- **Audacity:** https://www.audacityteam.org/

### Learning:
- **Pixel Art Tutorial:** https://blog.studiominiboss.com/pixelart
- **Unity 2D Animation:** https://learn.unity.com/tutorial/introduction-to-sprite-animations

---

**Ready to generate assets?** Start with the highest priority items and move down the list!

**Last Updated:** 2025-11-12
