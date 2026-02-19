namespace ThePredictions.Contracts.Authentication;

public record FailedAuthenticationResponse(string Message) : AuthenticationResponse(false);