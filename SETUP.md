# 🗡️ SHOGUN'S LEGACY - Setup Guide

Complete setup instructions for getting the project ready for development.

---

## ⚙️ Prerequisites

### Software Requirements
- [ ] **Unity Hub** (latest version)
- [ ] **Unity 2022.3 LTS** or higher
- [ ] **Git** (for version control)
- [ ] **Code Editor**: Visual Studio, VS Code, or Rider
- [ ] **Aseprite** or similar pixel art tool (for refining AI-generated sprites)

### Recommended AI Tools (for asset generation)
- [ ] **Stable Diffusion** (local) or **Midjourney** (subscription)
- [ ] **AIVA** or **Soundraw** (music generation)
- [ ] **Audacity** (free audio editing)

---

## 📦 Step 1: Project Structure Setup

### 1.1 Run Setup Script

```bash
# Make the script executable
chmod +x setup-unity-project.sh

# Run the setup script
./setup-unity-project.sh
```

This will create:
- `ShogunsLegacy/` - Main Unity project folder
- Complete folder structure inside `Assets/`
- All necessary subfolders organized by type

### 1.2 Verify Folder Structure

```
ShogunsLegacy/
├── Assets/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Player/
│   │   ├── Enemy/
│   │   ├── Combat/
│   │   ├── Generation/
│   │   ├── UI/
│   │   ├── Audio/
│   │   ├── Managers/
│   │   ├── Data/
│   │   ├── Interfaces/
│   │   └── Utils/
│   ├── Sprites/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Environment/
│   │   ├── Items/
│   │   ├── VFX/
│   │   ├── UI/
│   │   └── Bosses/
│   ├── Animations/
│   ├── Prefabs/
│   ├── Audio/
│   ├── Shaders/
│   ├── ScriptableObjects/
│   └── Resources/
├── ProjectSettings/
└── Packages/
```

---

## 🎮 Step 2: Unity Project Creation

### 2.1 Create Unity Project

1. Open **Unity Hub**
2. Click **"New Project"**
3. Select **2D (URP)** template
4. Project Name: `ShogunsLegacy`
5. Location: Select the `ShogunsLegacy/` folder created by the setup script
6. Click **"Create Project"**

### 2.2 Wait for Unity to Initialize

Unity will:
- Import default packages
- Set up the 2D URP renderer
- Create initial folders

This may take 5-10 minutes.

---

## 📚 Step 3: Install Unity Packages

Follow the guide in `configure-unity-packages.md`:

### Required Packages
- [ ] Input System (1.7.0+)
- [ ] Cinemachine (2.9.0+)
- [ ] 2D Animation (9.0.0+)
- [ ] TextMeshPro (pre-installed)

### Recommended Packages
- [ ] Universal RP (14.0.0+)
- [ ] Shader Graph (14.0.0+)
- [ ] 2D Pixel Perfect (5.0.0+)

**⚠️ Important:** When installing Input System, Unity will prompt to restart. Accept it.

---

## 🏗️ Step 4: Import Base Scripts

### 4.1 Copy Scripts to Unity Project

```bash
# Copy all base scripts to Unity Assets
cp -r UnityScripts/* ShogunsLegacy/Assets/Scripts/
```

### 4.2 Verify Scripts in Unity

In Unity Editor:
1. Check `Assets/Scripts/` folder
2. Wait for compilation (check bottom-right corner)
3. Fix any compilation errors (should be none)

### 4.3 Scripts Included

✅ **Interfaces:**
- `IDamageable.cs` - Interface for anything that can take damage

✅ **Data:**
- `HealthComponent.cs` - Generic health system
- `GameStats.cs` - ScriptableObject for entity stats

✅ **Managers:**
- `GameManager.cs` - Main game state manager (Singleton)
- `InputManager.cs` - Centralized input handling

✅ **Combat:**
- `DamageDealer.cs` - Component for dealing damage

✅ **Utils:**
- `ObjectPooler.cs` - Performance optimization for spawning objects

---

## ⚙️ Step 5: Configure Unity Settings

### 5.1 Project Settings - Tags and Layers

1. `Edit > Project Settings > Tags and Layers`
2. Add the following layers:
   - Layer 6: `Player`
   - Layer 7: `Enemy`
   - Layer 8: `Projectile`
   - Layer 9: `Ground`
   - Layer 10: `Walls`
   - Layer 11: `Items`
   - Layer 12: `Pickups`
   - Layer 13: `VFX`

3. Add the following tags:
   - `Player`
   - `Enemy`
   - `Boss`
   - `Projectile`
   - `Item`
   - `Pickup`

### 5.2 Project Settings - Physics 2D

1. `Edit > Project Settings > Physics 2D`
2. Configure Collision Matrix (which layers collide):
   - Player ↔ Enemy ✅
   - Player ↔ Walls ✅
   - Player ↔ Items ✅
   - Enemy ↔ Walls ✅
   - Projectile ↔ Enemy ✅ (player projectiles)
   - Projectile ↔ Player ✅ (enemy projectiles)
   - Projectile ↔ Walls ✅
   - Projectile ↔ Projectile ❌
   - Player ↔ Player ❌
   - Enemy ↔ Enemy ❌

### 5.3 Project Settings - Quality

1. `Edit > Project Settings > Quality`
2. Set default quality to **"High"**
3. Enable **VSync** for smooth framerate

### 5.4 Project Settings - Input System

1. `Edit > Project Settings > Player`
2. Scroll to **"Active Input Handling"**
3. Select **"Both"** (allows both old and new input system)
4. **Restart Unity** when prompted

