using Allup.Application.Services.Abstracts;
using Allup.Application.UI.Services.Abstracts;
using Allup.Application.UI.Services.Implementations;
using Allup.Application.UI.ViewModels;
using Allup.Application.ViewModels;
using Allup.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace Allup.Application.Services.Implementations;

public class BasketManager : IBasketService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ICookieService _cookieService;
    private readonly IBasketItemService _basketItemService;
    private readonly ExternalApiService _externalApiService;


    public BasketManager(IHttpContextAccessor contextAccessor, ICookieService cookieService, IBasketItemService basketItemService, UserManager<AppUser> userService, ExternalApiService externalApiService)
    {
        _contextAccessor = contextAccessor;
        _cookieService = cookieService;
        _basketItemService = basketItemService;
        _externalApiService = externalApiService;
    }

    public async Task<int> AddToBasketAsync(int productId)
    {
        string clientId = "";
        if (!_contextAccessor.HttpContext.User.Identity!.IsAuthenticated)
            clientId = _cookieService.GetBrowserId();
        else
            clientId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var basketCreateViewModel = new BasketItemCreateViewModel { ProductId = productId, ClientId = clientId, Count = 1 };
        await _basketItemService.CreateAsync(basketCreateViewModel);
        var count = (await _basketItemService.GetAllAsync(x => x.ClientId == clientId)).Count;
        return count;
    }

    public async Task<BasketViewModel> GetBasketViewModelAsync()
    {
        var language = await _cookieService.GetLanguageAsync();
        string clientId = "";

        if (!_contextAccessor.HttpContext.User.Identity!.IsAuthenticated)
             clientId = _cookieService.GetBrowserId();
        else
             clientId =_contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items = await _basketItemService.GetAllAsync(x => x.ClientId == clientId, include: x => x.Include(y => y.Product!).ThenInclude(x => x.ProductTranslations!.Where(x => x.LanguageId == language.Id)));
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
        basketViewModel.FormattedTotalAmount = totalAmount;
        return basketViewModel;
    }

    public async Task<int> RemoveFromBasketAsync(int id)
    {
        string clientId = "";
        if (!_contextAccessor.HttpContext.User.Identity!.IsAuthenticated)
            clientId = _cookieService.GetBrowserId();
        else
            clientId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        await _basketItemService.DeleteAsync(x=>x.Id==id && x.ClientId==clientId);
        var count = (await _basketItemService.GetAllAsync(x => x.ClientId == clientId)).Count;
        return count;
    }
}
