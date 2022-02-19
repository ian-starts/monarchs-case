namespace Monarchs.Factories;

public interface IDateTimeOffsetFactory
{
    /// <summary>
    /// Create a DateTimeOffset based on a year.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    DateTimeOffset Create(int year);
}