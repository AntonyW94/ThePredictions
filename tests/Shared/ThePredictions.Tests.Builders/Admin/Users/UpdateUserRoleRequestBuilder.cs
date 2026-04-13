using ThePredictions.Contracts.Admin.Users;

namespace ThePredictions.Tests.Builders.Admin.Users;

public class UpdateUserRoleRequestBuilder
{
    private string _newRole = "Player";

    public UpdateUserRoleRequestBuilder WithNewRole(string newRole)
    {
        _newRole = newRole;
        return this;
    }

    public UpdateUserRoleRequest Build() => new(_newRole);
}
