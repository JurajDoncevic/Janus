using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Commons.Tests
{
    public class QueryModelBuilderTests
    {
        private DataSource GetExampleSchema()
            => SchemaModelBuilder.InitDataSource("testDataSource")
                                .AddSchema("schema1", schemaBuilder =>
                                    schemaBuilder
                                        .AddTableau("tableau1", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder))
                                        .AddTableau("tableau2", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder)))
                                .AddSchema("schema2", schemaBuilder =>
                                    schemaBuilder
                                        .AddTableau("tableau1", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder))
                                        .AddTableau("tableau2", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder)))
                                .Build();

        [Fact(DisplayName = "Create query containing all components")]
        public void CreateAllComponentsQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            QueryBuilder.InitQueryOnTableau(tableauId, dataSource)
                        .WithProjection(conf => conf.AddAttribute("attr2")
                                                    .AddAttribute("attr1"))
                        .WithSelection(conf => conf.WithExpression("EXPRESSION"))
        }
    }
}
