using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Softmax.ZarinPal.Other;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Softmax.ZarinPal.Enums;

namespace Softmax.ZarinPal
{
	public class ZarinPalService : IZarinPalService
	{
		#region Constractor
		public ZarinPalService
			(string merchantId = null,
			HttpClient httpClient = null,
			Uri defaultCallbackUri = null,
			IOptions<ZarinPalOptions> options = null,
			CurrencyType? currencyType = null) : base()
		{
			_httpClient =
				httpClient ?? new HttpClient();

			if (options != null)
			{
				_options =
					options.Value ?? throw new ArgumentNullException(nameof(options));

				if (string.IsNullOrWhiteSpace(options.Value.MerchantId))
				{
					throw new ArgumentException
						($"'{nameof(options.Value.MerchantId)}' has not been configured.", nameof(options));
				}

				if (options.Value.CurrencyType != null)
				{
					_currencyType = options.Value.CurrencyType.Value;
				}

				_merchantId = options.Value.MerchantId;
			}
			else
			{
				_merchantId =
					merchantId ?? throw new ArgumentException($"'{nameof(options.Value.MerchantId)}' has not been configured.", nameof(options));

				if (currencyType != null)
				{
					_currencyType = currencyType.Value;
				}
			}

			_defaultCallbackUri = defaultCallbackUri;
		}
		#endregion /Constractor

		#region Properties
		private string _merchantId { get; set; }
		private HttpClient _httpClient { get; set; }
		private Uri _defaultCallbackUri { get; set; }
		private ZarinPalOptions _options { get; set; }
		private CurrencyType _currencyType { get; set; } = CurrencyType.IRR;
		private string _verifyUri => "https://api.zarinpal.com/pg/v4/payment/verify.json";
		private string _paymentUri => "https://api.zarinpal.com/pg/v4/payment/request.json";
		#endregion /Properties

		#region Methods
		public async Task<PaymentResponse> PaymentAsync(PaymentRequest paymentRequest)
		{
			PaymentResponse returnValue = null;

			Uri actualCallbackUri = null;

			ArgumentNullException.ThrowIfNull(paymentRequest);

			if (paymentRequest.CallbackUrl != null)
			{
				actualCallbackUri = paymentRequest.CallbackUrl;
			}
			else
			{
				actualCallbackUri = _defaultCallbackUri is null
					? _options?.DefaultCallbackUri
					: _defaultCallbackUri;
			}

			if (actualCallbackUri == null)
				throw new ArgumentException($"The {nameof(paymentRequest.CallbackUrl)} parameter is null", nameof(paymentRequest.CallbackUrl));

			if (string.IsNullOrWhiteSpace(paymentRequest.Description))
				throw new ArgumentNullException(nameof(paymentRequest.Description));

			if (paymentRequest.Amount < 1000)
				throw new ArgumentException($"The {nameof(paymentRequest.Amount)} must be at least 1000", nameof(paymentRequest.Amount));

			var response = await _httpClient.PostAsJsonAsync(_paymentUri, new
			{
				callback_url = actualCallbackUri,
				description = paymentRequest.Description,
				amount = paymentRequest.Amount,
				merchant_id = _merchantId,
				mobile = paymentRequest.Mobile,
				email = paymentRequest.Email,
				currency = _currencyType == 0 ? "IRR" : "IRT",
			});

			var jsonText = await response.Content.ReadAsStringAsync();

			var result = JsonSerializer.Deserialize<JsonObject>(jsonText);

			var data = result["data"] as JsonObject;

			if (data != null)
			{
				var fee = (int)data["fee"];
				var code = (int)data["code"];
				var message = (string)data["message"];
				var feeType = (string)data["fee_type"];
				var authority = (string)data["authority"];


				returnValue = new PaymentResponse()
				{
					Data = new PaymentData()
					{
						Authority = authority,
						Code = code,
						Fee = fee,
						FeeType = feeType,
						Message = message,
					}
				};

				returnValue.Data.PaymentUri = new Uri($"https://www.zarinpal.com/pg/StartPay/{authority}");
			}
			else
			{
				var errors = result["errors"] as JsonObject;

				var code = (int)errors["code"];
				var message = (string)errors["message"];
				var validations = errors["validations"] as JsonArray;

				returnValue = new PaymentResponse()
				{
					Error = new Error()
					{
						Code = code,
						Message = message,
					}
				};

				if (validations != null && validations.Count > 0)
				{
					foreach (var validation in validations)
					{

						var validationKeyValue = (validation as JsonObject).FirstOrDefault();

						returnValue.Error.Validations.Add((string)validationKeyValue.Value);

					}
				}
			}

			return returnValue;
		}


		public async Task<VerifyResponse> VerifyAsync(string authority, long amount)
		{
			VerifyResponse returnValue = null;

			if (string.IsNullOrWhiteSpace(authority))
			{
				throw new ArgumentNullException(nameof(authority));
			}

			var response = await _httpClient.PostAsJsonAsync(_verifyUri, new
			{
				amount = amount,
				authority = authority,
				merchant_id = _merchantId,
			});

			var jsonText = await response.Content.ReadAsStringAsync();

			var result = JsonSerializer.Deserialize<JsonObject>(jsonText);

			var data = result["data"] as JsonObject;

			if (data != null)
			{
				var fee = (int)data["fee"];
				var code = (int)data["code"];
				var refId = (long)data["ref_id"];
				var message = (string)data["message"];
				var feeType = (string)data["fee_type"];
				var cardMask = (string)data["card_pan"];
				var cardHash = (string)data["card_hash"];


				returnValue = new VerifyResponse()
				{
					Data = new VerifyData()
					{
						Fee = fee,
						Code = code,
						refId = refId,
						Message = message,
						FeeType = feeType,
						CardMask = cardMask,
						CardHash = cardHash,
					}
				};
			}
			else
			{
				var errors = result["errors"] as JsonObject;

				var code = (int)errors["code"];
				var message = (string)errors["message"];
				var validations = errors["validations"] as JsonArray;

				returnValue = new VerifyResponse()
				{
					Error = new Error()
					{
						Code = code,
						Message = message,
					}
				};

				if (validations != null && validations.Count > 0)
				{
					foreach (var validation in validations)
					{

						var validationKeyValue = (validation as JsonObject).FirstOrDefault();

						returnValue.Error.Validations.Add((string)validationKeyValue.Value);

					}
				}
			}

			return returnValue;
		}
		#endregion /Methods
	}
}
