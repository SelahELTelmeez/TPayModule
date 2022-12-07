﻿namespace TPayModule.Models
{
    public class TPayEndpointResendPinCodeRequest
    {
        public string TransactionId { get; set; }
        public int Language { get; set; } = 2;
        public string Signature { get; set; }
    }
}
