# 🎮 STAGE 2: SISTEMAS DE COMBATE AMPLIADOS - Setup Guide

Complete guide to implement Stage 2 advanced combat systems in Unity.

---

## 📦 Prerequisites

- [ ] Stage 1 completed and tested
- [ ] Player can move, attack, and kill enemies
- [ ] Basic gameplay loop working
- [ ] Additional sprites generated (arrows, shuriken, new enemies)

---

## 🎯 Stage 2 Goals

By the end of this stage, you will have:
- Projectile system (shuriken throwing)
- Parry/Block defensive mechanics
- Combat stance system (Water, Earth)
- 2 new enemy types (Archer, Runner)
- Advanced VFX (damage numbers, camera shake)
- Much deeper combat system

---

## 📂 Step 1: Copy New Scripts

```bash
# Copy all Stage 2 scripts to Unity project
cp -r UnityScripts/* ShogunsLegacy/Assets/Scripts/
```

Wait for Unity to compile. Check Console for errors.

### New Scripts Added:
- `Combat/ProjectileController.cs`
- `Data/CombatStance.cs`
- `Player/StanceManager.cs`
- `Player/PlayerDefense.cs`
- `Enemy/OniArcher.cs`
- `Enemy/YokaiRunner.cs`
- `Utils/DamageNumber.cs`
- `Utils/CameraShake.cs`

---

## 🎯 Step 2: Create Projectile System

### 2.1 Create Shuriken Prefab

1. Hierarchy: Create Empty → Name: `Shuriken`
2. Add Component → Sprite Renderer
   - Sprite: Your shuriken sprite (8x8)
   - Sorting Layer: `Characters`
   - Order: 5

3. Add Component → Rigidbody2D
   - Body Type: Dynamic
   - Gravity Scale: 0
   - Linear Drag: 0
   - Freeze Rotation: Check

4. Add Component → Circle Collider 2D
   - Radius: 0.2
   - Is Trigger: ✅

5. Add Component → Trail Renderer (optional)
   - Width: 0.1
   - Time: 0.2
   - Material: Default-Particle
   - Color: Blue/white gradient

6. Add Component → Projectile Controller
   - Speed: 10
   - Base Damage: 8
   - Destroy On Hit: ✅
   - Target Layers: `Enemy`
   - Max Lifetime: 3
   - Max Distance: 15
   - Can Pierce: ❌

7. Drag to `Assets/Prefabs/Combat/` to make prefab

### 2.2 Create Arrow Prefab (for Archer enemy)

Same process as shuriken, but:
- Name: `Arrow`
- Sprite: Arrow sprite (16x4)
- Speed: 8
- Base Damage: 8
- Can Pierce: ❌
- Trail color: Red/orange

### 2.3 Add Projectile Shooting to Player

1. Select Player
2. In Player Combat component:
   - Find "Projectile Settings" (you'll need to add these fields or they're already there in extended version)
   - Projectile Prefab: Drag `Shuriken` prefab
   - Projectile Speed: 10
   - Projectile Damage: 8
   - Projectile Cooldown: 0.5

3. Test: Right-click should throw shuriken

---

## 🛡️ Step 3: Setup Parry/Block System

### 3.1 Add PlayerDefense Component

1. Select Player GameObject
2. Add Component → Player Defense
3. Configure:
   - Can Block: ✅
   - Block Damage Reduction: 0.8 (80% reduction)
   - Block Movement Penalty: 0.5
   - Block Key: Left Shift
   - Can Parry: ✅
   - Parry Window: 0.2
   - Parry Cooldown: 1
   - Parry Stun Duration: 1.5
   - Can Reflect Projectiles: ✅
   - Projectile Layer: `Projectile`

### 3.2 Create Block VFX (Simple)

1. Create Empty → Name: `BlockVFX`
2. Add Sprite Renderer
   - Sprite: Simple shield/circle sprite
   - Color: Blue semi-transparent
   - Sorting Order: 10

3. Add Simple Animation (optional):
   - Pulse scale 0.8 → 1.0 loop

4. Save as Prefab in `Assets/Prefabs/VFX/`

5. In Player Defense:
   - Block VFX Prefab: Drag `BlockVFX`
   - Block VFX Spawn Point: Create Empty child of Player at (0.5, 0, 0)

