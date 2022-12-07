using System.Net.Http.Json;
using TPayModule.Models;

namespace TPayModule.HttpClients;

public class TPayClient
{
    private readonly HttpClient _httpClient;

    private readonly string baseUrl = "https://live.TPAY.me/";
    private readonly string publicKey = "8Nienx8EIwNS97e51JAS";
    private readonly string privateKey = "vKaDscWr6W4B8cMVUCpw";

    public TPayClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<TPayInitializerResponse?> InitializerAsync(TPayInitializerRequest request, Product product, CancellationToken cancellationToken = default)
    {
        string _orderInfo = string.Format("{0}:{1}", DateTime.UtcNow.ToString("yyyyMMddHH:mm:ss"), request.UserId);

        string signature = GenerateInitializerSignature(request, product, _orderInfo);

        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/TPay.svc/json/InitializePremiumDirectPaymentTransaction", new TPayEndpointInitializerRequest
        {
            OrderInfo = _orderInfo,
            Language = request.Language,
            MSISDN = request.MSISDN,
            OperatorCode = request.OperatorCode,
            ProductName = product.Name,
            ProductPrice = (double)product.Price,
            Signature = signature
        }, cancellationToken: cancellationToken);

        return await httpResponse.Content.ReadFromJsonAsync<TPayInitializerResponse>(cancellationToken: cancellationToken);
    }

    public async Task<TPayEndpointConfirmPaymentResponse?> ConfirmPaymentAsync(string PinCode, string TransactionId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/TPAY.svc/Json/ConfirmDirectPaymentTransaction", new TPayEndpointConfirmPaymentRequest
        {
            PinCode = PinCode,
            TransactionId = TransactionId,
            Signature = GenerateConfirmationSignature(TransactionId, PinCode)
        }, cancellationToken: cancellationToken);


        return await httpResponse.Content.ReadFromJsonAsync<TPayEndpointConfirmPaymentResponse>(cancellationToken: cancellationToken);
    }

    public async Task<TPayEndpointResendPinCodeResponse?> ResendPinCodeAsync(string TransactionId, int Language, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/TPAY.svc/Json/ResendVerificationPin", new TPayEndpointResendPinCodeRequest
        {
            TransactionId = TransactionId,
            Signature = GenerateResendPinCodeSignature(TransactionId, Language)
        }, cancellationToken: cancellationToken);

        return await httpResponse.Content.ReadFromJsonAsync<TPayEndpointResendPinCodeResponse>(cancellationToken: cancellationToken);

    }


    private string GenerateInitializerSignature(TPayInitializerRequest content, Product product, string OrderInfo)
    {
        string contentToHash = string.Format("{0}{1}{2}{3}{4}{5}", product.Name, (double)product.Price, content.MSISDN, content.OperatorCode, OrderInfo, content.Language);
        var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(privateKey));
        var correctHash = string.Join(string.Empty, hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contentToHash)).Select(b => b.ToString("x2")));
        return publicKey + ":" + correctHash;
    }
    private string GenerateConfirmationSignature(string TransactionId, string PinCode)
    {
        string contentToHash = string.Format("{0}{1}", TransactionId, PinCode);
        var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(privateKey));
        var correctHash = string.Join(string.Empty, hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contentToHash)).Select(b => b.ToString("x2")));
        return publicKey + ":" + correctHash;
    }
    private string GenerateResendPinCodeSignature(string TransactionId, int Language)
    {
        string contentToHash = string.Format("{0}{1}", TransactionId, Language);
        var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(privateKey));
        var correctHash = string.Join(string.Empty, hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contentToHash)).Select(b => b.ToString("x2")));
        return publicKey + ":" + correctHash;
    }
}
