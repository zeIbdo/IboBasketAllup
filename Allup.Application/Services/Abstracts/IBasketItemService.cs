using Allup.Application.ViewModels;
using Allup.Domain.Entities;
using System.Linq.Expressions;

namespace Allup.Application.Services.Abstracts;

public interface IBasketItemService : ICrudService<BasketItemViewModel, BasketItem, BasketItemCreateViewModel>
{

	Task<BasketItemViewModel> UpdateAsync(BasketItemUpdateViewModel item);
	Task<BasketItemViewModel> DeleteAsync(Expression<Func<BasketItem, bool>> predicate);

}