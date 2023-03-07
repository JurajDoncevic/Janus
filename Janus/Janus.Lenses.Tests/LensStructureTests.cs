using FunctionalExtensions.Base;
using System.Reflection;

namespace Janus.Lenses.Tests;
public class LensStructureTests
{
    [Fact]
    public void LensStructureCorrectness()
    {
        var lensTypes =
            Assembly.Load("Janus.Lenses")
            .GetTypes()
            .Where(type => type.IsClass && 
                           (type.BaseType?.IsGenericType ?? false) &&
                           (type.BaseType?.GetGenericTypeDefinition().IsSubclassOf(typeof(Lens<,>)) ?? false));

        var lensExtensionClasses =
            Assembly.Load("Janus.Lenses")
            .GetTypes()
            .Where(type => type.IsClass && type.IsAbstract && type.IsSealed) // static = abstract sealed
            .Where(type => lensTypes.Map(lt => lt.Name).Contains(type.Name));

        Assert.NotEmpty(lensTypes);
        Assert.NotEmpty(lensExtensionClasses);

        foreach (var lensType in lensTypes)
        {
            var lensTypeExtension = lensExtensionClasses.FirstOrDefault(c => c.Name == lensType.Name);
            
            Assert.NotNull(lensTypeExtension);

            var lensConstructMethod = 
                lensTypeExtension.GetMethods()
                .FirstOrDefault(method => method.Name.Equals("Construct") &&
                                 method.ReturnParameter.ParameterType.Equals(lensType));

            Assert.NotNull(lensConstructMethod);
        }
    }
}
