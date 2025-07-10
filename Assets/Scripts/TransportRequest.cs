using System;
using System.Collections;
using UnityEngine;

namespace Scripts
{
    public class TransportRequest
    {
        public BaseBuilding requestingBuilding;
        public ProductData requestingProduct;
        public int requestingAmount;

        public RequestStatus requestStatus;
        public string uuid;


        public TransportRequest (BaseBuilding requestingBuilding, ProductData requestingProduct, int requestingAmount)
        {
            this.requestingBuilding = requestingBuilding;
            this.requestingProduct = requestingProduct;
            this.requestingAmount = requestingAmount;
            this.requestStatus = RequestStatus.Waiting;
            this.uuid = Guid.NewGuid().ToString();
        }

    }

    public enum RequestStatus
    {
        Waiting,
        Processing,
        Delivered,
    }
}