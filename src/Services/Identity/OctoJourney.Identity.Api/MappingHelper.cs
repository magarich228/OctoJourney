using AutoMapper;

namespace OctoJourney.Identity.Api;

public static class MappingHelper
{
    public static TDestination Map<TSource, TDestination>(TSource source)
    {
        var config = new MapperConfiguration(cfg =>
            cfg.CreateMap<TSource, TDestination>());
        
        var mapper = new Mapper(config);

        return mapper.Map<TSource, TDestination>(source);
    }
    
    public static IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> sources)
    {
        var config = new MapperConfiguration(cfg =>
            cfg.CreateMap<TSource, TDestination>());
        
        var mapper = new Mapper(config);

        return mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(sources);
    }
}