# 🎮 STAGE 1: MVP CORE - COMBATE BÁSICO - Setup Guide

Complete guide to implement the Stage 1 gameplay in Unity.

---

## 📦 Prerequisites

- [ ] Stage 0 completed (Unity project created, packages installed)
- [ ] Base scripts copied to `Assets/Scripts/`
- [ ] Player sprites generated with AI (idle, walk, attack)
- [ ] Enemy sprites generated (Oni idle, walk, attack)
- [ ] Basic tileset imported
- [ ] VFX sprites (slash, hit impact)

---

## 🎯 Stage 1 Goals

By the end of this stage, you will have:
- Playable samurai character with WASD movement
- Mouse aim and sprite flipping
- Melee attack with hitbox detection
- Basic enemy AI (chase and attack)
- Health system for both player and enemy
- Health bar UI
- Combat feels functional

---

## 📂 Step 1: Copy New Scripts

```bash
# Copy all Stage 1 scripts to Unity project
cp -r UnityScripts/* ShogunsLegacy/Assets/Scripts/
```

Wait for Unity to compile. Check Console for errors (should be 0).

### New Scripts Added:
- `Player/PlayerController.cs`
- `Player/PlayerAnimationController.cs`
- `Player/PlayerCombat.cs`
- `Enemy/EnemyAI.cs`
- `UI/HealthBarUI.cs`
- `Utils/VFXManager.cs`

---

## 🎨 Step 2: Create Player GameObject

### 2.1 Create Player

1. In Hierarchy: Right-click → 2D Object → Sprite → Square
2. Rename to `Player`
3. Set Position: `(0, 0, 0)`
4. Set Tag: `Player`
5. Set Layer: `Player`

### 2.2 Configure Player Sprite

1. Select Player
2. In Sprite Renderer:
   - Sprite: Your player idle sprite
   - Sorting Layer: Create new layer `Characters` (Order: 10)
   - Order in Layer: 0

### 2.3 Add Components to Player

Click "Add Component" and add the following:

#### Rigidbody2D
- Body Type: Dynamic
- Linear Drag: 0
- Angular Drag: 0
- Gravity Scale: 0
- Constraints: Freeze Rotation Z ✅

#### Collider
- Add Component → Circle Collider 2D
- Radius: 0.3
- Is Trigger: ❌ (unchecked)

#### Health Component
- Add Component → Health Component (from your scripts)
- Max Health: 100
- Has Invulnerability Frames: ✅
- Invulnerability Duration: 0.5

#### Player Controller
- Add Component → Player Controller
- Move Speed: 5
- Can Dash: ✅
- Dash Speed: 15
- Dash Duration: 0.2
- Dash Cooldown: 1
- Debug Mode: ✅ (for testing)

#### Player Animation Controller
- Add Component → Player Animation Controller

#### Player Combat
- Add Component → Player Combat
- Attack Range: 1.5
- Attack Cooldown: 0.5
- Enemy Layers: Select "Enemy" layer
- Combo Enabled: ✅
- Max Combo Count: 3
- Debug Mode: ✅
- Show Gizmos: ✅

### 2.4 Create Weapon Pivot

1. Right-click Player → Create Empty
2. Rename to `WeaponPivot`
3. Position: `(0, 0, 0)` (relative to Player)
4. In Player Controller component:
   - Drag `WeaponPivot` into "Weapon Pivot" field

### 2.5 Create Attack Point

1. Right-click WeaponPivot → Create Empty
2. Rename to `AttackPoint`
3. Position: `(1, 0, 0)` (1 unit to the right)
4. In Player Combat component:
   - Drag `AttackPoint` into "Attack Point" field

### 2.6 Create Player Stats ScriptableObject

1. In Project: `Assets/ScriptableObjects/Stats/`
2. Right-click → Create → Shoguns Legacy → Stats → Entity Stats
3. Rename to `PlayerStats`
4. Configure:
   - Max Health: 100
   - Base Damage: 15
   - Attack Speed: 1.0
   - Critical Chance: 0.1
   - Critical Multiplier: 2.0
   - Move Speed: 5
   - Can Dash: ✅
   - Dash Cooldown: 1
   - Dash Distance: 3
