using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Users;
using ThePredictions.Validators.Admin.Users;

namespace ThePredictions.Validators.Tests.Unit.Admin.Users;

public class UpdateUserRoleRequestValidatorTests
{
    private readonly UpdateUserRoleRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenNewRoleIsValid()
    {
        var request = new UpdateUserRoleRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenNewRoleIsAdministrator()
    {
        var request = new UpdateUserRoleRequestBuilder()
            .WithNewRole("Administrator")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenNewRoleIsCaseInsensitive()
    {
        var request = new UpdateUserRoleRequestBuilder()
            .WithNewRole("player")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNewRoleIsEmpty()
    {
        var request = new UpdateUserRoleRequestBuilder()
            .WithNewRole("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NewRole);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNewRoleIsInvalid()
    {
        var request = new UpdateUserRoleRequestBuilder()
            .WithNewRole("SuperAdmin")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NewRole);
    }
}
