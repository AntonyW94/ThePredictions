using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Contracts.Authentication;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
public abstract record ResetPasswordResponse(bool IsSuccess);
