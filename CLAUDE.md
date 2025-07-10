# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity-based logistics/farming simulation game called "Project Logistics". The game features a grid-based building system where players place buildings that produce and consume resources, connected by roads for transportation.

**Project Data Location**: D:\Project Data\Game Design\Project Logistics
- Contains game design documents, assets, and planning materials
- Separate from Unity project code repository

## Core Architecture

### Grid System
- **GridManager.cs**: Core grid management system that creates a visual grid mesh and handles grid-based positioning
- **Node.cs**: Represents individual grid cells with state (Empty, Road, Building)
- Grid uses configurable cell size (default 1f) and supports camera clamping to grid boundaries

### Building System
- **BuildingType.cs**: ScriptableObject defining building templates with:
  - Production recipes (input/output products, production time)
  - Visual properties (prefab, icon, size)
  - Economic properties (build cost)
- **BuildingInstance.cs**: Runtime component managing individual building behavior:
  - Production timers and state machine
  - Input/output stock management
  - 3D text display for debugging stock levels
  - Experimental delivery system
- **BuildingGridAligner.cs**: Handles automatic grid snapping and building scaling in editor

### Product System
- **ProductType.cs**: ScriptableObject defining individual products (name, icon)
- **InputProduct**: Struct defining production requirements (product type + amount)
- Buildings consume inputs instantly when production starts, then produce output after timer completes

### Infrastructure
- **RoadBuilder.cs**: Handles road placement with touch/mouse input
- **CameraDrag.cs**: Implements pan camera controls for both editor and mobile
- **CameraZoom.cs**: Handles zoom controls (scroll wheel in editor, pinch on mobile)

### Utility Systems
- **GameManager.cs**: Singleton pattern, manages global settings and screen orientation
- **GridSnap.cs**: Editor-only component for snapping objects to grid during design
- **FPSDisplay.cs**: Performance monitoring component

## Key Dependencies

The project uses several Unity packages and external tools:
- **TextMeshPro**: For UI text rendering
- **Sirenix Odin Inspector**: Advanced inspector attributes (ShowIf, etc.)
- **Easy Save 3**: Save system (extensive plugin presence)
- **Hot Reload**: Development tool for faster iteration
- **Input System**: Modern Unity input handling
- Various editor enhancement tools (vFolders, vHierarchy, etc.)

## Development Commands

This is a standard Unity project. Common development tasks:
- **Build**: Use Unity's Build Settings (File > Build Settings)
- **Test**: Unity Test Runner (Window > General > Test Runner)
- **Run**: Press Play button in Unity Editor or build to target platform

## Code Conventions

- Uses C# with Unity-specific patterns
- ScriptableObjects for data definitions (BuildingType, ProductType)
- MonoBehaviour components for runtime behavior
- Experimental features controlled via boolean flags
- Debug information displayed via GameManager.Instance.debugMode
- Platform-specific code using #if UNITY_EDITOR and #else blocks
- Grid coordinates use integer x,y while world positions use Vector3

## Important Notes

- Grid system expects buildings to be placed at integer grid coordinates
- Production system uses immediate input consumption followed by timed output generation
- Camera movement is constrained to grid boundaries
- Building instances auto-create 3D text displays for stock information
- Project supports both editor mouse input and mobile touch controls
- Many components use ExecuteAlways attribute for editor-time functionality