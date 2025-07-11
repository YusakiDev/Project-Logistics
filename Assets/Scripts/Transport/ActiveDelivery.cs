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
        public float timeToPickup;
        public float timeToDeliver;

        public ActiveDelivery(DeliveryPhase deliveryPhase,TransportRequest originalRequest, BaseBuilding supplier, BaseBuilding requester,
            ProductData productData, int amount, float timeToPickup, float timeToDeliver)
        {
            this.originalRequest = originalRequest;
            this.deliveryPhase = deliveryPhase;
            this.supplier = supplier;
            this.requester = requester;
            this.productData = productData;
            this.amount = amount;
            this.timeToPickup = timeToPickup;
            this.timeToDeliver = timeToDeliver;
            timeRemaining = timeToPickup;
        }

    }

    public enum DeliveryPhase
    {
        Pickup,
        Delivery,

    }
}