### 3.3 Create Parry VFX

Similar to Block but:
- Name: `ParryVFX`
- Color: Gold/yellow
- Larger scale
- One-shot animation (not looping)

### 3.4 Test Block and Parry

- Hold Shift: Should see block VFX
- Press Shift right before enemy hits: Should trigger parry
- Parry projectile: Should reflect it back

---

## ⚔️ Step 4: Combat Stance System

### 4.1 Create Stance ScriptableObjects

1. `Assets/ScriptableObjects/Stances/` (create folder)

2. Create → Shoguns Legacy → Combat → Stance

3. **Water Stance** (Fast/Agile):
   - Stance Name: "Water Stance"
   - Description: "Flow like water - Swift strikes"
   - Stance Color: Blue
   - Move Speed Multiplier: 1.2
   - Attack Speed Multiplier: 1.3
   - Damage Multiplier: 0.9
   - Defense Multiplier: 1.0
   - Critical Chance Bonus: 0.05
   - Has Super Armor: ❌
   - Attack Range Multiplier: 0.9
   - Combo Length: 4

4. **Earth Stance** (Heavy/Powerful):
   - Stance Name: "Earth Stance"
   - Description: "Immovable as earth - Devastating power"
   - Stance Color: Brown/Orange
   - Move Speed Multiplier: 0.8
   - Attack Speed Multiplier: 0.7
   - Damage Multiplier: 1.5
   - Defense Multiplier: 0.7 (takes less damage)
   - Critical Chance Bonus: 0
   - Has Super Armor: ✅
   - Attack Range Multiplier: 1.2
   - Combo Length: 2

### 4.2 Add StanceManager to Player

1. Select Player
2. Add Component → Stance Manager
3. Configure:
   - Available Stances: (Drag Water and Earth stances)
   - Default Stance: Water Stance
   - Can Switch During Combat: ✅
   - Switch Cooldown: 0.5
   - Cycle Stance Key: Q
   - Player Controller: Auto-filled
   - Player Combat: Auto-filled

4. Create Aura VFX Spawn Point:
   - Right-click Player → Create Empty
   - Name: `AuraSpawnPoint`
   - Position: (0, -0.5, 0) (below player)

5. In Stance Manager:
   - Aura VFX Spawn Point: Drag `AuraSpawnPoint`

### 4.3 Create Stance UI Indicator

1. In Canvas, create UI Image
2. Name: `StanceIndicator`
3. Position: Top-left corner (under health bar)
4. Size: 50x50
5. Add TextMeshPro text showing current stance name

6. Create script to update this (simple):

```csharp
// StanceUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShogunsLegacy.Player;

public class StanceUI : MonoBehaviour
{
    [SerializeField] private StanceManager stanceManager;
    [SerializeField] private Image stanceIcon;
    [SerializeField] private TextMeshProUGUI stanceText;

    private void Start()
    {
        if (stanceManager != null)
        {
            stanceManager.OnStanceChanged.AddListener(UpdateUI);
            UpdateUI(stanceManager.GetCurrentStance());
        }
    }

    private void UpdateUI(ShogunsLegacy.Data.CombatStance stance)
    {
        if (stance == null) return;

        if (stanceIcon != null)
        {
            stanceIcon.color = stance.StanceColor;
            if (stance.StanceIcon != null)
                stanceIcon.sprite = stance.StanceIcon;
        }

        if (stanceText != null)
        {
            stanceText.text = stance.StanceName;
        }
    }
}
```

### 4.4 Test Stances

- Press Q: Should cycle between Water and Earth
- Notice speed/damage changes
- UI should update

---

## 🏹 Step 5: Create OniArcher Enemy

### 5.1 Create Archer GameObject

1. Duplicate `Oni` enemy → Rename to `OniArcher`
2. Position: (8, 2, 0)
3. Change sprite to Archer sprite

### 5.2 Replace AI Component

