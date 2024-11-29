using Allup.Domain.Entities;

namespace Allup.Application.ViewModels
{
	public class BasketItemViewModel
	{
		public int Id { get; set; }
		public string ClientId { get; set; } = null!;
		public required int ProductId { get; set; }
		public ProductViewModel? Product { get; set; }
		public int Count { get; set; }
	}
	public class BasketItemCreateViewModel
	{
		public string ClientId { get; set; } = null!;
		public required int ProductId { get; set; }
		public int Count { get; set; }
	}
	public class BasketItemUpdateViewModel
	{
		public int Id { get; set; }
		public string ClientId { get; set; } = null!;
		public required int ProductId { get; set; }
		public int Count { get; set; }
	}
}
