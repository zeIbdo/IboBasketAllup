using Allup.Application.Services.Abstracts;
using Allup.Application.UI.Services.Abstracts;
using Allup.Application.UI.Services.Implementations;
using Allup.Application.ViewModels;
using Allup.Domain.Entities;
using Allup.Persistence.Context;
using AutoMapper;
using Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using System.Globalization;
using System.Linq.Expressions;

namespace Allup.Application.Services.Implementations
{
	public class BasketItemManager : CrudManager<BasketItemViewModel, BasketItem, BasketItemCreateViewModel>, IBasketItemService
    {
        private readonly EfRepositoryBase<BasketItem, AppDbContext> _repository;
		private readonly ICookieService _cookieService;
		private readonly ExternalApiService _externalApiService;

		public BasketItemManager(EfRepositoryBase<BasketItem, AppDbContext> repository, IMapper mapper, ICookieService cookieService, ExternalApiService externalApiService) : base(repository, mapper)
		{
			_repository = repository;
			_cookieService = cookieService;
			_externalApiService = externalApiService;
		}

		public override async Task<BasketItemViewModel> CreateAsync(BasketItemCreateViewModel createViewModel)
		{
			var existItem = await _repository.GetAsync(x=>x.ClientId== createViewModel.ClientId && x.ProductId==createViewModel.ProductId);
			if(existItem != null)
			{
				existItem.Count++;
				await _repository.UpdateAsync(existItem);
				return new BasketItemViewModel() { ProductId=createViewModel.ProductId};
			}
			return await base.CreateAsync(createViewModel);
		}

		public async Task<BasketItemViewModel> DeleteAsync(int id)
		{
			var entity = await _repository.GetAsync(id);
		
			var deletedEntity = await _repository.DeleteAsync(entity);
			return Mapper.Map<BasketItemViewModel>(deletedEntity);
		}

		public override async Task<List<BasketItemViewModel>> GetAllAsync(Expression<Func<BasketItem, bool>>? predicate = null, Func<IQueryable<BasketItem>, IOrderedQueryable<BasketItem>>? orderBy = null, Func<IQueryable<BasketItem>, IIncludableQueryable<BasketItem, object>>? include = null)
		{
			var basketListItems = await base.GetAllAsync(predicate, orderBy, include);

			var currency = await _cookieService.GetCurrencyAsync();

			var coefficient = await _externalApiService.GetCurrencyCoefficient(currency.CurrencyCode ?? "azn");
			var culture = new CultureInfo(currency.IsoCode ?? "az-az");

			foreach (var item in basketListItems)
			{
				if (item.Product == null) continue;

				item.Product.FormattedPrice = (item.Product.Price / coefficient).ToString("C", culture);
			}

			return basketListItems;
		}

		public async Task<BasketItemViewModel> UpdateAsync(BasketItemUpdateViewModel item)
		{
			var entity = Mapper.Map<BasketItem>(item);
			var updatedEntity = await _repository.UpdateAsync(entity);
			return Mapper.Map<BasketItemViewModel>(updatedEntity);	
		}
	}
}
