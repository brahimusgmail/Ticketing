namespace Ticketing.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Ticketing.Api;
using Ticketing.Application.Abstractions;
using Ticketing.Domain.Entities;
using Ticketing.Shared.Contracts.Users;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(Resource.UserIdRequiredMessage);
        }

        var user = await this._repository.GetByIdAsync(id, ct);

        return user == null ? NotFound() : Ok(ToResponse(user));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken ct)
    {
        var items = await this._repository.GetAllAsync(ct);

        return Ok(items.Select(ToResponse));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest user, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user?.Email))
        {
            return this.BadRequest(Resource.UserMustHaveEmailMessage);
        }

        if (await this._repository.ExistsByEmailAsync(user.Email, ct))
        {
            return this.Conflict(Resource.UserExistsMessage);
        }

        if (string.IsNullOrWhiteSpace(user?.FullName))
        {
            return this.BadRequest(Resource.UserMustHaveFullNameMessage);
        }

        var created = await _repository.InsertAsync(
                                            new User
                                            {
                                                Email = user.Email,
                                                FullName = user.FullName,
                                            },
                                            ct);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest user, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(Resource.UserIdRequiredMessage);
        }

        var result = await _repository.UpdateAsync(
            new User
            {
                Id = id,
                Roles = user.Roles ?? new List<string>(),
                FullName = user.FullName ?? string.Empty,
                IsActive = user.IsActive,
            },
            ct);

        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(Resource.UserIdRequiredMessage);
        }

        var result = await _repository.DeleteAsync(id, ct);

        return result ? NoContent() : NotFound();
    }

    private static UserResponse ToResponse(User u)
    {
        return new(u.Id, u.Email, u.FullName, u.Roles, u.CreatedAtUtc, u.UpdatedAtUtc, u.IsActive);
    }
}
