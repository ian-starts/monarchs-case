using Monarchs.Factories;
using Monarchs.Mapper;
using Monarchs.Repositories;

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