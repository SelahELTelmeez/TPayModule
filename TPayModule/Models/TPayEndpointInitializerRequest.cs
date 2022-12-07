﻿namespace TPayModule.Models
{
    public class TPayEndpointInitializerRequest
    {
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public string OperatorCode { get; set; }
        public string MSISDN { get; set; }
        public int Language { get; set; }
        public string OrderInfo { get; set; }
        public string Signature { get; set; }
    }
}