5. In Player Controller:
   - Drag `PlayerStats` into "Stats" field
6. In Player Combat:
   - Drag `PlayerStats` into "Stats" field

---

## 👹 Step 3: Create Enemy GameObject (Oni)

### 3.1 Create Enemy

1. Hierarchy: Right-click → 2D Object → Sprite → Square
2. Rename to `Oni`
3. Position: `(5, 0, 0)`
4. Set Tag: `Enemy`
5. Set Layer: `Enemy`

### 3.2 Configure Enemy Sprite

1. Select Oni
2. Sprite Renderer:
   - Sprite: Your Oni idle sprite
   - Sorting Layer: Characters
   - Order in Layer: 0

### 3.3 Add Components to Enemy

#### Rigidbody2D
- Same settings as Player

#### Circle Collider 2D
- Radius: 0.4

#### Health Component
- Max Health: 50
- Has Invulnerability Frames: ✅
- Invulnerability Duration: 0.3

#### Enemy AI
- Add Component → Enemy AI
- Detection Range: 8
- Attack Range: 1.5
- Lose Target Range: 12
- Move Speed: 3
- Attack Cooldown: 1.5
- Attack Damage: 10
- Player Layer: Select "Player" layer
- Debug Mode: ✅
- Show Gizmos: ✅

### 3.4 Create Enemy Attack Point

1. Right-click Oni → Create Empty
2. Rename to `AttackPoint`
3. Position: `(0.5, 0, 0)`
4. In Enemy AI:
   - Drag `AttackPoint` into "Attack Point" field

### 3.5 Create Enemy Stats

1. Assets/ScriptableObjects/Stats/
2. Create → Shoguns Legacy → Stats → Entity Stats
3. Rename to `OniStats`
4. Configure:
   - Max Health: 50
   - Base Damage: 10
   - Move Speed: 3
5. In Enemy AI:
   - Drag `OniStats` into "Stats" field

---

## 🏗️ Step 4: Create Test Room

### 4.1 Create Tilemap

1. Hierarchy: Right-click → 2D Object → Tilemap → Rectangular
2. Rename Grid to `Level`
3. Select Tilemap child object
4. Set Layer: `Ground`
5. Sorting Layer: Create new `Ground` (Order: 0)

### 4.2 Create Tile Palette

1. Window → 2D → Tile Palette
2. Create New Palette → Name: "Environment"
3. Drag your tileset sprites from Project to Palette
4. Unity will prompt to save tiles → Save in `Assets/Tiles/`

### 4.3 Paint Room

1. In Tile Palette, select tiles
2. Paint a 20x15 room in Scene view
3. Create walls around the edges

### 4.4 Add Tilemap Collider

1. Select Tilemap
2. Add Component → Tilemap Collider 2D
3. Add Component → Composite Collider 2D
4. In Tilemap Collider 2D:
   - Used By Composite: ✅
5. In Rigidbody2D (auto-added):
   - Body Type: Static

---

## 🎨 Step 5: Setup UI

### 5.1 Create Canvas

1. Hierarchy: Right-click → UI → Canvas
2. Canvas:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler:
     - UI Scale Mode: Scale with Screen Size
     - Reference Resolution: 1920 x 1080

### 5.2 Create Health Bar

1. Right-click Canvas → UI → Image
2. Rename to `HealthBar_Background`
3. Rect Transform:
   - Anchor: Top-Left
   - Pos X: 100, Pos Y: -50
   - Width: 300, Height: 30
4. Image:
   - Color: Black (alpha: 0.7)

5. Right-click HealthBar_Background → UI → Image
6. Rename to `HealthBar_Fill`
7. Rect Transform:
   - Anchors: Stretch (left: 0, right: 0, top: 0, bottom: 0)
   - Left: 5, Right: -5, Top: -5, Bottom: 5
8. Image:
   - Color: Green
   - Image Type: Filled
   - Fill Method: Horizontal
   - Fill Origin: Left

9. Right-click HealthBar_Background → UI → Text - TextMeshPro
10. Rename to `HealthText`
11. Rect Transform: Center it
12. TextMeshPro:
    - Text: "100/100"
    - Font Size: 20
    - Alignment: Center + Middle
    - Color: White

