using Org.BouncyCastle.Asn1.X9;
using Shopping_Laptop.Models;
using Shopping_Laptop.Models.Momo;

namespace Shopping_Laptop.Services.Momo
{
    public interface IMomoService
    {
            Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfo model);
            MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
