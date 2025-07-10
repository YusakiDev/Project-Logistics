using System;
using UnityEngine;

namespace Scripts
{
    public class ActiveDelivery
    {
        public TransportRequest originalRequest;
        public DeliveryPhase deliveryPhase;
        public float timeRemaining;
        public BaseBuilding supplier;
        public BaseBuilding requester;
        public ProductData productData;
        public int amount;
        private float timeToPickup;
        private float timeToDeliver;

        public ActiveDelivery(DeliveryPhase deliveryPhase,TransportRequest originalRequest, BaseBuilding supplier, BaseBuilding requester,
            ProductData productData, int amount)
        {
            this.originalRequest = originalRequest;
            this.deliveryPhase = deliveryPhase;
            this.supplier = supplier;
            this.requester = requester;
            this.productData = productData;
            this.amount = amount;
            this.timeToPickup = 5;
            this.timeToDeliver = 10;
            if (deliveryPhase == DeliveryPhase.Pickup)
            {
                timeRemaining = timeToPickup;
            }
            else if (deliveryPhase == DeliveryPhase.Delivery)
            {
                timeRemaining = timeToDeliver;
            }
        }

    }

    public enum DeliveryPhase
    {
        Pickup,
        Delivery,

    }
}