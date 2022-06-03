using Janus.Commons.CommandModels.JsonConversion;
using Janus.Commons.SchemaModels;
using System.Text.Json.Serialization;

namespace Janus.Commons.CommandModels
{
    public abstract class BaseCommand
    {
        protected readonly string _onTableauId;

        protected BaseCommand(string onTableauId!!)
        {
            _onTableauId = onTableauId;
        }

        public string OnTableauId => _onTableauId;

        public abstract Result IsValidForDataSource(DataSource dataSource);
    }
}