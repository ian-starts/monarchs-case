using Newtonsoft.Json;

Console.WriteLine("Hello! I'm Margeret, your (derivatively named) Monarch expert.");
Console.WriteLine("Press any key to start.");
Console.ReadKey(true);
Console.WriteLine("Working, please hold.");

// TODO: Use IHttpClientFactory.
// See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0
var repo = new MonarchRepository(new HttpClient());
var mapper = new MonarchMapper(new DateTimeOffsetFactory());
try
{
    // Get the monarchs from the API.
    var monarchDtos = await repo.GetMonarchsAsync();

    // Map the DTOs to something we can use.
    var monarchs = monarchDtos.Select(m => mapper.Map(m)).ToList();

    // Query for getting the longest ruling Monarch.
    var longestRuling = monarchs.OrderBy(i => i.RuledFor).Last();

    // Query for getting the longest ruling House.
    var longestRulingHouse =
        monarchs.GroupBy(i => i.House)
            .Select(
                i => new
                {
                    // Sum all the ruling time of the Monarchs in the current group.
                    RuledFor =
                        i.AsEnumerable().Aggregate(TimeSpan.Zero, (span, monarch) => span + monarch.RuledFor),
                    House = i.Key
                })
            .OrderBy(i => i.RuledFor)
            .Last();

    // Query for getting the most common first name. Should throw an exception if there is no first name,
    // because the parsing would then be broken in current use case.
    var mostCommonFirstName = monarchs.GroupBy(monarch => monarch.FirstName!.ToLowerInvariant())
        .Select(
            i => new
            {
                FirstName = i.Key,
                Hits = i.AsEnumerable().Count()
            })
        .OrderBy(i => i.Hits)
        .Last();

    // I added thread sleeps for a more human pacing.
    Console.WriteLine("Amount of Monarchs:");
    Thread.Sleep(TimeSpan.FromMilliseconds(2000));
    Console.WriteLine(monarchs.Count);
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    Console.WriteLine("Longest ruling Monarch:");
    Thread.Sleep(TimeSpan.FromMilliseconds(2000));
    Console.WriteLine($"{longestRuling.Name} (ruled for {longestRuling.RuledFor.Days / 365} years)");
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    Console.WriteLine("Longest ruling House:");
    Thread.Sleep(TimeSpan.FromMilliseconds(2000));
    Console.WriteLine($"{longestRulingHouse.House} (ruled for {longestRulingHouse.RuledFor.Days / 365} years)");
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    Console.WriteLine("Most common FirstName:");
    Thread.Sleep(TimeSpan.FromMilliseconds(2000));
    Console.WriteLine($"Well, subjectively that's COMMONwealth...");
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    Console.WriteLine($"Ha");
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    Console.WriteLine($"Ha");
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    Console.WriteLine($"Ha");
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    Console.WriteLine($"JK, it's: {mostCommonFirstName.FirstName} (with {mostCommonFirstName.Hits} hits)");
}
// In an application I would let ExceptionFilters handle the flow of exception. Personally I feel that
// exceptionhandling is more resilient then doing things like null checks, due to their predictable behaviour.  
catch (HttpRequestException e)
{
    Console.WriteLine("\nException Caught!");
    Console.WriteLine("Message :{0} ", e.Message);
}
catch (ArgumentNullException e)
{
    Console.WriteLine("\nException Caught!");
    Console.WriteLine("Message :{0} ", e.Message);
}

#region Repositories

// Should probably interface this to make it easier to stub, but seeing as this is on the API level DTO
// (and no Tests in sight) it may be a bit overkill for now.
// Would make more sense to create an extra abstraction for getting the Monarch Entity instead of the DTO.
public class MonarchRepository
{
    private readonly HttpClient _client;

    public MonarchRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<MonarchDto>> GetMonarchsAsync()
    {
        // Would add this in AppSettings.json.
        var response = await _client.GetAsync(
            "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        var monarchs = JsonConvert.DeserializeObject<List<MonarchDto>>(responseBody);
        if (monarchs == null || !monarchs.Any())
        {
            throw new Exception("No Monarchs found.");
        }

        return monarchs;
    }
}

#endregion

#region Mapper

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

/// <summary>
/// Map one type to another.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public interface IMapper<in TIn, out TOut>
    where TIn : class
    where TOut : class
{
    /// <summary>
    /// Execute the mapping function.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    TOut Map(TIn dto);
}

#endregion

#region Factories

public interface IDateTimeOffsetFactory
{
    /// <summary>
    /// Create a DateTimeOffset based on a year.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    DateTimeOffset Create(int year);
}

public class DateTimeOffsetFactory : IDateTimeOffsetFactory
{
    /// <inheritdoc />
    public DateTimeOffset Create(int year)
    {
        return new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}

#endregion

#region DataContainers

// Personally, not a great fan of JsonProperties, they add logic to a POCO, which is by definition not the goal of a POCO.
// This is a DTO that's only used for the API, making it a less of an issue, and it allows to
// add more logical property names. These are things a team can make decisions on, as long as it's consistent.
public class MonarchDto
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nm")]
    public string? Name { get; set; }

    [JsonProperty("cty")]
    public string? City { get; set; }

    [JsonProperty("hse")]
    public string? House { get; set; }

    [JsonProperty("yrs")]
    public string? RulingYears { get; set; }
}

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

#endregion