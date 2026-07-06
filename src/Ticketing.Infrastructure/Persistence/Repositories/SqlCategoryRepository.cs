namespace Ticketing.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Abstractions;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Persistence;

public sealed class SqlCategoryRepository : ICategoryRepository
{
    private readonly TicketingSqlDbContext _dbContext;

    public SqlCategoryRepository(TicketingSqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<Category> InsertAsync(Category category, CancellationToken ct)
    {
        await _dbContext.Categories.AddAsync(category, ct);
        await _dbContext.SaveChangesAsync(ct);

        return category;
    }

    public async Task<bool> UpdateAsync(Category category, CancellationToken ct)
    {
        _dbContext.Categories.Update(category);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (category is null)
        {
            return false;
        }

        _dbContext.Categories.Remove(category);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .AnyAsync(x => x.Name == name, ct);
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
