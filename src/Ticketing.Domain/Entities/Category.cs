namespace Ticketing.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
