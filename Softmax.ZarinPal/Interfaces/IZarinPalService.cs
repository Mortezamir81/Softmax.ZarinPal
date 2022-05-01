using System.Threading.Tasks;

namespace Softmax.ZarinPal
{
    public interface IZarinPalService
    {
        Task<VerifyResponse> VerifyAsync(string authority, int amount);

        Task<PaymentResponse> PaymentAsync(PaymentRequest paymentRequest);
    }
}
