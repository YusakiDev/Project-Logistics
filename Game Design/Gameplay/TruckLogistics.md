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

## Mechanics
- **Autonomous Movement:** Trucks follow built roads automatically.
- **Attributes:** Each truck has speed, capacity, and upgradeable stats.
- **Fleet Management:** Players can purchase new trucks and upgrade existing ones.
- **Pickup/Delivery:** Trucks pick up goods at production nodes and deliver to the next destination.

## Mobile Considerations
- Tap to select and upgrade trucks
- Visual feedback for truck actions
- Haptic feedback for upgrades or deliveries

## Implementation Priority
1. **Transport Request System** - Foundation for all logistics
2. **Basic Pathfinding** - A* or BFS for road navigation
3. **Vehicle System** - Visual trucks moving along roads
4. **Delivery Matching** - Connect producers with consumers

See related files for road building and production chain details. 