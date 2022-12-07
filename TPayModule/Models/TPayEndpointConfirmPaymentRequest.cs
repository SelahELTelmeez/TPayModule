namespace TPayModule.Models
{
    public class TPayEndpointConfirmPaymentRequest
    {
        public string Signature { get; set; }
        public string PinCode { get; set; }
        public string TransactionId { get; set; }
    }
}
