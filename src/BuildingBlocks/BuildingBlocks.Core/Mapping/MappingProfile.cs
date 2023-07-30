using System.Reflection;
using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;

namespace BuildingBlocks.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile() : this(Assembly.GetExecutingAssembly())
    {
        //ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public MappingProfile(Assembly assembly)
    {
        ApplyMappingsFromAssembly(assembly);
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mapFromType = typeof(IMapWith<>);

        var mappingMethodName = nameof(IMapWith<object>.Mapping);

        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

        var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        var argumentTypes = new[]
                            {
                                typeof(Profile)
                            };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(
                    instance,
                    new object[]
                    {
                        this
                    });
            }
            else
            {
                var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (var @interface in interfaces)
                    {
                        var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(
                            instance,
                            new object[]
                            {
                                this
                            });
                    }
                }
            }
        }
    }
}
