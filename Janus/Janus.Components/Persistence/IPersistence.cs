namespace Janus.Components.Persistence;
public interface IPersistence<TId, TModel>
{
    Result<IEnumerable<TModel>> GetAll();
    Result<TModel> Get(TId id);
    Result Insert(TModel model);
    Result Delete(TId id);
}
