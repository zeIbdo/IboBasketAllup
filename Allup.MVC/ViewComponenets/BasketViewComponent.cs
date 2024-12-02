using Allup.Application.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Allup.MVC.ViewComponenets
{
    public class BasketViewComponent : ViewComponent
    {

        private readonly IBasketService _basketService;

        public BasketViewComponent(IBasketService basketService)
        {

            _basketService = basketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basketViewModel = await _basketService.GetBasketViewModelAsync();
            return View(basketViewModel);
        }
    }
}
