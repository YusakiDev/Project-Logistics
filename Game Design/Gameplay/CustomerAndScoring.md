# Customer & Scoring System

## Overview
Customers are simulated artificially through timed stock consumption at stores. Players must maintain adequate stock levels to satisfy customer demand and maintain store ratings.

## Mechanics
- **Artificial Customers:** Customers arrive at random intervals and consume stock automatically
- **Stock Consumption:** Each customer reduces store stock by 1 unit of available products
- **Waiting Queue:** When stock is empty, customers wait in a queue with patience timers
- **Patience System:** Customers wait up to 30 seconds before leaving if no stock is available
- **Store Rating:** Based on customer satisfaction - successful service vs. customers who leave
- **Level Completion:** Calculated on overall store performance and customer satisfaction

## Customer Flow
1. **Customer Arrival:** Random intervals (configurable per level)
2. **Stock Check:** If product available, customer purchases and leaves satisfied
3. **Stock Empty:** Customer joins waiting queue with 30-second patience timer
4. **Queue Management:** First-in-first-out when stock becomes available
5. **Patience Timeout:** Customer leaves after 30 seconds, reducing store rating

## Store Rating System
- **Positive Impact:** Successful customer service (stock available when customer arrives)
- **Negative Impact:** Customer leaves due to empty stock (waited > 30 seconds)
- **Rating Calculation:** Ratio of satisfied customers to total customers
- **Level Scoring:** Store ratings aggregated at level completion

## Configuration Parameters
- **Customer Arrival Rate:** Interval between customer arrivals (randomized)
- **Patience Time:** How long customers wait (default: 30 seconds)
- **Rating Impact:** How much each lost customer affects store rating
- **Demand Patterns:** Different arrival rates for different levels

## Mobile Considerations
- **Visual Feedback:** Simple counters showing waiting customers
- **Store Status:** Clear indicators for stock levels and customer satisfaction
- **Minimal UI:** No complex customer management - focus on logistics optimization

## Technical Implementation
- Each store tracks its own customer queue and rating
- Timer-based system for customer arrivals and patience
- Integration with existing transport and stock management systems
- Configurable parameters for level designers

See related files for economy and UI details. 