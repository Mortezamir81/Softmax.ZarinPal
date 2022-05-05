# Softmax.ZarinPal
Unofficial implementation of ZarinPal API for .NET 6

[![Nuget Version][nuget-shield]][nuget]
[![Nuget Downloads][nuget-shield-dl]][nuget]

## Installing the NuGet Package
You can install this package by entering the following command into your `Package Manager Console`:

```powershell
Install-Package Softmax.ZarinPal
```

*Note:* This package requires **.NET 6.0**.

## Use in ASP.NET Core 6
### Startup Configuration
Register `ZarinPalService` in your `Program.cs` class: 

```csharp
// using Softmax.ZarinPal;
// using Softmax.ZarinPal.Enums;

builder.Services.AddZarinPal(options =>
{
    // Required
    options.MerchantId = "your-merchant-id";
    
    // Optional (defualt: IRR)
    options.CurrencyType = CurrencyType.IRR;
    
    // Optional
    options.DefaultCallbackUri = new Uri("your-callback-uri");
});
```

*Note:* IRR is **Iran Rial** and IRT is **Iran Toman**.

### Controller Configuration
Then, you need to add `ZarinPalService` to your controller:

```csharp
// using Softmax.ZarinPal;

public class HomeController : Controller
{
    private readonly IZarinPalService _zarinpal;

    public HomeController(IZarinPalService zarinpal)
    {
        _zarinpal = zarinpal;
    }
}
```

### Payment
You can request a new payment by the `PaymentAsync` method.

- **Amount** is the transaction amount. (Required)
- **Description** description of the transaction. (Required)
- **Callback Url** the url to which the transaction should be redirected after completion. (Optional)

```csharp
[Route("send")]
public async Task<IActionResult> Send()
{
      // Defualt CurrencyType is IRR (Rial) , you can change it in service options
      long amount = 1000; // Required   
      string description = "This is a test payment"; // Required
      
      string callbackUrl = new Uri("https://localhost:5001/verify"); // Optional 
      string email = "your-email"; // Optional 
      string mobile = "your-mobie"; // Optional 
  
      var result = await _zarinPal.PaymentAsync(new PaymentRequest
      {
          CallbackUrl = callbackUrl,
          Description = description,
          Amount = amount,
          Email = email,
          Mobile = mobile,
      });
  
      if (result.IsSuccess())
      {
          return Redirect(result.Data.PaymentUri.AbsoluteUri);
      }
  
      return Ok($"Failed, Error code: {result.Error.Code}");
}
```

### Verify Payment
You can verify the payment transaction using the `VerifyAsync` method.

> This action is called when the **Callback Url** we defined earlier is called (when the transaction is completed)

```csharp
[Route("verify")]
public async Task<IActionResult> Verify()
{
    // Get Status and Authority and show error if not available.
    if (!Request.Query.TryGetValue("Status", out var status) ||
        !Request.Query.TryGetValue("Authority", out var authority))
    {
      return BadRequest();
    }
  
    long amount = 1000;
    var result = await _zarinPal.VerifyAsync(authority: authority, amount: amount);
  
    // Check if transaction was successful.
    if (result.IsSuccess())
    {
      return Ok($"Success, RefId: {result.Data.RefId}");
    }
  
    // Show unsuccessful transaction with code.
    return BadRequest($"Failed, Error code: {result.Error.Code}");
}
```

## Another .NET Platforms
### Payment
You can request a payment via `PaymentAsync` method.

- **Amount** is the transaction amount. (Required)
- **Description** description of the transaction. (Required)
- **Callback Url** the url to which the transaction should be redirected after completion. (Optional)

```csharp
using Softmax.ZarinPal;
using Softmax.ZarinPal.Enums;

namespace ZarinPal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IZarinPalService zarinPal = 
              new ZarinPalService(merchantId: "your-merchant-id", defaultCallbackUri: new Uri("your-callback-uri"), currencyType: CurrencyType.IRR);
              
            // Defualt CurrencyType is IRR (Rial) , you can change it in service options
            long amount = 1000; // Required   
            string description = "This is a test payment"; // Required
        
            string callbackUrl = new Uri("https://localhost:5001/verify"); // Optional 
            string email = "your-email"; // Optional 
            string mobile = "your-mobie"; // Optional 
        
            var result = await zarinPal.PaymentAsync(new PaymentRequest
            {
                CallbackUrl = callbackUrl,
                Description = description,
                Amount = amount,
                Email = email,
                Mobile = mobile,
            });
        
            if (result.IsSuccess())
            {
                Console.WriteLine(result.Data.PaymentUri.AbsoluteUri);
            }
        
            Console.WriteLine($"Failed, Error code: {result.Error.Code}");
        }
    }
}
```

### Verify Payment
You can verify the payment transaction using the `VerifyAsync` method.

```csharp
using Softmax.ZarinPal;
using Softmax.ZarinPal.Enums;

namespace ZarinPal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IZarinPalService zarinPal = 
              new ZarinPalService(merchantId: "your-merchant-id", defaultCallbackUri: new Uri("your-callback-uri"), currencyType: CurrencyType.IRR);
                  
            long amount = 1000;
            var result = await zarinPal.VerifyAsync(authority: "your-authority", amount: 1000);
          
            // Check if transaction was successful.
            if (result.IsSuccess())
            {
              Console.WriteLine($"Success, RefId: {result.Data.RefId}");
            }
          
            // Show unsuccessful transaction with code.
            Console.WriteLine($"Failed, Error code: {result.Error.Code}");
        }
    }
}
```

## Contact with me
- **Telegram**: @mortezamir81
- **Email**: mortezamirshekar81@gmail.com


## License
This project is licensed under the [MIT License](LICENSE).

[nuget]: https://www.nuget.org/packages/Softmax.ZarinPal
[nuget-shield]: https://img.shields.io/nuget/v/Softmax.ZarinPal?label=Release&color=blue
[nuget-shield-dl]: https://img.shields.io/nuget/dt/Softmax.ZarinPal?label=Downloads&color=red
