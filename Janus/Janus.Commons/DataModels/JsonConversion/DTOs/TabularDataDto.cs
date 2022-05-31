using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels.JsonConversion.DTOs;

public class TabularDataDto
{
    public Dictionary<string, DataTypes> AttributeDataTypes { get; set; } = new Dictionary<string, DataTypes>();
    public List<Dictionary<string, object>> AttributeValues { get; set; } = new List<Dictionary<string, object>>();
}
