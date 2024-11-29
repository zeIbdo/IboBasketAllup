using Core.Persistence.Repositories;

namespace Allup.Domain.Entities
{
	public class BasketItem : Entity
	{
		public string ClientId { get; set; } = null!;
		public required int ProductId { get; set; }
		public Product? Product { get; set; }
		public DateTime? AddedTime { get; set; }
        public int Count { get; set; }

    }
}
