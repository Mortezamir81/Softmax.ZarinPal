using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softmax.ZarinPal
{
	public class Error
	{
		public Error()
		{
			Validations = new List<string>();
		}

		public int Code { get; set; }

		public string Message { get; set; }

		public IList<string> Validations { get; set; }
	}
}
