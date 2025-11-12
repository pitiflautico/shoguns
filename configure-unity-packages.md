# Unity Package Configuration Guide

After opening the Unity project, follow these steps to install required packages:

## 📦 Required Packages Installation

### 1. Open Package Manager
`Window > Package Manager`

### 2. Install the following packages:

#### Essential Packages (Required)
- [ ] **Input System** (com.unity.inputsystem)
  - Version: 1.7.0 or higher
  - Unity Registry → Search "Input System" → Install
  - ⚠️ Will prompt to restart Unity - accept it

- [ ] **Cinemachine** (com.unity.cinemachine)
  - Version: 2.9.0 or higher
  - Unity Registry → Search "Cinemachine" → Install

- [ ] **2D Animation** (com.unity.2d.animation)
  - Version: 9.0.0 or higher
  - Unity Registry → Search "2D Animation" → Install

- [ ] **2D Sprite** (com.unity.2d.sprite)
  - Usually pre-installed in 2D projects

- [ ] **2D Tilemap Editor** (com.unity.2d.tilemap)
  - Usually pre-installed in 2D projects

- [ ] **TextMeshPro** (com.unity.textmeshpro)
  - Usually pre-installed
  - On first use, import "TMP Essential Resources"

#### Recommended Packages (Optional but useful)
- [ ] **Universal RP** (com.unity.render-pipelines.universal)
  - For better 2D lighting and post-processing
  - Version: 14.0.0 or higher (for Unity 2022.3)

- [ ] **Shader Graph** (com.unity.shadergraph)
  - For custom shader effects
  - Version: 14.0.0 or higher

- [ ] **2D Pixel Perfect** (com.unity.2d.pixel-perfect)
  - For crisp pixel art rendering
  - Version: 5.0.0 or higher

- [ ] **ProBuilder** (com.unity.probuilder)
  - Optional: for quick prototyping of collision geometry

---

## ⚙️ Project Settings Configuration

### 3. Configure Input System
1. `Edit > Project Settings > Player`
2. Under "Other Settings" find "Active Input Handling"
3. Select "Both" (allows old and new input system)
4. Restart Unity when prompted

### 4. Configure Quality Settings
1. `Edit > Project Settings > Quality`
2. Set default quality to "High"
3. Enable VSync for target platforms

### 5. Configure Physics 2D
1. `Edit > Project Settings > Physics 2D`
2. Set up collision matrix:
   - Create layers: Player, Enemy, Projectile, Ground, Walls, Items
3. Configure collision interactions (we'll detail this in Stage 1)

### 6. Configure Tags and Layers
1. `Edit > Project Settings > Tags and Layers`
2. Add layers:
   - Layer 6: `Player`
   - Layer 7: `Enemy`
   - Layer 8: `Projectile`
   - Layer 9: `Ground`
   - Layer 10: `Walls`
   - Layer 11: `Items`
   - Layer 12: `Pickups`
   - Layer 13: `VFX`

3. Add tags:
   - `Player`
   - `Enemy`
   - `Boss`
   - `Projectile`
   - `Item`
   - `Pickup`

---

## 🎨 Universal Render Pipeline Setup (if using URP)

### 7. Create URP Asset
1. `Assets > Create > Rendering > URP Asset (with 2D Renderer)`
2. Name it "ShogunsLegacy_URPAsset"
3. Go to `Edit > Project Settings > Graphics`
4. Assign the URP Asset to "Scriptable Render Pipeline Settings"

### 8. Configure 2D Renderer
1. Select the 2D Renderer Data asset created with URP
2. Enable "Post Processing"
3. Add Renderer Features later for effects

---

## 📝 Verify Installation Checklist

After completing the above:

- [ ] Can create Input Actions asset
- [ ] Cinemachine menu appears in GameObject menu
- [ ] Can create 2D sprite animations
- [ ] TextMeshPro is available in UI
- [ ] All layers and tags are configured
- [ ] No console errors related to packages

---

## 🚀 Next Steps

Once packages are installed:
1. Run the base scripts creation: `create-base-scripts.sh`
2. Start implementing Stage 1 tasks
3. Begin generating AI assets for sprites

---

## 📚 Documentation Links

- [Input System Docs](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Cinemachine Docs](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/index.html)
- [2D Animation Docs](https://docs.unity3d.com/Packages/com.unity.2d.animation@9.0/manual/index.html)
- [URP Docs](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/index.html)
