# Road & Route Building

## Overview
The game uses a hybrid road system: each map features pre-placed, uneditable main roads and rivers, while players can build extensions from these main routes. Some map tiles are unbuildable terrain, adding further challenge and variety.

## Map Structure
- **Scripted Main Roads/Rivers:**
  - Pre-placed, uneditable main roads and rivers form the backbone of each map.
  - Main roads connect major locations (e.g., farm, factory, store).
  - Rivers act as obstacles; bridges (special buildable tiles) are required to cross.
- **Player-Buildable Extensions:**
  - Players can build roads branching from main roads, within constraints (budget, space, allowed tiles).
  - Extensions allow for optimization, shortcuts, and creative solutions.
- **Unbuildable Terrain:**
  - Certain tiles (e.g., mountains, forests, water) cannot be built on.
  - Forces players to plan routes around obstacles and use main roads strategically.

## Traffic System
- Each road segment has a traffic capacity (number of trucks per time unit).
- Congestion occurs if too many trucks use the same segment: trucks slow down, deliveries are delayed.
- Main roads have higher capacity; player-built roads may be narrower/cheaper but more prone to congestion.
- Bridges may have unique traffic rules or limits.
- Players can upgrade certain roads to increase capacity (optional feature).

## Strategic Implications
- Players must balance shortest routes with traffic flow and terrain constraints.
- Incentivizes building alternate routes, upgrading roads, or timing deliveries.
- Unbuildable terrain and rivers create unique map challenges and replayability.

See related files for truck logistics, level design, and UI details. 