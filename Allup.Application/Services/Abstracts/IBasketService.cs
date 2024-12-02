using Allup.Application.UI.ViewModels;

namespace Allup.Application.Services.Abstracts;

public interface IBasketService
{
    Task<int> AddToBasketAsync(int productId);
    Task<int> RemoveFromBasketAsync(int productId);
    Task<BasketViewModel> GetBasketViewModelAsync();
}
