namespace Ticketing.Application.Abstractions;

using System;
using System.Collections.Generic;
using System.Text;
using Ticketing.Domain.Entities;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct);

    Task<Category> InsertAsync(Category category, CancellationToken ct);

    Task<bool> UpdateAsync(Category category, CancellationToken ct);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task<bool> ExistsByNameAsync(string name,  CancellationToken ct);
}
