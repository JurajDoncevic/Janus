using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;

namespace Janus.Commons.Tests
{
    public class CommandModelBuilderTests
    {

        private DataSource GetSchemaModel() =>
            SchemaModelBuilder.InitDataSource("dataSource")
                .AddSchema("schema1", 
                    schemaConf => schemaConf.AddTableau("tableau1", 
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(true))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false)))
                    .AddTableau("tableau2", 
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false))))
                .Build();
                                            

        [Fact(DisplayName = "Construct a valid insert command")]
        public void ConstructSimpleInsertCommand()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TableauDataBuilder.InitTableauData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", "TEST_STRING2" }, { "attr3", 2.3 } }))
                                  .Build();

            var insertCommand =
                InsertCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                    .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                    .Build();

            Assert.NotNull(insertCommand);
            Assert.Equal(tableauId, insertCommand.OnTableauId);
            Assert.NotNull(insertCommand.Instantiation);
                    
        }

        [Fact(DisplayName = "Construct a valid insert command with open builder")]
        public void ConstructSimpleInsertCommandWithOpenBuilder()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TableauDataBuilder.InitTableauData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", "TEST_STRING2" }, { "attr3", 2.3 } }))
                                  .Build();

            var insertCommand =
                InsertCommandOpenBuilder.InitOpenInsert(tableauId)
                                    .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                    .Build();

            Assert.NotNull(insertCommand);
            Assert.True(insertCommand.IsValidForDataSource(dataSource));
            Assert.Equal(tableauId, insertCommand.OnTableauId);
            Assert.NotNull(insertCommand.Instantiation);

        }


        [Fact(DisplayName = "Construct a valid update command")]
        public void ConstructSimpleUpdateCommand()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attr3", 1.9 } };

            var updateCommand =
                UpdateCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                    .WithMutation(conf => conf.WithValues(valueUpdates))
                                    .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                    .Build();

            Assert.NotNull(updateCommand);
            Assert.Equal(tableauId, updateCommand.OnTableauId);
            Assert.NotNull(updateCommand.Mutation);
            Assert.True(updateCommand.Selection);

        }

        [Fact(DisplayName = "Construct a valid update command with open builder")]
        public void ConstructSimpleUpdateCommandWithOpenBuilder()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attr3", 1.9 } };

            var updateCommand =
                UpdateCommandOpenBuilder.InitOpenUpdate(tableauId)
                                    .WithMutation(conf => conf.WithValues(valueUpdates))
                                    .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                    .Build();

            Assert.NotNull(updateCommand);
            Assert.Equal(tableauId, updateCommand.OnTableauId);
            Assert.NotNull(updateCommand.Mutation);
            Assert.True(updateCommand.Selection);
            Assert.True(updateCommand.IsValidOnDataSource(dataSource));

        }
    }
}