---

## 🎨 Step 6: Create Initial Scenes

### 6.1 Create Main Scenes

1. In Project window: `Assets/Scenes/`
2. Create the following scenes (Right-click → Create → Scene):
   - `MainMenu.unity`
   - `Hub.unity`
   - `Gameplay.unity`
   - `TestRoom.unity` (for testing)

### 6.2 Set Up Build Settings

1. `File > Build Settings`
2. Add scenes in this order:
   1. MainMenu
   2. Hub
   3. Gameplay
3. Click **"Add Open Scenes"** for each
4. Set Platform to **PC, Mac & Linux Standalone**

---

## 🎯 Step 7: Create Essential GameObjects

### 7.1 Create Managers Scene

1. Create new scene: `_Managers.unity` in `Assets/Scenes/`
2. In Hierarchy, create Empty GameObjects:
   - `GameManager` - Add `GameManager.cs` script
   - `InputManager` - Add `InputManager.cs` script
   - `ObjectPooler` - Add `ObjectPooler.cs` script

3. **Mark these as DontDestroyOnLoad** (already done in scripts)

### 7.2 Create Input Actions Asset

1. In Project: `Assets/` → Right-click → Create → Input Actions
2. Name it: `PlayerInputActions`
3. Double-click to open Input Actions window
4. Create Action Map: **"Gameplay"**
5. Add Actions:
   - `Move` - Action Type: Value, Control Type: Vector2
   - `Aim` - Action Type: Value, Control Type: Vector2
   - `Attack` - Action Type: Button
   - `SecondaryAttack` - Action Type: Button
   - `Dash` - Action Type: Button
   - `Interact` - Action Type: Button
   - `Pause` - Action Type: Button

6. Bind controls:
   - Move: WASD / Left Stick (Gamepad)
   - Aim: Mouse Position / Right Stick (Gamepad)
   - Attack: Mouse Left Click / West Button (Gamepad)
   - SecondaryAttack: Mouse Right Click / East Button (Gamepad)
   - Dash: Space / South Button (Gamepad)
   - Interact: E / North Button (Gamepad)
   - Pause: Escape / Start Button (Gamepad)

7. Click **"Save Asset"**
8. Click **"Generate C# Class"** - Name it `PlayerInputActions`

### 7.3 Connect Input to InputManager

1. Select `InputManager` GameObject
2. Add `Player Input` component
3. Set Actions to `PlayerInputActions`
4. Set Behavior to **"Invoke Unity Events"**
5. In Events section, link each action to corresponding `InputManager` method:
   - Move → `InputManager.OnMove`
   - Attack → `InputManager.OnAttack`
   - etc.

---

## ✅ Step 8: Verify Setup

### 8.1 Test Compilation

1. Check Console (bottom of Unity) - should be **0 errors**
2. If warnings appear, they're usually safe to ignore for now

### 8.2 Test Scene Load

1. Open `TestRoom` scene
2. Add a simple Sprite (2D Object → Sprites → Square)
3. Press **Play** ▶️
4. Game should run without errors
5. Press **Escape** - should exit play mode

### 8.3 Test Input

1. In `TestRoom` scene, select `InputManager` in Hierarchy
2. Press **Play** ▶️
3. In Inspector, watch `InputManager` variables change when you:
   - Press WASD (MoveInput should change)
   - Move mouse (MouseWorldPosition should update)
   - Click mouse (AttackPressed should flash true)

---

## 📝 Step 9: Version Control Setup

### 9.1 Initialize Git (if not already done)

```bash
# In project root (where ROADMAP.md is)
git add .
git commit -m "Stage 0: Unity project setup complete"
git push
```

### 9.2 Verify .gitignore

Make sure `.gitignore` is working:
- `Library/` folder should NOT be tracked
- `Temp/` folder should NOT be tracked
- Only `Assets/`, `ProjectSettings/`, `Packages/` are tracked

---

## 🎨 Step 10: Prepare for AI Asset Generation

See `AI_PROMPTS.md` for detailed prompts to generate initial assets.

You should now generate:
- [ ] Player sprite sheet
- [ ] Basic enemy (Oni) sprite sheet
- [ ] Basic tileset (16x16 tiles)
- [ ] Basic VFX (slash, hit impact)

---

## ✅ Setup Complete!

You're now ready to start **Stage 1: MVP Core - Combate Básico**!

### Next Steps:
1. Generate initial sprites using AI (see `AI_PROMPTS.md`)
2. Import sprites to `Assets/Sprites/`
3. Start implementing player movement (Stage 1.1)
4. Update `ROADMAP.md` and check off Stage 0 tasks

### Debug Shortcuts (from GameManager):
- **F1** - Load Main Menu
- **F2** - Load Hub
- **F3** - Start New Run

---

## 🆘 Troubleshooting

### Input System Not Working
- Verify you restarted Unity after installing Input System
- Check `Project Settings > Player > Active Input Handling` is set to "Both"

### Scripts Not Compiling
- Check for missing using statements
- Verify all scripts are in correct folders
- Close and reopen Unity

### Can't Find Packages
- Open Package Manager: `Window > Package Manager`
- Change dropdown from "In Project" to "Unity Registry"
- Search for package name

### Performance Issues in Editor
- Disable "Maximize on Play"
- Reduce Game View resolution
- Close unnecessary windows

---

**Setup Time:** ~1-2 hours (including package downloads)

**Last Updated:** 2025-11-12
