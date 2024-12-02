using Allup.Application.Services.Abstracts;
using Allup.Application.UI.Services.Abstracts;
using Allup.Application.UI.Services.Implementations;
using Allup.Application.UI.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Allup.MVC.ViewComponenets
{
    public class BasketViewComponent :  ViewComponent
    {
        private readonly IProductService _productService;
        private readonly ILanguageService _languageService;
		private readonly ICookieService _cookieService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IBasketItemService _basketItemService;
		private readonly ExternalApiService _externalApiService;

		public BasketViewComponent(IProductService productService, ILanguageService languageService, ICookieService cookieService, IHttpContextAccessor httpContextAccessor, IBasketItemService basketItemService, ExternalApiService externalApiService)
		{
			_productService = productService;
			_languageService = languageService;
			_cookieService = cookieService;
			_httpContextAccessor = httpContextAccessor;
			_basketItemService = basketItemService;
			_externalApiService = externalApiService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
        {
			#region oldCode
			//var basket = Request.Cookies["basket"];
			//var basketViewModel = new BasketViewModel();
			//var basketItemViewModels = new List<BasketItemViewModel>();

			//if (string.IsNullOrEmpty(basket))
			//{
			//    return View(basketViewModel);
			//}

			//var basketCookieViewModels = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(basket);


			//var culture = Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
			//var isoCode = culture?.Substring(culture.LastIndexOf("=") + 1) ?? "en-Us";
			//var selectedLanguage = await _languageService.GetAsync(x => x.IsoCode == isoCode);

			//foreach (var item in basketCookieViewModels ?? [])
			//{
			//    var existBasketItem = await _productService.GetAsync(x => x.Id == item.ProductId, 
			//        x => x.Include(y => y.ProductTranslations!.Where(z => z.LanguageId == selectedLanguage.Id)));

			//    if (existBasketItem == null) continue;

			//    basketItemViewModels.Add(new BasketItemViewModel
			//    {
			//        ProductId = existBasketItem.Id,
			//        Name = existBasketItem.Name,
			//        Price = existBasketItem.Price,
			//        CoverImageUrl = existBasketItem?.CoverImageUrl,
			//        FormattedPrice = existBasketItem?.FormattedPrice,
			//        Count = item.Count
			//    });
			//}

			////var totalAmount = basketItemViewModels.Sum(x => x.Price * x.Count);

			//basketViewModel.Items = basketItemViewModels;
			////basketViewModel.TotalAmount = totalAmount;
			//TempData["Count"] = basketViewModel.Count; 
			#endregion
			string clientId = "";
			if (!User.Identity!.IsAuthenticated)
				clientId = _cookieService.GetBrowserId();
			else
				clientId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			var items = await _basketItemService.GetAllAsync(x => x.ClientId == clientId, include: x => x.Include(y => y.Product!));
			var basketProducts = new List<BasketProductViewModel>();
			foreach (var item in items)
			{
				var basketProduct = new BasketProductViewModel
				{
					BasketItemId = item.Id,
					Count = item.Count,
					ProductId = item.ProductId,
					Name = item.Product!.Name,
					Price = item.Product.Price,
					CoverImageUrl = item.Product.CoverImageUrl,
					FormattedPrice = item.Product.FormattedPrice,
				};
				basketProducts.Add(basketProduct);
			}
			var basketViewModel = new BasketViewModel() { Items = basketProducts };
			var currency = await _cookieService.GetCurrencyAsync();

			var coefficient = await _externalApiService.GetCurrencyCoefficient(currency.CurrencyCode ?? "azn");
			var culture = new CultureInfo(currency.IsoCode ?? "az-az");

			var totalAmount = (basketViewModel.TotalAmount / coefficient).ToString("C", culture);
			//ViewData["TotalAmount"] = totalAmount;
			basketViewModel.FormattedTotalAmount = totalAmount;
			return View(basketViewModel);
        }
    }
}
