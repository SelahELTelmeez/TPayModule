using System.Text.Json;
using TPayModule.HttpClients;
using TPayModule.Models;

namespace TPayModule
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TPayClient tpayClient = new TPayClient(new HttpClient());

            Console.WriteLine("Enter Operation Id");
            Console.WriteLine("1- Initialize Payment");
            Console.WriteLine("2- Confirm Payment");

            int operationId = int.Parse(Console.ReadLine());

            if (operationId == 1)
            {
                await InitializerAsync(tpayClient);

                Console.WriteLine("===================================================  Confirm Payment =============================================");

                await ConfirmAsync(tpayClient);
            }
            else
            {
                await ConfirmAsync(tpayClient);
            }
        }

        public static async Task InitializerAsync(TPayClient tpayClient)
        {
            Console.WriteLine("Enter User Id or leave it empty for default");

            string userId = Console.ReadLine() ?? "aead1060-db16-4f12-b1ac-373d4a3a15d6";

            Console.WriteLine("Enter MobileNumber");

            string MobileNumber = Console.ReadLine();

            Console.WriteLine("Enter Operator Code");

            string OperatorCode = Console.ReadLine();

            Console.WriteLine("Enter Language 1- English OR 2- Arabic");

            int language = int.Parse(Console.ReadLine());

            TPayInitializerResponse? response = await tpayClient.InitializerAsync(new TPayInitializerRequest
            {
                UserId = userId,
                Language = language,
                MobileNumber = MobileNumber,
                OperatorCode = OperatorCode,
                ProductId = 1,
            }, new Product
            {
                Name = "JoySchool",
                Price = 1.0m
            });

            Console.WriteLine(JsonSerializer.Serialize(response));
        }

        public static async Task ConfirmAsync(TPayClient tpayClient)
        {
            Console.WriteLine("Enter Pin Code");

            string pinCode = Console.ReadLine();

            Console.WriteLine("Enter Transaction Id");

            string transactionId = Console.ReadLine();

            TPayEndpointConfirmPaymentResponse confirmResponse = await tpayClient.ConfirmPaymentAsync(pinCode, transactionId);

            Console.WriteLine(JsonSerializer.Serialize(confirmResponse));
        }
    }
}