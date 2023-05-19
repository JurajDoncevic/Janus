using Janus.Base;
using System.Reflection;

namespace Janus.Lenses.Tests;
public class LensStructureTests
{
    private const string IMPL_NAMESPACE = "Janus.Lenses.Implementations";
    private const string ASSEMBLY_NAME = "Janus.Lenses";

    [Fact]
    public void LensStructureCorrectness()
    {
        var lensTypes =
            Assembly.Load(ASSEMBLY_NAME)
            .GetTypes()
            .Where(type => type.Namespace?.Equals(IMPL_NAMESPACE) ?? false)
            .Where(type => !type.IsAbstract &&
                           !type.IsInterface &&
                           (type.BaseType?.IsGenericType ?? false) &&
                           type.BaseType?.GetGenericTypeDefinition() == typeof(AsymmetricLens<,>))
            .ToList();

        var lensDefinitions =
            Assembly.Load(ASSEMBLY_NAME)
            .GetTypes()
            .Where(extType => extType.IsClass && extType.IsAbstract && extType.IsSealed) // static = abstract sealed
            .Select(extType => (lensType: lensTypes.FirstOrDefault(lt => lt.Name.Contains(extType.Name[0..^2])), extType))
            .Where(tuple => tuple.lensType != null)
            .ToList();


        // do all lenses have a valid extension static class?
        Assert.True(lensTypes.Count == lensDefinitions.Count); 

        foreach (var (lensType, lensExtensionType) in lensDefinitions)
        {
            // check the lens type
            // are lenses sealed
            Assert.True(lensType!.IsSealed);
            Assert.Empty(lensType.GetConstructors().Where(ctor => ctor.IsPublic));

            // check extension static class
            var lensConstructMethod =
                lensExtensionType.GetMethods()
                .FirstOrDefault(method => method.IsStatic && method.Name.Equals("Construct"));

            // does it contain a static construct method
            Assert.NotNull(lensConstructMethod);
            // is the return type correct
            Assert.Equal(lensType, lensConstructMethod.ReturnType.IsGenericType ? lensConstructMethod.ReturnType.GetGenericTypeDefinition() : lensConstructMethod.ReturnType);
        }
    }
}
