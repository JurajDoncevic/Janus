namespace Janus.Mediation.QueryMediationModels;
public class MediationJoin
{
    private readonly string _primaryKeyTabularDataName;
    private readonly string _primaryKeyColumnName;
    private readonly string _foreignKeyTabularDataName;
    private readonly string _foreignKeyColumnName;

    public MediationJoin(string primaryKeyTabularDataName, string primaryKeyColumnName, string foreignKeyTabularDataName, string foreignKeyColumnName)
    {
        _primaryKeyTabularDataName = primaryKeyTabularDataName;
        _primaryKeyColumnName = primaryKeyColumnName;
        _foreignKeyTabularDataName = foreignKeyTabularDataName;
        _foreignKeyColumnName = foreignKeyColumnName;
    }

    public string PrimaryKeyTabularDataName => _primaryKeyTabularDataName;

    public string PrimaryKeyColumnName => _primaryKeyColumnName;

    public string ForeignKeyTabularDataName => _foreignKeyTabularDataName;

    public string ForeignKeyColumnName => _foreignKeyColumnName;
}
