namespace Monarchs.Factories;

public class DateTimeOffsetFactory : IDateTimeOffsetFactory
{
    /// <inheritdoc />
    public DateTimeOffset Create(int year)
    {
        return new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}