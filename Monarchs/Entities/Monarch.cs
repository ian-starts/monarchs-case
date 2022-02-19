namespace Monarchs.Entities;

public class Monarch
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Monarch"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="firstName"></param>
    /// <param name="city"></param>
    /// <param name="house"></param>
    /// <param name="ruledFrom"></param>
    /// <param name="ruledTo"></param>
    public Monarch(
        int id,
        string? name,
        string? firstName,
        string? city,
        string? house,
        DateTimeOffset ruledFrom,
        DateTimeOffset? ruledTo = default)
    {
        Id = id;
        Name = name;
        FirstName = firstName;
        City = city;
        House = house;
        RuledFrom = ruledFrom;
        RuledTo = ruledTo;
    }

    public int Id { get; private set; }

    /// <summary>
    /// Full name of the monarch.
    /// </summary>
    public string? Name { get; private set; }

    /// <summary/>
    public string? FirstName { get; private set; }

    /// <summary/>
    public string? City { get; private set; }

    /// <summary>
    /// The house this belongs Monarch belongs to (kind of like a family).
    /// </summary>
    public string? House { get; private set; }

    public DateTimeOffset RuledFrom { get; private set; }

    /// <summary>
    /// If this is null the Monarch is still ruling.
    /// </summary>
    public DateTimeOffset? RuledTo { get; private set; }

    /// <summary>
    /// Calculates the time a Monarch ruled on the fly. Ignore in persistence.
    /// </summary>
    public TimeSpan RuledFor => (RuledTo
                                 ?? new DateTimeOffset(DateTime.Now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero))
                                - RuledFrom;
}