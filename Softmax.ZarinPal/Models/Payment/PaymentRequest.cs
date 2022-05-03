using System;

namespace Softmax.ZarinPal
{
    public class PaymentRequest
    {
		public long Amount { get; set; }

		public string Mobile { get; set; }

		public string Email { get; set; }

		public Uri CallbackUrl { get; set; }

		public string Description { get; set; }
	}
}
