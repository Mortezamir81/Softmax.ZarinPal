using Softmax.ZarinPal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softmax.ZarinPal.Other
{
	public class ZarinPalOptions
	{
		public string MerchantId { get; set; }

		public CurrencyType? CurrencyType { get; set; }

		public Uri DefaultCallbackUri { get; set; }
	}
}
