using System;
using lostborn_backend.Models;
namespace lostborn_web_API.Services
{
    public class MockPaymentService
    {
        public PaymentResponse ProcessPayment(PaymentRequest request)
        {
            // Simulate payment processing
            var random = new Random();
            bool isSuccess = random.Next(100) < 95; // 95% success rate..
            //We're not doing anything with request...
            if (isSuccess)
            {
                return new PaymentResponse
                {
                    IsSuccess = isSuccess,
                    TransactionId = Guid.NewGuid().ToString(),
                    ErrorMessage = "Success"
                };
            }
            else
            {
                return new PaymentResponse
                {
                    IsSuccess = isSuccess,
                    TransactionId = null,
                    ErrorMessage = "Payment unsuccessful due to insufficient funds."
                };
            }


        }
    }





}

