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

## Implementation Priority
1. **Transport Request System** - Foundation for all logistics
2. **Basic Pathfinding** - A* or BFS for road navigation
3. **Vehicle System** - Visual trucks moving along roads
4. **Delivery Matching** - Connect producers with consumers

See related files for road building and production chain details. 