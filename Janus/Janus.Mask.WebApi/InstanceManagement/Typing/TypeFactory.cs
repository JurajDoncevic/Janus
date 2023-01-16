using System.Reflection.Emit;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Janus.Commons.SchemaModels;
using FunctionalExtensions.Base.Resulting;
using FunctionalExtensions.Base;
using System.Runtime.Loader;
using Janus.Mask.WebApi.InstanceManagement.Templates;
using Janus.Logging;

namespace Janus.Mask.WebApi.InstanceManagement.Typing;
public class TypeFactory : IDisposable
{
    private readonly AssemblyName _dynamicAssemblyName;
    private readonly AssemblyBuilder _assemblyBuilder;
    private ModuleBuilder _moduleBuilder;
    private readonly Assembly _assembly;

    public TypeFactory(string assemblyName = "DynamicAssembly")
    {
        
        _dynamicAssemblyName = new AssemblyName(assemblyName);
        _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_dynamicAssemblyName, AssemblyBuilderAccess.RunAndCollect);
        _moduleBuilder = _assemblyBuilder.DefineDynamicModule("DynamicModule");
        _assembly = _moduleBuilder.Assembly;
        
        AssemblyLoadContext.GetLoadContext(_assembly);
    }

    /// <summary>
    /// Creates a DTO type according to a DtoTyping specification
    /// </summary>
    /// <param name="dtoTyping">DtoTyping specification</param>
    /// <param name="namespace">Namespace to place the DTO type into</param>
    /// <param name="parentType">Parent type of the DTO</param>
    /// <returns>DTO type</returns>
    public Type CreateDtoType(DtoTyping dtoTyping, string @namespace, Type? parentType = null)
    {
        TypeBuilder typeBuilder = _moduleBuilder.DefineType(
              $"{@namespace}.{dtoTyping.Name}", TypeAttributes.Public, parentType);

        // define a default constructor
        ConstructorBuilder ctorBuilder;

        // get the parent constructor
        var baseCtor = parentType?.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.EmptyTypes);

        if (baseCtor != null) // call base constructor if base class exists
        {
            ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, baseCtor.CallingConvention, baseCtor.GetParameters().Map(p => p.ParameterType).ToArray());
            // generate the constructor of the new subtype
            var ilGen = ctorBuilder.GetILGenerator();
            // i want to call the constructor of the baseclass with eventName as param
            ilGen.Emit(OpCodes.Ldarg_0); // push "this"
            ilGen.Emit(OpCodes.Call, baseCtor); // call base constructor
            ilGen.Emit(OpCodes.Ret); // return :)
        }
        else
        {
            ctorBuilder = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
        }

        // add properties if any
        foreach (var (propertyName, propertyType) in dtoTyping.Properties)
        {
            string fieldName = "_" + char.ToLower(propertyName.First()).ToString() + string.Concat(propertyName.Skip(1));
            var fieldBuilder = typeBuilder.DefineField(fieldName, propertyType, FieldAttributes.Private);
            var propBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);


            // method attributes for property getters and setters
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            var setPropBuilder = typeBuilder.DefineMethod($"set_{propertyName}", getSetAttr, CallingConventions.HasThis, typeof(void), new Type[] { propertyType });
            var setIlGen = setPropBuilder.GetILGenerator();
            setIlGen.Emit(OpCodes.Ldarg_0); // push this
            setIlGen.Emit(OpCodes.Ldarg_1); // push value
            setIlGen.Emit(OpCodes.Stfld, fieldBuilder); // store value in field
            setIlGen.Emit(OpCodes.Ret); // return

            var getPropBuilder = typeBuilder.DefineMethod($"get_{propertyName}", getSetAttr, CallingConventions.HasThis, propertyType, Type.EmptyTypes);
            var getIlGen = getPropBuilder.GetILGenerator();
            getIlGen.Emit(OpCodes.Ldarg_0); // push this
            getIlGen.Emit(OpCodes.Ldfld, fieldBuilder); // push field onto stack
            getIlGen.Emit(OpCodes.Ret); // return

            propBuilder.SetGetMethod(getPropBuilder);
            propBuilder.SetSetMethod(setPropBuilder);
        }

        // create the type itself
        Type createdType = typeBuilder?.CreateType() ?? typeof(object);

        return createdType;
    }

    /// <summary>
    /// Creates a Controller type according to a ControllerTyping specification over a template GenericController<> class
    /// </summary>
    /// <param name="controllerTyping">Controller typing specification</param>
    /// <param name="namespace">Namespace to place the controller type</param>
    /// <param name="parentType">Parent type template. Expected to be GenericController<></param>
    /// <param name="ctorInitTypes">Init types for the template constructor</param>
    /// <returns>Controller type</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Type CreateControllerType(ControllerTyping controllerTyping, string @namespace, Type? parentType, Type[]? ctorInitTypes = null)
    {
        parentType ??= typeof(GenericController<,>);

        if (controllerTyping is null)
        {
            throw new ArgumentNullException(nameof(controllerTyping));
        }

        string typeName = controllerTyping.ControllerName + "Controller";

        TypeBuilder typeBuilder = _moduleBuilder.DefineType(
              $"{@namespace}.{typeName}", TypeAttributes.Public, parentType);

        // field for tableau id string representation that is targeted
        var targetTableauIdField = typeBuilder.DefineField("_tableauIdString", typeof(string), FieldAttributes.Private);
        // field for identity attribute id string representation of the targeted tableau 
        var identityAttributeIdField = typeBuilder.DefineField("_attributeIdString", typeof(string), FieldAttributes.Private);
        // field for command provider
        var commandProviderField = parentType.GetField("_commandProvider", BindingFlags.NonPublic | BindingFlags.Instance);
        // field for query provider
        var queryProviderField = parentType.GetField("_queryProvider", BindingFlags.NonPublic | BindingFlags.Instance);
        // field for ILogger
        var loggerType = typeof(ILogger<>).MakeGenericType(parentType);
        var loggerField = typeBuilder.DefineField("_logger", loggerType, FieldAttributes.Private);
        // default error code get method
        var defaultErrorCodeGetMethod = parentType.GetMethod("get_DEFAULT_ERROR_CODE", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

        // if there is a route prefix for the controller, set Route attribute
        if (controllerTyping.RoutePrefix is not null)
        {
            string controllerRoute = $"{controllerTyping.RoutePrefix}/{controllerTyping.ControllerName}";
            var routePrefixAttributeType = typeof(RouteAttribute);
            var constructorInfo = routePrefixAttributeType.GetConstructor(new Type[] { typeof(string) });
            var attributeBuilder = new CustomAttributeBuilder(constructorInfo, new object[] { controllerRoute });
            typeBuilder.SetCustomAttribute(attributeBuilder);
        }

        typeBuilder = GenerateTargetSchemaModelControllerPropertyOverrides(typeBuilder, parentType, targetTableauIdField, identityAttributeIdField);

        // create Create POST method if there is a PostDto specified
        if (controllerTyping.PostDto)
        {
            typeBuilder = GenerateCreateControllerActionMethod(typeBuilder, controllerTyping.PostDto.Value, commandProviderField, loggerField, defaultErrorCodeGetMethod);
        }

        // create update methods
        foreach (var (putDtoIdx, putDto) in Enumerable.Zip(Enumerable.Range(0, controllerTyping.PutDtos.Count()), controllerTyping.PutDtos))
        {
            typeBuilder = GenerateUpdateControllerActionMethod(typeBuilder, putDto, putDtoIdx, commandProviderField, loggerField, defaultErrorCodeGetMethod);
        }

        // define constructor over base constructor
        typeBuilder = GenerateControllerConstructor(typeBuilder, controllerTyping, ctorInitTypes, parentType, targetTableauIdField, identityAttributeIdField, loggerField);


        // Create the type itself
        Type createdType = typeBuilder?.CreateType() ?? typeof(object);

        return createdType;
    }

    /// <summary>
    /// Generates the target tableau id and identity attribute id field overrides. 
    /// These are used by the query and command providers to target the appropriate schema models.
    /// </summary>
    /// <param name="targetTypeBuilder">Target type builder for the controller</param>
    /// <param name="parentType">Controller parent type template</param>
    /// <param name="targetTableauIdField">Target tableau id field from the parent type template</param>
    /// <param name="identityAttributeIdField">Target identity attribute id field from the parent type template</param>
    /// <returns>Type builder after generation</returns>
    private TypeBuilder GenerateTargetSchemaModelControllerPropertyOverrides(TypeBuilder targetTypeBuilder, Type parentType, FieldBuilder targetTableauIdField, FieldBuilder identityAttributeIdField)
    {
        // override the abstract get for target tableau id
        var abstractGetTableauIdMethod = parentType.GetMethod("get_TargetingTableauId", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo tableauIdFromMethod = typeof(TableauId).GetMethod("From", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string) });
        // build the get method
        var getTableauIdMethodImplBuilder = targetTypeBuilder.DefineMethod("get_TargetingTableauId", MethodAttributes.Family | MethodAttributes.Virtual, typeof(TableauId), Type.EmptyTypes);
        var getTableauIdMethodImplGen = getTableauIdMethodImplBuilder.GetILGenerator();
        getTableauIdMethodImplGen.Emit(OpCodes.Ldarg_0); // push this
        getTableauIdMethodImplGen.Emit(OpCodes.Ldfld, targetTableauIdField); // get tableau id string field value
        getTableauIdMethodImplGen.Emit(OpCodes.Call, tableauIdFromMethod); // send it to static method to create TableauId
        getTableauIdMethodImplGen.Emit(OpCodes.Ret); // return whatever is on the stack
        targetTypeBuilder.DefineMethodOverride(getTableauIdMethodImplBuilder, abstractGetTableauIdMethod);

        // override the abstract get for target identity attribute id
        var abstractGetAttributeIdMethod = parentType.GetMethod("get_IdentityAttributeId", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo attributeIdFromMethod = typeof(AttributeId).GetMethod("From", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string) });
        // build the get method
        var getAttributeIdMethodImplBuilder = targetTypeBuilder.DefineMethod("get_IdentityAttributeId", MethodAttributes.Family | MethodAttributes.Virtual, typeof(AttributeId), Type.EmptyTypes);
        var getAttributeIdMethodImplGen = getAttributeIdMethodImplBuilder.GetILGenerator();
        getAttributeIdMethodImplGen.Emit(OpCodes.Ldarg_0); // push this
        getAttributeIdMethodImplGen.Emit(OpCodes.Ldfld, identityAttributeIdField); // get attribute id string field value
        getAttributeIdMethodImplGen.Emit(OpCodes.Call, attributeIdFromMethod); // send it to static method to create TableauId
        getAttributeIdMethodImplGen.Emit(OpCodes.Ret); // return whatever is on the stack
        targetTypeBuilder.DefineMethodOverride(getAttributeIdMethodImplBuilder, abstractGetAttributeIdMethod);

        return targetTypeBuilder;
    }

    /// <summary>
    /// Generates the controller constructor
    /// </summary>
    /// <param name="targetTypeBuilder">Target type builder for the </param>
    /// <param name="controllerTyping">Controller typing specification</param>
    /// <param name="ctorInitTypes">Parent template controller parameter types</param>
    /// <param name="parentType">Parent template type</param>
    /// <param name="targetTableauIdField">Target tableau id field</param>
    /// <param name="identityAttributeIdField">Target identity attribute id field</param>
    /// <param name="loggerField">Logger field</param>
    /// <returns>Type builder after generation</returns>
    private TypeBuilder GenerateControllerConstructor(TypeBuilder targetTypeBuilder, ControllerTyping controllerTyping, Type[]? ctorInitTypes, Type parentType, FieldBuilder targetTableauIdField, FieldBuilder identityAttributeIdField, FieldBuilder loggerField)
    {
        // get base constructor from GenericConstructor
        var baseCtor = parentType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, ctorInitTypes ?? Type.EmptyTypes, null);

        // define the same constructor on the new subtype
        var ctorBuilder = targetTypeBuilder.DefineConstructor(baseCtor.Attributes, baseCtor.CallingConvention, baseCtor.GetParameters().Select(p => p.ParameterType).ToArray());

        // generate the constructor of the new subtype
        var ilGen = ctorBuilder.GetILGenerator();
        // i want to call the constructor of the baseclass with ILogger as param
        ilGen.Emit(OpCodes.Ldarg_0);
        ilGen.Emit(OpCodes.Ldstr, controllerTyping.TargetTableauId.ToString()); // load string for tableau id
        ilGen.Emit(OpCodes.Stfld, targetTableauIdField); // store tableau id string into field
        ilGen.Emit(OpCodes.Ldarg_0);
        ilGen.Emit(OpCodes.Ldstr, controllerTyping.IdentityAttributeId.ToString()); // load string for identity attribute id
        ilGen.Emit(OpCodes.Stfld, identityAttributeIdField); // store attribute id string into field
        ilGen.Emit(OpCodes.Ldarg_0); // push this
        ilGen.Emit(OpCodes.Ldarg_1); // push logger
        ilGen.Emit(OpCodes.Call, baseCtor); // call base constructor
        ilGen.Emit(OpCodes.Ldarg_0); // push this
        ilGen.Emit(OpCodes.Ldarg_1); // push logger
        ilGen.Emit(OpCodes.Stfld, loggerField); // store tableau id string into field
        ilGen.Emit(OpCodes.Ret); // return :)

        return targetTypeBuilder;
    }

    /// <summary>
    /// Generates an update action for a PUT DTO
    /// </summary>
    /// <param name="targetTypeBuilder">Target type builder</param>
    /// <param name="putDto">PUT DTO typing specification</param>
    /// <param name="dtoIdx">DTO index - Update set index</param>
    /// <param name="commandProviderField">Command provider field</param>
    /// <param name="loggerField">Logger field</param>
    /// <param name="defaultErrorCodeGetMethod">Default error code getter method</param>
    /// <returns>Type builder after generation</returns>
    private TypeBuilder GenerateUpdateControllerActionMethod(TypeBuilder targetTypeBuilder, DtoTyping putDto, int dtoIdx, FieldInfo commandProviderField, FieldBuilder loggerField, MethodInfo defaultErrorCodeGetMethod)
    {
        var putDtoType = putDto.GenerateType(this);
        var putInterfaceType = typeof(IPutController<>).MakeGenericType(putDtoType);
        var interfaceUpdateMethod = putInterfaceType.GetMethod("Update");
        targetTypeBuilder.AddInterfaceImplementation(putInterfaceType);

        var updateMethodImplBuilder =
            targetTypeBuilder.DefineMethod(
                $"Update_{dtoIdx}",
                MethodAttributes.Public | MethodAttributes.Virtual,
                interfaceUpdateMethod.ReturnType,
                interfaceUpdateMethod.GetParameters().Select(p => p.ParameterType).ToArray()
                );

        updateMethodImplBuilder
            .DefineParameter(1, ParameterAttributes.None, "selection")
            .SetCustomAttribute(
                typeof(FromQueryAttribute).GetConstructor(Type.EmptyTypes),
                new byte[] { }
                );
        updateMethodImplBuilder
            .DefineParameter(2, ParameterAttributes.None, "model")
            .SetCustomAttribute(
                typeof(FromBodyAttribute).GetConstructor(Type.EmptyTypes),
                new byte[] { }
                );

        var httpPutAttributeType = typeof(HttpPutAttribute).GetConstructor(Type.EmptyTypes);
        var httpPutAttr = new CustomAttributeBuilder(httpPutAttributeType, new object[] { });
        updateMethodImplBuilder.SetCustomAttribute(httpPutAttr);

        var routeAttributeType = typeof(RouteAttribute).GetConstructor(new Type[] { typeof(string) });
        var routeAttr = new CustomAttributeBuilder(routeAttributeType, new object[] { $"UpdateSet_{dtoIdx}" });
        updateMethodImplBuilder.SetCustomAttribute(routeAttr);

        var concatMethod = typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, new Type[] { typeof(string), typeof(string) });
        var debugLogMethod = loggerField.FieldType.GetMethod("Debug");
        var okActionResultMethod = typeof(ControllerBase).GetMethod("Ok", BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);
        var problemActionResultMethod = typeof(ControllerBase).GetMethod("Problem", BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(string), typeof(string), typeof(int), typeof(string), typeof(string) });
        var commandProviderUpdateMethod = commandProviderField.FieldType.GetMethod("Update");
        var resultImplicitBoolMethod = typeof(Result).GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(Result) });
        var resultGetMessageMethod = typeof(Result).GetMethod("get_Message", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField, Type.EmptyTypes);

        var updateMethodImplGen = updateMethodImplBuilder.GetILGenerator();
        // prepare labels
        var resultSuccessLabel = updateMethodImplGen.DefineLabel();
        var resultFailureLabel = updateMethodImplGen.DefineLabel();
        var finalLabel = updateMethodImplGen.DefineLabel();
        // declare locals
        updateMethodImplGen.DeclareLocal(typeof(Result)); // var result
                                                          // log update event
        updateMethodImplGen.Emit(OpCodes.Ldarg_0); // push this
        updateMethodImplGen.Emit(OpCodes.Ldfld, loggerField); // push logger 
        updateMethodImplGen.Emit(OpCodes.Ldstr, $"Controller {targetTypeBuilder.Name} action Update_{dtoIdx} called with selection "); // push string
        updateMethodImplGen.Emit(OpCodes.Ldarg_1); // push selection for concat
        updateMethodImplGen.Emit(OpCodes.Call, concatMethod); // concat
        updateMethodImplGen.Emit(OpCodes.Callvirt, debugLogMethod); // log concated string
        updateMethodImplGen.Emit(OpCodes.Nop);
        // call command provider and store result in var
        updateMethodImplGen.Emit(OpCodes.Ldarg_0); // push this
        updateMethodImplGen.Emit(OpCodes.Ldfld, commandProviderField);
        updateMethodImplGen.Emit(OpCodes.Ldarg_1); // load selection string
        updateMethodImplGen.Emit(OpCodes.Ldarg_2); // load model
        updateMethodImplGen.Emit(OpCodes.Callvirt, commandProviderUpdateMethod); // call command provider update
        updateMethodImplGen.Emit(OpCodes.Stloc_0); // store result in var in loc_0
        // return action result depending on result
        updateMethodImplGen.Emit(OpCodes.Ldloc_0); // load result from var in loc_0
        updateMethodImplGen.Emit(OpCodes.Call, resultImplicitBoolMethod); // call implicit bool method
        updateMethodImplGen.Emit(OpCodes.Brtrue_S, resultSuccessLabel); // if result impl bool method gives true jump to success
        // on failure return Problem(statusCode: DEFAULT_ERROR_CODE, detail: result.Message)
        updateMethodImplGen.MarkLabel(resultFailureLabel);
        updateMethodImplGen.Emit(OpCodes.Ldarg_0); // load this
        updateMethodImplGen.Emit(OpCodes.Ldloca_S, 0); // get the result
        updateMethodImplGen.Emit(OpCodes.Call, resultGetMessageMethod); // call the result message getter; message now on eval stack
        updateMethodImplGen.Emit(OpCodes.Ldnull); // null for instance
        updateMethodImplGen.Emit(OpCodes.Ldarg_0); // load this
        updateMethodImplGen.Emit(OpCodes.Call, defaultErrorCodeGetMethod); // load status code 
        updateMethodImplGen.Emit(OpCodes.Newobj, typeof(Nullable<int>).GetConstructor(new Type[] { typeof(int) })); // cast to int?; now on stack
        updateMethodImplGen.Emit(OpCodes.Ldnull); // null for title
        updateMethodImplGen.Emit(OpCodes.Ldnull); // null for type
        updateMethodImplGen.Emit(OpCodes.Call, problemActionResultMethod); // call Problem
        updateMethodImplGen.Emit(OpCodes.Br_S, finalLabel); // go to return
        // on success, return Ok
        updateMethodImplGen.MarkLabel(resultSuccessLabel);
        updateMethodImplGen.Emit(OpCodes.Ldarg_0); // load this
        updateMethodImplGen.Emit(OpCodes.Call, okActionResultMethod); // call Ok
        // final label for return
        updateMethodImplGen.MarkLabel(finalLabel);
        updateMethodImplGen.Emit(OpCodes.Ret); // return result on eval stack

        targetTypeBuilder.DefineMethodOverride(updateMethodImplBuilder, interfaceUpdateMethod);

        return targetTypeBuilder;
    }

    /// <summary>
    /// Generates a create action for a POST DTO
    /// </summary>
    /// <param name="targetTypeBuilder">Target type builder</param>
    /// <param name="postDto">POST DTO typing specification</param>
    /// <param name="commandProviderField">Command provider field</param>
    /// <param name="loggerField">Logger field</param>
    /// <param name="defaultErrorCodeGetMethod">Default error code getter method</param>
    /// <returns>Type builde after generation</returns>
    private TypeBuilder GenerateCreateControllerActionMethod(TypeBuilder targetTypeBuilder, DtoTyping postDto, FieldInfo commandProviderField, FieldBuilder loggerField, MethodInfo defaultErrorCodeGetMethod)
    {
        var postDtoType = postDto.GenerateType(this);
        var postInterfaceType = typeof(IPostController<>).MakeGenericType(postDtoType);
        var interfaceCreateMethod = postInterfaceType.GetMethod("Create");
        targetTypeBuilder.AddInterfaceImplementation(postInterfaceType);

        var createMethodImplBuilder =
            targetTypeBuilder.DefineMethod(
                "Create",
                MethodAttributes.Public | MethodAttributes.Virtual,
                interfaceCreateMethod.ReturnType,
                interfaceCreateMethod.GetParameters().Select(p => p.ParameterType).ToArray()
                );

        createMethodImplBuilder
            .DefineParameter(1, ParameterAttributes.None, "model")
            .SetCustomAttribute(
                typeof(FromBodyAttribute).GetConstructor(Type.EmptyTypes),
                new byte[] { }
                );

        var httpPostAttributeType = typeof(HttpPostAttribute).GetConstructor(Type.EmptyTypes);
        var httpPostAttr = new CustomAttributeBuilder(httpPostAttributeType, new object[] { });
        createMethodImplBuilder.SetCustomAttribute(httpPostAttr);

        var routeAttributeType = typeof(RouteAttribute).GetConstructor(new Type[] { typeof(string) });
        var routeAttr = new CustomAttributeBuilder(routeAttributeType, new object[] { "" });
        createMethodImplBuilder.SetCustomAttribute(routeAttr);

        var debugLogMethod = loggerField.FieldType.GetMethod("Debug");
        var okActionResultMethod = typeof(ControllerBase).GetMethod("Ok", BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);
        var problemActionResultMethod = typeof(ControllerBase).GetMethod("Problem", BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(string), typeof(string), typeof(int), typeof(string), typeof(string) });
        var commandProviderCreateMethod = commandProviderField.FieldType.GetMethod("Create");
        var resultImplicitBoolMethod = typeof(Result).GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(Result) });
        var resultGetMessageMethod = typeof(Result).GetMethod("get_Message", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField, Type.EmptyTypes);

        var createMethodImplGen = createMethodImplBuilder.GetILGenerator();
        // prepare labels
        var resultSuccessLabel = createMethodImplGen.DefineLabel();
        var resultFailureLabel = createMethodImplGen.DefineLabel();
        var finalLabel = createMethodImplGen.DefineLabel();
        // declare locals
        createMethodImplGen.DeclareLocal(typeof(Result)); // var result
                                                          // log create event
        createMethodImplGen.Emit(OpCodes.Ldarg_0); // push this
        createMethodImplGen.Emit(OpCodes.Ldfld, loggerField);
        createMethodImplGen.Emit(OpCodes.Ldstr, $"Controller {targetTypeBuilder.Name} action Create called");
        createMethodImplGen.Emit(OpCodes.Callvirt, debugLogMethod);
        createMethodImplGen.Emit(OpCodes.Nop);
        // call command provider and store result in var
        createMethodImplGen.Emit(OpCodes.Ldarg_0); // push this
        createMethodImplGen.Emit(OpCodes.Ldfld, commandProviderField);
        createMethodImplGen.Emit(OpCodes.Ldarg_1); // load model
        createMethodImplGen.Emit(OpCodes.Callvirt, commandProviderCreateMethod); // call command provider update
        createMethodImplGen.Emit(OpCodes.Stloc_0); // store result in var in loc_0
                                                   // return action result depending on result
        createMethodImplGen.Emit(OpCodes.Ldloc_0); // load result from var in loc_0
        createMethodImplGen.Emit(OpCodes.Call, resultImplicitBoolMethod); // call implicit bool method
        createMethodImplGen.Emit(OpCodes.Brtrue_S, resultSuccessLabel); // if result impl bool method gives true jump to success
                                                                        // on failure return Problem(statusCode: DEFAULT_ERROR_CODE, detail: result.Message)
        createMethodImplGen.MarkLabel(resultFailureLabel);
        createMethodImplGen.Emit(OpCodes.Ldarg_0); // load this
        createMethodImplGen.Emit(OpCodes.Ldloca_S, 0); // get the result
        createMethodImplGen.Emit(OpCodes.Call, resultGetMessageMethod); // call the result message getter; message now on eval stack
        createMethodImplGen.Emit(OpCodes.Ldnull); // null for instance
        createMethodImplGen.Emit(OpCodes.Ldarg_0); // load this
        createMethodImplGen.Emit(OpCodes.Call, defaultErrorCodeGetMethod); // load status code 
        createMethodImplGen.Emit(OpCodes.Newobj, typeof(Nullable<int>).GetConstructor(new Type[] { typeof(int) })); // cast to int?; now on stack
        createMethodImplGen.Emit(OpCodes.Ldnull); // null for title
        createMethodImplGen.Emit(OpCodes.Ldnull); // null for type
        createMethodImplGen.Emit(OpCodes.Call, problemActionResultMethod); // call Problem
        createMethodImplGen.Emit(OpCodes.Br_S, finalLabel); // go to return
                                                            // on success, return Ok
        createMethodImplGen.MarkLabel(resultSuccessLabel);
        createMethodImplGen.Emit(OpCodes.Ldarg_0); // load this
        createMethodImplGen.Emit(OpCodes.Call, okActionResultMethod); // call Ok
                                                                      // final label for return
        createMethodImplGen.MarkLabel(finalLabel);
        createMethodImplGen.Emit(OpCodes.Ret); // return result on eval stack

        targetTypeBuilder.DefineMethodOverride(createMethodImplBuilder, interfaceCreateMethod);

        return targetTypeBuilder;
    }

    public void Dispose()
    {
        _moduleBuilder = null;
        GC.Collect(0, GCCollectionMode.Forced);
    }
}