1. Remove `Enemy AI` component
2. Add Component → Oni Archer
3. Configure:
   - Stats: Create `OniArcherStats` (Max HP: 40)
   - Shoot Point: Create Empty child at (0.5, 0, 0)
   - Projectile Prefab: Drag `Arrow` prefab
   - Player Layer: `Player`
   - Detection Range: 10
   - Preferred Min Range: 5
   - Preferred Max Range: 8
   - Too Close Range: 3
   - Move Speed: 2.5
   - Flee Speed: 4
   - Shoot Cooldown: 2
   - Projectile Speed: 8
   - Projectile Damage: 8
   - Burst Count: 1
   - Debug Mode: ✅
   - Show Gizmos: ✅

### 5.3 Test Archer

- Should stay at distance
- Should shoot arrows
- Should flee if you get close
- Should strafe occasionally

---

## 💨 Step 6: Create YokaiRunner Enemy

### 6.1 Create Runner GameObject

1. Create new 2D Object → Sprite → Square
2. Name: `YokaiRunner`
3. Position: (0, 5, 0)
4. Sprite: Runner sprite (smaller, more agile-looking)
5. Scale: 0.8, 0.8, 1 (slightly smaller)
6. Tag: `Enemy`
7. Layer: `Enemy`

### 6.2 Add Components

**Rigidbody2D** (same as other enemies)

**Circle Collider 2D**
- Radius: 0.3
- Is Trigger: ❌

**Health Component**
- Max Health: 30 (low HP, glass cannon)

**Yokai Runner Script**
- Stats: Create `YokaiRunnerStats` (Max HP: 30, Damage: 12, Speed: 4)
- Player Layer: `Player`
- Detection Range: 12
- Normal Speed: 4
- Dash Speed: 12
- Dash Windup Time: 0.5
- Dash Duration: 0.4
- Dash Cooldown: 2
- Dash Range: 8
- Dash Damage: 12
- Dash Through Target: ✅
- Circle Before Dash: ✅
- Circle Distance: 5
- Circle Duration: 1.5
- Can Evade: ✅
- Debug Mode: ✅

### 6.3 Add Trail Renderer

1. Add Component → Trail Renderer
2. Configure:
   - Width: Start: 0.2, End: 0
   - Time: 0.3
   - Color: Gradient (white → transparent)
   - Material: Default-Particle
   - Sorting Layer: Characters
   - Order: 1

### 6.4 Test Runner

- Should circle around player
- Should wind up (flash red) before dashing
- Should dash through player dealing damage
- Should be harder to hit due to speed

---

## ✨ Step 7: Advanced VFX - Damage Numbers

### 7.1 Create Damage Number Prefab

1. Create Empty → Name: `DamageNumber`
2. Add Component → TextMeshPro Text
3. Configure TextMeshPro:
   - Font: Any pixelated font
   - Font Size: 4
   - Color: White
   - Alignment: Center/Middle
   - Sorting Layer: UI or top layer
   - Order: 100

4. Add Component → Damage Number
5. Configure:
   - Lifetime: 1
   - Rise Speed: 2
   - Randomize Direction: ✅
   - Normal Color: White
   - Critical Color: Yellow
   - Heal Color: Green

6. Save as Prefab in `Assets/Resources/DamageNumber` (must be in Resources!)

### 7.2 Integrate with Combat

In `PlayerCombat.cs`, after dealing damage, add:

```csharp
// Show damage number
DamageNumber.Spawn(enemy.transform.position, damage, isCritical);
```

Similarly in `EnemyAI.cs` when player is hit.

---

## 📹 Step 8: Camera Shake

### 8.1 Setup Camera Shake

1. Select Main Camera (or Cinemachine vcam)
2. Add Component → Cinemachine Impulse Listener
3. Create Empty GameObject → Name: `CameraShake`
4. Add Component → Camera Shake script
5. Add Component → Cinemachine Impulse Source
6. Configure Impulse Source:
   - Raw Signal: (Create new) → 6D Shake
   - Amplitude Gain: 1
   - Frequency Gain: 1
   - Duration: 0.2

7. In CameraShake script:
   - Virtual Camera: Drag Cinemachine vcam
   - Impulse Source: Auto-filled
   - Light Shake: 0.5, 0.1
   - Medium Shake: 1.0, 0.2
   - Heavy Shake: 2.0, 0.3

### 8.2 Integrate with Combat

In damage dealing code:

