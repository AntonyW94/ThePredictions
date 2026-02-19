using Ardalis.GuardClauses;
using ThePredictions.Domain.Common.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Domain.Common.Guards;

public static class GuardClauseExtensions
{
    public static void EntityNotFound<T>(this IGuardClause _, object key, [NotNull] T? input, string name = "Entity")
    {
        if (input is null)
            throw new EntityNotFoundException(name, key.ToString()!);
    }
}
