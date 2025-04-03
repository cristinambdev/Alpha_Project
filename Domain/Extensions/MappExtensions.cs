using System.Reflection;

namespace Domain.Extentions;

//map the properties of one object (the source) to another object (the destination),
//when both objects share properties with matching names and types.
public static class MappExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        TDestination destination = Activator.CreateInstance<TDestination>()!;

        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance); //get all properties for our source (i.e. entity)
        var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance); //get all part for our model


        foreach ( var destinationProperty in destinationProperties )
        {
            var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == destinationProperty.Name && x.PropertyType == destinationProperty.PropertyType);
            if ( sourceProperty != null && destinationProperty.CanWrite)
            {
                var value = sourceProperty.GetValue(source);
                destinationProperty.SetValue(destination, value);
            }
        }
        return destination;
    }
}