```csharp
// After dealing damage
if (CameraShake.Instance != null)
{
    CameraShake.Instance.ShakeLight();
}

// On critical hit
if (isCritical && CameraShake.Instance != null)
{
    CameraShake.Instance.ShakeMedium();
}

// On parry
if (parrySuccess && CameraShake.Instance != null)
{
    CameraShake.Instance.ShakeMedium();
}
```

---

## 🧪 Step 9: Test Complete Stage 2

### Testing Checklist:

**Projectiles:**
- [ ] Right-click throws shuriken
- [ ] Shuriken damages enemies
- [ ] Shuriken has trail effect
- [ ] Shuriken destroys on hit
- [ ] Archer shoots arrows
- [ ] Arrows damage player

**Defense:**
- [ ] Hold Shift shows block VFX
- [ ] Blocking reduces damage
- [ ] Pressing Shift before hit = parry
- [ ] Parry stuns enemy
- [ ] Parry reflects projectiles back
- [ ] Reflected projectiles damage enemies

**Stances:**
- [ ] Press Q to change stance
- [ ] UI shows current stance
- [ ] Water stance feels faster
- [ ] Earth stance hits harder but slower
- [ ] Stance aura VFX appears

**New Enemies:**
- [ ] Archer maintains distance
- [ ] Archer shoots regularly
- [ ] Archer flees when too close
- [ ] Runner circles player
- [ ] Runner winds up (flashes) before dash
- [ ] Runner dashes through player
- [ ] Runner is harder to hit

**VFX:**
- [ ] Damage numbers appear on hits
- [ ] Critical hits show yellow numbers
- [ ] Camera shakes on impacts
- [ ] Bigger shakes for crits/parries

---

## 🎮 Step 10: Balance and Polish

### Recommended Tweaks:

**Player Balance:**
- If too easy: Reduce base damage to 12
- If too hard: Increase starting HP to 120
- Parry window too strict: Increase to 0.3s
- Projectiles weak: Increase damage to 10

**Enemy Balance:**
- Archer too passive: Reduce shoot cooldown to 1.5s
- Runner too fast: Reduce dash speed to 10
- Too many enemies: Reduce spawns in rooms

**Feel:**
- More screen shake: Increase shake intensities
- Less shake: Decrease or disable in settings
- Damage numbers too fast: Increase lifetime to 1.5s
- Stance switching too frequent: Increase cooldown to 1s

---

## ✅ Stage 2 Complete Checklist

- [ ] Projectile system works (shuriken, arrows)
- [ ] Parry reflects projectiles
- [ ] Block reduces damage
- [ ] Stances switch with Q
- [ ] Different feel between Water/Earth stance
- [ ] OniArcher fights at range
- [ ] YokaiRunner dashes aggressively
- [ ] Damage numbers appear
- [ ] Camera shakes on hits
- [ ] Combat feels much deeper than Stage 1
- [ ] No console errors
- [ ] 60 FPS maintained

---

## 🎯 Gameplay Feel Goals

After Stage 2, combat should feel:
- **Dynamic**: Switching stances mid-fight
- **Skillful**: Parrying projectiles is satisfying
- **Varied**: Different enemies require different tactics
- **Impactful**: Screen shake and VFX make hits feel powerful
- **Strategic**: Choosing when to block vs dodge vs parry

---

## 📈 Performance Check

- Profiler should show < 5ms CPU time
- Draw calls should be < 50
- Object pooling prevents GC spikes
- 60 FPS steady with 10+ enemies

---

## 🐛 Common Issues and Fixes

### Projectiles not damaging:
- Check Target Layers matches enemy layer
- Verify Collider2D is trigger
- Check IDamageable interface on target

### Parry not working:
- Verify parry window > 0
- Check Input for Shift key
- Ensure HealthComponent has damage events

### Stance not changing stats:
- StanceManager might need integration with PlayerController
- Check that GetModifiedStats() is being called
- Verify stance ScriptableObjects have correct multipliers

### Archer not shooting:
- Check Shoot Point exists and is positioned correctly
- Verify Arrow prefab is assigned
- Check projectile prefab has ProjectileController

### Runner not dashing:
- Verify dash range includes player
- Check dash cooldown isn't too high
- Ensure player layer is set correctly

---

**Estimated Time for Stage 2 Setup:** 4-6 hours

**Next: Stage 3 - Procedural Generation!**

**Last Updated:** 2025-11-12
