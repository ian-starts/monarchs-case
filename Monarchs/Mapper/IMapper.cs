namespace Monarchs.Mapper;

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