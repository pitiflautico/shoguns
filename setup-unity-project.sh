#!/bin/bash

# Shogun's Legacy - Unity Project Setup Script
# This script creates the complete folder structure for the Unity project

echo "🗡️  SHOGUN'S LEGACY - Project Setup"
echo "===================================="
echo ""

# Check if we're in the right directory
if [ ! -f "ROADMAP.md" ]; then
    echo "❌ Error: ROADMAP.md not found. Please run this from the project root."
    exit 1
fi

echo "📁 Creating Unity folder structure..."

# Create main Unity project directory
mkdir -p ShogunsLegacy

# Create Assets folder structure
mkdir -p ShogunsLegacy/Assets/{Scenes,Scripts,Sprites,Animations,Prefabs,Audio,Shaders,ScriptableObjects,Materials,Fonts,Resources}

# Scripts subfolders
mkdir -p ShogunsLegacy/Assets/Scripts/{Player,Enemy,Combat,Generation,UI,Audio,Managers,Data,Interfaces,Utils}

# Sprites subfolders
mkdir -p ShogunsLegacy/Assets/Sprites/{Player,Enemies,Environment,Items,VFX,UI,Bosses}

# Animations subfolders
mkdir -p ShogunsLegacy/Assets/Animations/{Player,Enemies,UI,Bosses}

# Prefabs subfolders
mkdir -p ShogunsLegacy/Assets/Prefabs/{Player,Enemies,Combat,Environment,UI,Managers}

# Audio subfolders
mkdir -p ShogunsLegacy/Assets/Audio/{Music,SFX,Ambient}

# ScriptableObjects subfolders
mkdir -p ShogunsLegacy/Assets/ScriptableObjects/{Items,Enemies,Biomes,Abilities,Stats}

# ProjectSettings folder
mkdir -p ShogunsLegacy/ProjectSettings

# Packages folder
mkdir -p ShogunsLegacy/Packages

echo "✅ Folder structure created!"
echo ""
echo "📋 Created directories:"
echo "   - Assets/ (with all subfolders)"
echo "   - Scripts/ (organized by system)"
echo "   - Sprites/ (organized by category)"
echo "   - ProjectSettings/"
echo "   - Packages/"
echo ""
echo "✅ Next steps:"
echo "   1. Open Unity Hub"
echo "   2. Click 'Open' → Select 'ShogunsLegacy' folder"
echo "   3. Use Unity 2022.3 LTS (2D URP template)"
echo "   4. Run 'configure-unity-packages.sh' after Unity opens"
echo ""
