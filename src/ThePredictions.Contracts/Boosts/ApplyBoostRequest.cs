namespace ThePredictions.Contracts.Boosts;

public record ApplyBoostRequest(int LeagueId, int RoundId, string BoostCode);