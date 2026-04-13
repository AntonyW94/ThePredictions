using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Matches;
using ThePredictions.Tests.Builders.Admin.Rounds;
using ThePredictions.Validators.Admin.Rounds;

namespace ThePredictions.Validators.Tests.Unit.Admin.Rounds;

public class UpdateRoundRequestValidatorTests
{
    private readonly UpdateRoundRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new UpdateRoundRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenStartDateUtcIsDefault()
    {
        var request = new UpdateRoundRequestBuilder()
            .WithStartDateUtc(default)
            .WithDeadlineUtc(new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.StartDateUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDeadlineUtcIsDefault()
    {
        var request = new UpdateRoundRequestBuilder()
            .WithDeadlineUtc(default)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.DeadlineUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDeadlineIsBeforeStartDate()
    {
        var request = new UpdateRoundRequestBuilder()
            .WithStartDateUtc(new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc))
            .WithDeadlineUtc(new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.DeadlineUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchesIsEmpty()
    {
        var request = new UpdateRoundRequestBuilder()
            .WithMatches([])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Matches);
    }

    [Fact]
    public void Validate_ShouldFail_WhenChildMatchIsInvalid()
    {
        var request = new UpdateRoundRequestBuilder()
            .WithMatches([new UpdateMatchRequestBuilder()
                .WithHomeTeamId(0)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveAnyValidationError();
    }
}
