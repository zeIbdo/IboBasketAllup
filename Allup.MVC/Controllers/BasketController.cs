using Allup.Application.Services.Abstracts;
using Allup.Application.UI.Services.Abstracts;
using Allup.Application.UI.Services.Implementations;
using Allup.Application.UI.ViewModels;
using Allup.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;

namespace Allup.MVC.Controllers
{
    public class BasketController : LocalizerController
    {
        private readonly IProductService _productService;
        private readonly IBasketService _basketService;
        private readonly IBasketItemService _basketItemService;
        private readonly ICookieService _cookieService;
        private readonly ExternalApiService _externalApiService;

        public BasketController(IProductService productService, ILanguageService languageService, IBasketService basketService, IBasketItemService basketItemService, ICookieService cookieService, ExternalApiService externalApiService) : base(languageService)
        {
            _productService = productService;
            _basketService = basketService;
            _basketItemService = basketItemService;
            _cookieService = cookieService;
            _externalApiService = externalApiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int productId)
        {
            #region oldCode
            //var basket = Request.Cookies["basket"];
            //var basketViewModel = new BasketViewModel();
            //var basketCookieViewModels = new List<BasketCookieViewModel>();
            //var basketItemViewModels = new List<BasketItemViewModel>();

            //var languageId = await GetLanguageAsync();

            //if (string.IsNullOrEmpty(basket))
            //{
            //    basketCookieViewModels.Add(new BasketCookieViewModel
            //    {
            //        Count = 1,
            //        ProductId = productId
            //    });
            //}
            //else
            //{
            //    basketCookieViewModels = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(basket) ?? [];

            //    if (basketCookieViewModels.Any(x => x.ProductId == productId))
            //    {
            //        var existBasketItem = basketCookieViewModels.Find(x => x.ProductId == productId);
            //        existBasketItem!.Count++;
            //    }
            //    else
            //    {
            //        basketCookieViewModels.Add(new BasketCookieViewModel
            //        {
            //            Count = 1,
            //            ProductId = productId
            //        });
            //    }    
            //}

            //Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketCookieViewModels));

            //foreach (var item in basketCookieViewModels ?? [])
            //{
            //    var existBasketItem = await _productService.GetAsync(x => x.Id == item.ProductId,
            //        x => x.Include(y => y.ProductTranslations!.Where(z => z.LanguageId == languageId)));

            //    if (existBasketItem == null) continue;

            //    basketItemViewModels.Add(new BasketItemViewModel
            //    {
            //        ProductId = existBasketItem.Id,
            //        Name = existBasketItem.Name,
            //        CoverImageUrl = existBasketItem.CoverImageUrl,
            //        Price = existBasketItem.Price,
            //        Count = item.Count
            //    });
            //}

            ////var totalAmount = basketItemViewModels.Sum(x => x.Price * x.Count);

            //basketViewModel.Items = basketItemViewModels;
            ////basketViewModel.TotalAmount = totalAmount; 
            #endregion
            var languageId = await GetLanguageAsync();
            var count = await _basketService.AddToBasketAsync(productId);
            string clientId = "";
            if(!User.Identity!.IsAuthenticated)
                clientId = _cookieService.GetBrowserId();
            else 
                clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var items = await _basketItemService.GetAllAsync(x=>x.ClientId==clientId,include:x=>x.Include(y=>y.Product!).ThenInclude(z=>z.ProductTranslations!.Where(x => x.LanguageId==languageId)));
			var basketProducts = new List<BasketProductViewModel>();
            foreach (var item in items)
            {
                var basketProduct = new BasketProductViewModel
                {
                    BasketItemId=item.Id,
                    Count = item.Count,
                    ProductId = productId,
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
            return Json(basketViewModel);
        }

        public async Task<IActionResult> Remove(int basketItemId)
        {
			#region oldCode
			//var basket = Request.Cookies["basket"];
			//var basketViewModel = new BasketViewModel();
			//var basketItemViewModels = new List<BasketItemViewModel>();
			//var languageId = await GetLanguageAsync();

			//if (string.IsNullOrEmpty(basket))
			//{
			//    return BadRequest();
			//}

			//var basketCookieViewModels = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(basket);

			//var existProduct = basketCookieViewModels.Find(x => x.ProductId == productId);

			//if (existProduct == null)
			//    return BadRequest();

			//basketCookieViewModels.Remove(existProduct);
			//Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketCookieViewModels));

			//foreach (var item in basketCookieViewModels ?? [])
			//{
			//    var existBasketItem = await _productService.GetAsync(x => x.Id == item.ProductId,
			//        x => x.Include(y => y.ProductTranslations!.Where(z => z.LanguageId == languageId)));

			//    if (existBasketItem == null) continue;

			//    basketItemViewModels.Add(new BasketItemViewModel
			//    {
			//        ProductId = existBasketItem.Id,
			//        Name = existBasketItem.Name,
			//        CoverImageUrl = existBasketItem.CoverImageUrl,
			//        Price = existBasketItem.Price,
			//        Count = item.Count
			//    });
			//}

			////var totalAmount = basketItemViewModels.Sum(x => x.Price * x.Count);

			//basketViewModel.Items = basketItemViewModels;
			////basketViewModel.TotalAmount = totalAmount; 
			#endregion
			var languageId = await GetLanguageAsync();
			var count = await _basketService.RemoveFromBasketAsync(basketItemId);
			string clientId = "";
			if (!User.Identity!.IsAuthenticated)
				clientId = _cookieService.GetBrowserId();
			else
				clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			var items = await _basketItemService.GetAllAsync(x => x.ClientId == clientId, include: x => x.Include(y => y.Product!).ThenInclude(z => z.ProductTranslations!.Where(x => x.LanguageId == languageId)));
			var basketProducts = new List<BasketProductViewModel>();
			foreach (var item in items)
			{
				var basketProduct = new BasketProductViewModel
				{
                    BasketItemId=item.Id,
					Count = item.Count,
					ProductId = item.Product!.Id,
					Name = item.Product.Name,
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
            return Json(basketViewModel);
        }
    }
}
