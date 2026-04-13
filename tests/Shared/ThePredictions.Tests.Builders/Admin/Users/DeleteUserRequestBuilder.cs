using ThePredictions.Contracts.Admin.Users;

namespace ThePredictions.Tests.Builders.Admin.Users;

public class DeleteUserRequestBuilder
{
    private string? _newAdministratorId;

    public DeleteUserRequestBuilder WithNewAdministratorId(string? newAdministratorId)
    {
        _newAdministratorId = newAdministratorId;
        return this;
    }

    public DeleteUserRequest Build() => new()
    {
        NewAdministratorId = _newAdministratorId
    };
}
