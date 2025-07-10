# Truck Logistics

## Overview
Trucks are the backbone of the logistics network, transporting goods between farms, factories, and the store.

## Transport Request System (Demand-Driven)

### Core Design Philosophy
The logistics system is **demand-driven**: stores create requests when they need products, and factories only send products when there's a specific request. This prevents overproduction and creates a realistic supply chain.

### Request Creation (Stores)
- **Stock Monitoring:** Stores check their inventory at regular intervals (every 5-10 seconds)
- **Request Trigger:** When stock drops to 25% of capacity, store creates a transport request
- **Request Amount:** Store requests enough products to fill to 100% capacity
- **Example:** Store capacity = 20, current stock = 4 → Request 16 items

### Production & Fulfillment (Factories)
- **Continuous Production:** Factories produce continuously until their stock is full
- **Request-Based Sending:** Factories only send products when there's an incoming request
- **No Overproduction:** This prevents waste and maintains game balance

### Building Capacities
- **Different Buildings:** Each building type has different storage capacities
- **Capacity Settings:** Defined in BuildingType ScriptableObject
- **Store vs Factory:** May have different input/output capacity limits

## Transport Control System (Automatic)

### Design Philosophy: Simple Mobile Experience
Inspired by Mini Metro and Mini Motorways, the transport system emphasizes **infrastructure optimization** over vehicle micromanagement.

### Player Responsibilities
- **Road Network:** Build and optimize road layouts for efficient transport
- **Building Placement:** Strategic positioning of farms, factories, and stores
- **Fleet Upgrades:** Purchase more trucks, upgrade speed/capacity globally
- **Economic Management:** Balance road costs vs. transport efficiency

### System Responsibilities
- **Automatic Routing:** System finds optimal paths using available roads
- **Request Fulfillment:** Trucks automatically assigned to transport requests
- **Delivery Execution:** No manual "send truck A to building B" required
- **Load Balancing:** System distributes trucks efficiently across active requests

### Truck Mechanics
- **Autonomous Operation:** Trucks automatically navigate road networks
- **Global Fleet:** All trucks share capacity, no individual assignment
- **Upgradeable Stats:** Speed, capacity, and count improvements affect entire fleet
- **Smart Routing:** System chooses shortest/most efficient available paths

### Strategic Depth
- **Layout Optimization:** Efficient road networks reduce transport time and costs
- **Capacity Planning:** Balance truck count vs. upgrade costs vs. demand
- **Network Effects:** Better road connectivity improves overall system efficiency
- **Economic Trade-offs:** Direct expensive routes vs. longer cheaper alternatives

## Mobile Considerations
- **Simple Interface:** No complex truck assignment UI required
- **Tap to Upgrade:** Global fleet improvements through simple upgrade menus
- **Visual Feedback:** Clear indicators for transport efficiency and bottlenecks
- **Intuitive Controls:** Focus on road building and building placement

## Implementation Status

### ✅ Completed Systems

#### Transport Request System (Phase 1)
- **TransportRequest Class**: Core data structure for delivery orders
  - Building reference, product type, amount, status tracking
  - Priority and timing information for efficient processing
  - Status flow: Waiting → Processing → Delivered

- **ActiveDelivery System**: Two-phase delivery simulation
  - **Pickup Phase**: 5-second realistic loading time
  - **Delivery Phase**: 10-second transport and unloading time
  - Multiple trucks can work on same order simultaneously for scalability

- **TransportManager**: Central coordination system
  - Singleton pattern for global transport management
  - 3-second game tick processing for mobile optimization
  - Resource reservation system prevents race conditions
  - Automatic request matching and fulfillment

#### Resource Management Flow
```
Producer Building → outputStock → reservedStock → Transport → inputStock → Consumer Building
```

- **Reservation System**: Prevents overselling and ensures delivery integrity
- **Dictionary-based Stock Management**: Efficient lookups with proper initialization
- **Continuous Flow Design**: Multiple concurrent deliveries supported

#### Integration Features
- **BuildingRequestComponent Integration**: Seamless connection to building system
- **Visual Feedback**: Buildings display "🚚 X truck(s) en route" status
- **Debug Logging**: Comprehensive system for development and troubleshooting
- **Mobile Optimization**: Batch processing and efficient update cycles

### 🔄 Next Implementation Phases

#### Phase 2: Visual Truck Movement
- **Basic Pathfinding** - A* or BFS for road navigation
- **Vehicle System** - Visual trucks moving along roads
- **Route Optimization** - Efficient path selection algorithms

#### Phase 3: Advanced Features
- **Traffic System Integration** - Road capacity and congestion
- **Fleet Management** - Multiple truck types and upgrades
- **Route Efficiency Metrics** - Performance analytics and optimization

#### Phase 4: Polish and Optimization
- **Advanced Pathfinding** - Dynamic routing and traffic awareness
- **Visual Polish** - Smooth animations and particle effects
- **Performance Scaling** - Large-scale logistics network support

### Technical Implementation Notes

#### Key Design Decisions
- **Automatic Transport System**: Inspired by Mini Metro/Mini Motorways
- **Resource Reservation**: Prevents race conditions using outputStock → reservedStock → inputStock flow
- **Two-Phase Delivery**: Realistic pickup time + delivery time simulation
- **Continuous Flow**: Multiple trucks can work on same order simultaneously
- **Game Tick Processing**: 3-second intervals for mobile performance optimization

#### Performance Considerations
- **Mobile-First Design**: Optimized for touch devices and limited processing power
- **Batch Processing**: Efficient update cycles to maintain target frame rate
- **Memory Management**: Proper object lifecycle and garbage collection awareness
- **Scalability**: Architecture supports growing numbers of buildings and requests

#### Integration Points
- **Grid System Compatibility**: Works with existing grid-based building placement
- **Building System Integration**: Seamless connection to production and consumption
- **Future-Ready Architecture**: Prepared for visual truck movement and traffic systems
- **Store Building Support**: Ready for customer interaction features

See related files for road building and production chain details. 