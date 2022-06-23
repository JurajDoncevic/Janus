using Janus.Commons.SchemaModels;
using Janus.Wrapper.Core.SchemaInferrence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core.SchemaInferrence;
public class SchemaInferrer
{
    private readonly ISchemaModelProvider _provider;

    public SchemaInferrer(ISchemaModelProvider schemaModelProvider)
    {
        _provider = schemaModelProvider;
    }

    public Result<DataSource> InferSchemaModel()
    {
        return Result<DataSource>.OnFailure<DataSource>();
    }
}
