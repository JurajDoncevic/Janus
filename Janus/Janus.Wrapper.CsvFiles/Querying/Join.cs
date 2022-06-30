using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public class Join
{
    private readonly string _foreignKeyFilePath;
    private readonly string _foreignKeyColumnPath;
    private readonly string _primaryKeyFilePath;
    private readonly string _primaryKeyColumnPath;

    public Join(string foreignKeyFilePath!!, string foreignKeyColumnPath!!, string primaryKeyFilePath!!, string primaryKeyColumnPath!!)
    {
        _foreignKeyFilePath = foreignKeyFilePath;
        _foreignKeyColumnPath = foreignKeyColumnPath;
        _primaryKeyFilePath = primaryKeyFilePath;
        _primaryKeyColumnPath = primaryKeyColumnPath;
    }

    public string ForeignKeyFilePath => _foreignKeyFilePath;

    public string ForeignKeyColumnPath => _foreignKeyColumnPath;

    public string PrimaryKeyFilePath => _primaryKeyFilePath;

    public string PrimaryKeyColumnPath => _primaryKeyColumnPath;
}
