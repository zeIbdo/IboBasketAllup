using Allup.Application.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Allup.MVC.Controllers
{
    public class BasketController : LocalizerController
    {
        private readonly IBasketService _basketService;

        public BasketController(ILanguageService languageService, IBasketService basketService) : base(languageService)
        {
            _basketService = basketService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int productId)
        {
            await _basketService.AddToBasketAsync(productId);
            var basketViewModel = await _basketService.GetBasketViewModelAsync();
            return Json(basketViewModel);
        }

        public async Task<IActionResult> Remove(int basketItemId)
        {
            await _basketService.RemoveFromBasketAsync(basketItemId);
            var basketViewModel = await _basketService.GetBasketViewModelAsync();
            return Json(basketViewModel);
        }
    }
}
