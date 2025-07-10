using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class TransportManager : Singleton<TransportManager>
    {
        private List<TransportRequest> pendingRequests = new();
        private void FixedUpdate()
        {
            if (pendingRequests.Count > 0)
            {
                ProcessPendingRequest(pendingRequests[0]);
                pendingRequests.RemoveAt(0);
            }
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

            //timer I guess
            Pickup(building, request, amount);
        }

        void Pickup(BaseBuilding building, TransportRequest request, int amount)
        {
            request.requestStatus = RequestStatus.Processing;

            building.reservedStock[request.requestingProduct] -= amount;
            //load onto trucks stuff later
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


    }
}