using MediatR;
using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Application.Features.Authentication.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    bool Over18Confirmed,
    bool TermsAccepted,
    bool MarketingOptIn) : IRequest<AuthenticationResponse>;
