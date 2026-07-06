namespace Ticketing.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Ticketing.Api;
using Ticketing.Application.Abstractions;
using Ticketing.Domain.Entities;
using Ticketing.Shared.Contracts.Categories;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repository;

    public CategoriesController(ICategoryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(Resource.CategoryIdRequiredMessage);
        }

        var category = await this._repository.GetByIdAsync(id, ct);

        return category == null ? NotFound() : Ok(ToResponse(category));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll(CancellationToken ct)
    {
        var items = await this._repository.GetAllAsync(ct);

        return Ok(items.Select(ToResponse));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CategoryCreateRequest category, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(category?.Name))
        {
            return this.BadRequest(Resource.CagetoryMustHaveNameMessage);
        }

        if (await this._repository.ExistsByNameAsync(category.Name, ct))
        {
            return this.Conflict(Resource.CategoryExistsMessage);
        }

        var created = await _repository.InsertAsync(
                                            new Category
                                            {
                                                Name = category.Name,
                                                Description = category.Description,
                                            },
                                            ct);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateRequest category, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(Resource.CategoryIdRequiredMessage);
        }

        if (string.IsNullOrWhiteSpace(category?.Name))
        {
            return BadRequest(Resource.CagetoryMustHaveNameMessage);
        }

        var result = await _repository.UpdateAsync(new Category { Id = id, Name = category.Name, Description = category.Description }, ct);

        return result ? NoContent() : NotFound();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<CategoryResponse>> Patch(
    Guid id,
    [FromBody] CategoryUpdateRequest request,
    CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(Resource.CategoryIdRequiredMessage);
        }

        var category = await _repository.GetByIdAsync(id, ct);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(category, ct);

        var response = new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(Resource.CategoryIdRequiredMessage);
        }

        var result = await _repository.DeleteAsync(id, ct);

        return result ? NoContent() : NotFound();
    }

    private static CategoryResponse ToResponse(Category c)
    {
        return new(c.Id, c.Name, c.Description, c.CreatedAtUtc, c.UpdatedAtUtc);
    }
}
