using FluentValidation;
using ThePredictions.Contracts.Admin.Matches;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Validators.Admin.Matches;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class MatchResultDtoValidator : AbstractValidator<MatchResultDto>
{
    public MatchResultDtoValidator()
    {
        RuleFor(x => x.MatchId)
            .GreaterThan(0)
            .WithMessage("Match ID must be greater than 0.");

        RuleFor(x => x.HomeScore)
            .InclusiveBetween(0, 9)
            .WithMessage("Home score must be between 0 and 9.");

        RuleFor(x => x.AwayScore)
            .InclusiveBetween(0, 9)
            .WithMessage("Away score must be between 0 and 9.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Match status must be a valid status value.");
    }
}