### 5.3 Add HealthBarUI Script

1. Select `HealthBar_Background`
2. Add Component → Health Bar UI
3. Configure:
   - Target Health: Drag `Player` GameObject
   - Fill Image: Drag `HealthBar_Fill`
   - Health Text: Drag `HealthText`
   - Smooth Transition: ✅
   - Transition Speed: 5
   - Show Numbers: ✅
   - Flash On Damage: ✅

---

## 🎥 Step 6: Setup Camera

### 6.1 Configure Main Camera

1. Select Main Camera
2. Camera:
   - Background: Dark color (20, 20, 30)
   - Size: 5

### 6.2 Add Cinemachine Virtual Camera

1. In Hierarchy: Right-click → Cinemachine → 2D Camera
2. Select CM vcam1
3. Configure:
   - Follow: Drag `Player`
   - Look At: Drag `Player` (optional)
   - Lens → Orthographic Size: 5
   - Body → Dead Zone: 0.2, 0.2
   - Body → Soft Zone: 0.5, 0.5

### 6.3 Add Cinemachine Confiner (Optional - bounds)

1. Create Empty GameObject: `RoomBounds`
2. Add Component → Polygon Collider 2D
3. Set as Trigger: ✅
4. Edit points to match room size
5. In CM vcam1:
   - Extensions → Add Extension: CinemachineConfiner2D
   - Bounding Shape 2D: Drag `RoomBounds`

---

## 🎮 Step 7: Setup Input

### 7.1 Configure Input Manager

1. In Hierarchy: Find `InputManager` (from Managers scene)
2. If not exists, create Empty → Add Component → Input Manager
3. Add Component → Player Input
4. Player Input:
   - Actions: `PlayerInputActions` (created in Stage 0)
   - Default Map: Gameplay
   - Behavior: Invoke Unity Events

### 7.2 Connect Input Events

In Player Input → Events → Gameplay:

1. **Move**:
   - Click `+`
   - Drag `InputManager` into Object field
   - Function: `InputManager > OnMove(CallbackContext)`

2. **Aim**:
   - Click `+`
   - Drag `InputManager` into Object field
   - Function: `InputManager > OnAim(CallbackContext)`

3. **Attack**:
   - Click `+`
   - Drag `InputManager` into Object field
   - Function: `InputManager > OnAttack(CallbackContext)`

4. **Dash**:
   - Click `+`
   - Drag `InputManager` into Object field
   - Function: `InputManager > OnDash(CallbackContext)`

5. **Pause**:
   - Click `+`
   - Drag `InputManager` into Object field
   - Function: `InputManager > OnPause(CallbackContext)`

---

## ⚙️ Step 8: Configure Physics Collision Matrix

1. Edit → Project Settings → Physics 2D
2. Layer Collision Matrix (bottom of window):

| Layer | Player | Enemy | Ground | Projectile |
|-------|--------|-------|--------|------------|
| Player | ❌ | ✅ | ✅ | ❌ |
| Enemy | ✅ | ❌ | ✅ | ✅ |
| Ground | ✅ | ✅ | ❌ | ✅ |
| Projectile | ❌ | ✅ | ✅ | ❌ |

- ✅ = Layers collide
- ❌ = Layers don't collide

---

## 🧪 Step 9: Test Basic Gameplay

### 9.1 Initial Test

1. Click **Play** ▶️
2. **WASD** to move
3. **Mouse** to aim (player should flip)
4. **Left Click** to attack
5. **Space** to dash

### 9.2 Verify Functionality

- [ ] Player moves with WASD
- [ ] Player sprite flips based on mouse position
- [ ] Attack animation plays on left click
- [ ] Red circle (attack range) appears in Scene view
- [ ] Enemy detects player when close
- [ ] Enemy chases player (yellow line in Scene)
- [ ] Enemy attacks when in range
- [ ] Health bar decreases when damaged
- [ ] Player/Enemy die when health reaches 0

### 9.3 Troubleshooting

**Player not moving:**
- Check InputManager is receiving input (watch Inspector while playing)
- Verify Player Input component is connected
- Check Rigidbody2D is Dynamic, Gravity Scale = 0

