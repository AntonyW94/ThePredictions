using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Users;
using ThePredictions.Validators.Admin.Users;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Admin.Users;

public class DeleteUserRequestValidatorTests
{
    private readonly DeleteUserRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenNewAdministratorIdIsNull()
    {
        var request = new DeleteUserRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenNewAdministratorIdIsValid()
    {
        var request = new DeleteUserRequestBuilder()
            .WithNewAdministratorId("some-valid-id")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNewAdministratorIdIsEmptyButNotNull()
    {
        var request = new DeleteUserRequestBuilder()
            .WithNewAdministratorId("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NewAdministratorId);
    }
}
