using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class TransportManager : Singleton<TransportManager>
    {
        private List<TransportRequest> pendingRequests = new();
        private List<ActiveDelivery> activeDeliveries = new();
        private float processingTimer = 0f;
        private float processingInterval = 3f;

        public float timeToPickup;
        public float timeToDeliver;

        protected override void Start()
        {
            base.Start();
        }

        private void FixedUpdate()
        {
            processingTimer += Time.fixedDeltaTime;

            if (processingTimer >= processingInterval)
            {
                processingTimer = 0f;

                if (pendingRequests.Count > 0)
                {
                    ProcessPendingRequest(pendingRequests[0]);
                    pendingRequests.RemoveAt(0);
                }
            }

            ProcessActiveDelivery(); // Keep this frequent for smooth timers
        }

        public void AddRequest(TransportRequest request)
        {

            request.requestStatus = RequestStatus.Waiting;
            var (supplier,availableAmount) = FindSupplier(request.requestingProduct);

            if (supplier != null)
            {
                int transactionAmount = Math.Min(request.requestingAmount, availableAmount);

                SchedulePickUp(supplier, request, transactionAmount);
                request.requestingAmount -= transactionAmount;
                if (request.requestingAmount > 0)
                {
                    pendingRequests.Add(request);
                }
            }
            else
            {
                pendingRequests.Add(request);
            }
        }

        void ProcessPendingRequest(TransportRequest request)
        {
            var (supplier,availableAmount) = FindSupplier(request.requestingProduct);
            if (supplier != null)
            {
                int transactionAmount = Math.Min(request.requestingAmount, availableAmount);
                SchedulePickUp(supplier, request, transactionAmount);
                request.requestingAmount -= transactionAmount;
            }
            if (request.requestingAmount > 0)
            {
                pendingRequests.Add(request);
            }

        }

        private void SchedulePickUp(BaseBuilding building, TransportRequest request, int amount)
        {
            if (building.outputStock.ContainsKey(request.requestingProduct))
            {
                building.outputStock[request.requestingProduct] -= amount;
            }
            if (building.reservedStock.ContainsKey(request.requestingProduct))
            {
                building.reservedStock[request.requestingProduct] += amount;
            }
            else
            {
                building.reservedStock[request.requestingProduct] = amount;
            }

            ActiveDelivery delivery = new ActiveDelivery(DeliveryPhase.Pickup,
                request, building, request.requestingBuilding,
                request.requestingProduct, amount, timeToPickup, timeToDeliver);
            activeDeliveries.Add(delivery);
        }

        void ProcessActiveDelivery()
        {
            for (int i = activeDeliveries.Count - 1; i >= 0; i--)
            {
                ActiveDelivery delivery = activeDeliveries[i];
                delivery.timeRemaining -= Time.fixedDeltaTime;

                if (delivery.timeRemaining <= 0)
                {
                    if (delivery.deliveryPhase == DeliveryPhase.Pickup)
                    {
                        delivery.deliveryPhase = DeliveryPhase.Delivery;
                        delivery.timeRemaining = timeToDeliver;
                        delivery.originalRequest.requestStatus = RequestStatus.Processing;
                        delivery.supplier.reservedStock[delivery.productData] -= delivery.amount;
                    }
                    else if (delivery.deliveryPhase == DeliveryPhase.Delivery)
                    {
                        CompleteDelivery(delivery);
                        activeDeliveries.RemoveAt(i);
                    }
                }


            }
        }


        void CompleteDelivery(ActiveDelivery delivery)
        {
            if (delivery.requester.buildingData.buildingRole == BuildingRole.Store)
            {
                // AddStoreStock will trigger OnProductDelivered event
                ((Store) delivery.requester).AddStoreStock(delivery.productData, delivery.amount);
            }
            else if (delivery.requester.buildingData.buildingRole == BuildingRole.Factory)
            {
                delivery.requester.inputStock[delivery.productData] += delivery.amount;
                // Trigger the delivery event to notify request components
                delivery.requester.OnProductDelivered?.Invoke(delivery.productData, delivery.amount);
            }
            
            delivery.originalRequest.requestStatus = RequestStatus.Delivered;
        }

        private (BaseBuilding supplier, int availableAmount) FindSupplier(ProductData requestingProduct)
        {
            BaseBuilding bestSupplier = null;
            int highestAmount = 0;
            foreach (var producer in GameManager.Instance.producerBuilding)
            {
                producer.outputStock.TryGetValue(requestingProduct, out int productAmount);
                if (productAmount > highestAmount)
                {
                    bestSupplier = producer;
                    highestAmount = productAmount;
                }
            }

            foreach (var factory in GameManager.Instance.factoryBuilding)
            {
                factory.outputStock.TryGetValue(requestingProduct, out int productAmount);
                if (productAmount > highestAmount)
                {
                    bestSupplier = factory;
                    highestAmount = productAmount;
                }
            }

            return (bestSupplier, highestAmount);

        }

        public List<ActiveDelivery> GetActiveDeliveries()
        {
            return activeDeliveries;
        }
        
        /// <summary>
        /// Gets the current pending requests for debug UI purposes
        /// </summary>
        public List<TransportRequest> GetPendingRequests()
        {
            return pendingRequests;
        }
    }
}