**Attack not damaging enemy:**
- Check Attack Point position (should be in front of player)
- Verify Attack Range in Scene view (red circle)
- Check Enemy Layers in PlayerCombat is set to "Enemy"
- Verify Enemy has HealthComponent

**Enemy not chasing:**
- Check Detection Range (yellow circle in Scene)
- Verify Player tag is "Player"
- Check Player Layer in EnemyAI is set to "Player"

**No health bar showing:**
- Verify HealthBarUI target is set to Player HealthComponent
- Check Canvas is enabled
- Check Fill Image fillAmount is changing

---

## 🎨 Step 10: Create Placeholder VFX (Temporary)

Until you have final VFX sprites:

### 10.1 Create Slash VFX

1. Hierarchy: Create Empty → Name: `SlashVFX`
2. Add Component → Sprite Renderer
3. Create simple sprite:
   - In Project: Right-click → Create → Sprites → Square
   - Tint it white/yellow
4. Add Component → Animator (optional for animation)
5. Make it a Prefab: Drag to `Assets/Prefabs/VFX/`

### 10.2 Create Hit VFX

1. Same process, name: `HitVFX`
2. Tint it orange/red
3. Make smaller (scale 0.5)

### 10.3 Assign VFX to PlayerCombat

1. Select Player
2. In Player Combat:
   - Slash VFX Prefab: Drag `SlashVFX` prefab
   - Hit VFX Prefab: Drag `HitVFX` prefab

---

## ✅ Stage 1 Complete Checklist

- [ ] Player moves smoothly with WASD
- [ ] Player aims with mouse, sprite flips correctly
- [ ] Click to attack, deals damage to enemies
- [ ] Dash works (Space key)
- [ ] Enemy chases player when in range
- [ ] Enemy attacks player when close
- [ ] Health bars work for both player and enemy
- [ ] Enemy dies and disappears when HP = 0
- [ ] Player dies and game over state triggers
- [ ] VFX spawn on attacks (even if placeholder)
- [ ] No console errors

---

## 📈 Performance Tips

### While testing:

1. **Stats window:** Window → Analysis → Stats (shows FPS, draw calls)
2. **Profiler:** Window → Analysis → Profiler (check for performance issues)
3. Target: **60 FPS** steady

### Common issues:
- Too many sprite renderers without batching
- Missing object pooling (we'll add in Stage 2)
- Excessive Debug.Log calls (disable Debug Mode when not needed)

---

## 🎯 Next Steps After Stage 1

Once Stage 1 is complete and tested:

1. **Test thoroughly** - Play for 5-10 minutes
2. **Fix any bugs** - Note them down
3. **Adjust values** - Tweak move speed, damage, etc. until it feels good
4. **Commit to Git:**
   ```bash
   git add .
   git commit -m "Stage 1: MVP combat complete"
   git push
   ```

5. **Update ROADMAP.md** - Mark Stage 1 as complete

6. **Move to Stage 2** - Expand combat system with:
   - Projectile attacks (shuriken)
   - Combo system improvements
   - More enemy types
   - Parry/block system

---

## 🆘 Common Issues and Solutions

### Issue: "Assembly not found" errors
**Solution:** Ensure all scripts are in correct folders under `Assets/Scripts/`

### Issue: Player falls through floor
**Solution:**
- Check Tilemap has Tilemap Collider 2D + Composite Collider 2D
- Check Rigidbody2D on Tilemap is Static

### Issue: Input not working
**Solution:**
- Verify InputManager GameObject exists in scene
- Check Player Input component is added and connected
- Test in Game view, not Scene view

### Issue: Attack hits but no damage
**Solution:**
- Check HealthComponent is attached to enemy
- Verify enemy Layer is set to "Enemy"
- Check PlayerCombat "Enemy Layers" includes Enemy layer
- Look for errors in Console during attack

### Issue: Health bar not updating
**Solution:**
- Check HealthBarUI "Target Health" field has HealthComponent
- Verify fillImage is assigned
- Check Canvas is enabled in Hierarchy

---

**Estimated Time for Stage 1 Setup:** 3-4 hours (first time)

**Last Updated:** 2025-11-12
