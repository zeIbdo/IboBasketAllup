using Allup.Application.Services.Abstracts;
using Allup.Application.UI.Services.Abstracts;
using Allup.Application.UI.ViewModels;
using Allup.Application.ViewModels;
using Allup.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Allup.Application.Services.Implementations;

public class BasketManager : IBasketService
{
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly ICookieService _cookieService;
	private readonly IBasketItemService _basketItemService;


	public BasketManager(IHttpContextAccessor contextAccessor, ICookieService cookieService, IBasketItemService basketItemService, UserManager<AppUser> userService)
	{
		_contextAccessor = contextAccessor;
		_cookieService = cookieService;
		_basketItemService = basketItemService;
	}

	public async Task<int> AddToBasketAsync(int productId)
	{
		string clientId = "";
		if (!_contextAccessor.HttpContext.User.Identity!.IsAuthenticated)
			 clientId = _cookieService.GetBrowserId();			
		else
			 clientId =  _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

		var basketCreateViewModel = new BasketItemCreateViewModel { ProductId = productId, ClientId = clientId, Count = 1 };
		await _basketItemService.CreateAsync(basketCreateViewModel);
		var count = (await _basketItemService.GetAllAsync(x => x.ClientId == clientId)).Count;
		return count;
	}

	public async Task<int> RemoveFromBasketAsync(int id)
	{
		string clientId = "";
		if (!_contextAccessor.HttpContext.User.Identity!.IsAuthenticated)
			clientId = _cookieService.GetBrowserId();
		else
			clientId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
		await _basketItemService.DeleteAsync(id);
		var count = (await _basketItemService.GetAllAsync(x => x.ClientId == clientId)).Count;
		return count;
	}
}
