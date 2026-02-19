using ThePredictions.Contracts.Admin.Teams;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Validators.Admin.Teams;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class UpdateTeamRequestValidator : BaseTeamRequestValidator<UpdateTeamRequest>;