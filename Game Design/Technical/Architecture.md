# Technical Architecture

## Overview
The game is built with modular, maintainable code and optimized for mobile performance.

## Key Features
- Modular scripts (Unity C# recommended)
- ScriptableObjects for machines, products, levels
- Event-driven systems for timer, scoring, UI
- Use Unity's Input System for touch events

## Core Systems Architecture

### Grid System
- **GridManager.cs**: Core grid management system that creates a visual grid mesh and handles grid-based positioning
- **Node.cs**: Represents individual grid cells with state (Empty, Road, Building)
- Grid uses configurable cell size (default 1f) and supports camera clamping to grid boundaries

### Building System
- **BuildingType.cs**: ScriptableObject defining building templates with:
  - Production recipes (input/output products, production time)
  - Visual properties (prefab, icon, size)
  - Economic properties (build cost)
  - Storage capacity limits (inputStorageLimit, outputStorageLimit)
- **BaseBuilding.cs**: Abstract base class for all buildings:
  - Shared stock management (inputStock, outputStock dictionaries)
  - 3D text display system
  - Common building behavior and lifecycle
- **ProductionBuilding.cs**: Handles factories and farms:
  - Production timers and state machine
  - Capacity-aware production logic
  - Input consumption and output generation
- **Store.cs**: Handles customer-facing buildings:
  - Direct-to-shelf stock management
  - Customer interaction and transport requests

### Transport System Architecture

#### Core Components
- **TransportRequest**: Data class holding delivery order information
  - Requesting building reference
  - Product type and amount
  - Status tracking (Waiting → Processing → Delivered)
  - Priority and timing information

- **ActiveDelivery**: Tracks delivery phases with realistic timing
  - Pickup phase (5 seconds)
  - Delivery phase (10 seconds)
  - Building references (source and destination)
  - Status and progress tracking

- **TransportManager**: Central singleton coordinator
  - Resource reservation system
  - Game tick processing (3-second intervals)
  - Request matching and fulfillment
  - Performance optimization for mobile

#### Resource Management Flow
```
Producer Building → outputStock → reservedStock → Transport → inputStock → Consumer Building
```

#### Key Design Decisions
- **Automatic Transport System**: Inspired by Mini Metro/Mini Motorways
- **Resource Reservation**: Prevents race conditions and ensures delivery integrity
- **Two-Phase Delivery**: Realistic pickup time + delivery time simulation
- **Continuous Flow**: Multiple trucks can work on same order simultaneously
- **Game Tick Processing**: 3-second intervals for mobile performance optimization

#### Technical Implementation Details
- **Singleton Pattern**: TransportManager with proper inheritance
- **Dictionary-based Stock Management**: Efficient lookups with proper initialization
- **Status Tracking**: Comprehensive state management across delivery lifecycle
- **Mobile Optimization**: Batch processing and efficient update cycles
- **Debug Support**: Comprehensive logging system for development

### Integration Points
- **BuildingRequestComponent**: Seamless integration with building system
- **Grid-based Building System**: Compatible with existing placement mechanics
- **Visual System**: Ready for truck movement visualization expansion
- **Store Building Integration**: Prepared for customer interaction features

### Performance Considerations
- **Mobile-First Design**: Optimized for touch devices and limited processing power
- **Batch Processing**: Efficient update cycles to maintain frame rate
- **Memory Management**: Proper object lifecycle and garbage collection considerations
- **Scalability**: Architecture supports growing numbers of buildings and requests

### Visual Features
- **Real-time Status Updates**: Building text displays show "🚚 X truck(s) en route"
- **Debug-friendly Logging**: Comprehensive system for development and troubleshooting
- **Future-ready Visualization**: Architecture prepared for animated truck movement

Expand this file with diagrams and additional system details as development progresses. 