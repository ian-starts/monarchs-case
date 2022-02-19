using Monarchs.Dtos;
using Monarchs.Entities;
using Monarchs.Factories;

namespace Monarchs.Mapper;

/// <inheritdoc />
public class MonarchMapper : IMapper<MonarchDto, Monarch>
{
    private readonly IDateTimeOffsetFactory _dateTimeOffsetFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonarchMapper"/> class.
    /// </summary>
    /// <param name="dateTimeOffsetFactory"></param>
    public MonarchMapper(IDateTimeOffsetFactory dateTimeOffsetFactory)
    {
        _dateTimeOffsetFactory = dateTimeOffsetFactory;
    }

    /// <inheritdoc />
    public Monarch Map(MonarchDto dto)
    {
        // If RulingYears is not set, the constructor of Monarch will fail.
        var rulingYears = dto.RulingYears ?? throw new ArgumentNullException(dto.RulingYears);
        var ruledFrom = default(DateTimeOffset);
        var ruledTo = default(DateTimeOffset?);

        // If the ruling years string does not contain a hyphen, the monarch seized ruling in the same year.
        if (!rulingYears.Contains('-'))
        {
            ruledFrom = _dateTimeOffsetFactory.Create(int.Parse(rulingYears));
            ruledTo = ruledFrom;
        }
        else
        {
            // Split out the string, should in production environments be replaced with a Regex expression for
            // more resilience. 
            var ruledTimes = rulingYears.Split("-");
            ruledFrom = _dateTimeOffsetFactory.Create(int.Parse(ruledTimes.First()));
            var ruledToString = ruledTimes.Last();

            // If there is no RuledTo available, the ruler is still ruling, thus is null. 
            ruledTo = ruledToString == string.Empty
                ? null
                : _dateTimeOffsetFactory.Create(int.Parse(ruledToString));
        }

        // TODO: Implement a robust algo for getting first names. They can be quite tricky, especially internationally.
        var firstName = dto.Name?.Split(" ").First();

        return new Monarch(dto.Id, dto.Name, firstName, dto.City, dto.House, ruledFrom, ruledTo);
    }
}