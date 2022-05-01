using System;

namespace Softmax.ZarinPal
{
	public class PaymentResponse
	{
		public PaymentData Data { get; set; }

		public Error Error { get; set; }
	}

	public class PaymentData
	{
		public int Fee { get; set; }

		public int Code { get; set; }

		public string Message { get; set; }

		public string FeeType { get; set; }

		public string Authority { get; set; }

		public Uri PaymentUri { get; internal set; }
	}
}
