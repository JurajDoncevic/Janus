namespace Janus.Components.Persistence;

/// <summary>
/// Base persistence interface
/// </summary>
/// <typeparam name="TId">Type of Id in persistence</typeparam>
/// <typeparam name="TModel">Type of model that is given and returned</typeparam>
public interface IPersistence<TId, TModel>
{
    /// <summary>
    /// Gets all models from persistence
    /// </summary>
    /// <returns></returns>
    Result<IEnumerable<TModel>> GetAll();
    /// <summary>
    /// Gets a model with given id from persistence
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Result<TModel> Get(TId id);
    /// <summary>
    /// Inserts the given model into persistence
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Result Insert(TModel model);
    /// <summary>
    /// Deletes a model with given id from persistence
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Result Delete(TId id);
    /// <summary>
    /// Checks if a model with given id exists in persistence
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool Exists(TId id);
}
