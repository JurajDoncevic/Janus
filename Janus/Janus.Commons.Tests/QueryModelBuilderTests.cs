using Janus.Commons.QueryModels;
using Janus.Commons.QueryModels.Exceptions;
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
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder))
                                        .AddTableau("tableau3", tableauBuilder =>
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

        [Fact(DisplayName = "Create query on one tableau")]
        public void CreateQueryOnOneTableau()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
            QueryBuilder.InitQueryOnTableau(tableauId, dataSource)
                        .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr2")
                                                    .AddAttribute("testDataSource.schema1.tableau1.attr1"))
                        .WithSelection(conf => conf.WithExpression("EXPRESSION"))
                        .Build();


        }


        [Fact(DisplayName = "Create cycle query")]
        public void CreateCycleQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<CyclicJoinNotSupportedException>(() =>
            {
                var query =
                QueryBuilder.InitQueryOnTableau(tableauId, dataSource)
                            .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau3.attr1", "testDataSource.schema2.tableau1.attr1")
                                                     .AddJoin("testDataSource.schema2.tableau1.attr1", "testDataSource.schema1.tableau1.attr1"))
                            .Build();
            });

        }
    }
}
