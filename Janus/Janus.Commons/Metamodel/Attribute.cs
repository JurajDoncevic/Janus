using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.Metamodel
{
    public class Attribute
    {
        private readonly string _name;
        private readonly DataType _dataType;
        private readonly bool _isPrimaryKey;
        private readonly bool _isNullable;
        private readonly int _ordinal;
        private readonly Tableau _tableau;
    }
}
