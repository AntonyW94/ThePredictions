namespace ThePredictions.Application.Formatters;

public interface IEmailDateFormatter
{
    string FormatDeadline(DateTime dateUtc);
}