using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SchemaModels.JsonConversion.DTOs
{
    internal class AttributeDTO
    {
        public string Name { get; set; }
        public DataTypes DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }
        public int Ordinal { get; set; }
    }
}
