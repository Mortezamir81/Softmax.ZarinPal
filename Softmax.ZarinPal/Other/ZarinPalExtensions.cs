﻿using Microsoft.Extensions.DependencyInjection;
using Softmax.ZarinPal.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softmax.ZarinPal
{
	public static class ZarinPalExtensions
	{
        public static void AddZarinPal(this IServiceCollection services, Action<ZarinPalOptions> options)
        {
            services.Configure(options);
            services.AddHttpClient<IZarinPalService, ZarinPalService>();
        }
    }
}
