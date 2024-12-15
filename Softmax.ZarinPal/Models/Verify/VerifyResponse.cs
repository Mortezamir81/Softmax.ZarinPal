using System;
using System.Collections.Generic;

namespace Softmax.ZarinPal
{
	public class VerifyResponse
	{
		public VerifyData Data { get; set; }

		public Error Error { get; set; }
	}

	public class VerifyData
	{
		public int Fee { get; set; }

		public int Code { get; set; }

		public long refId { get; set; }

		public string Message { get; set; }

		public string FeeType { get; set; }

		public string CardMask { get; set; }

		public string CardHash { get; set; }
	}
}
