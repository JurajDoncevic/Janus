using Janus.Commons.CommandModels.JsonConversion;
using System.Text.Json.Serialization;

namespace Janus.Commons.CommandModels
{
    [JsonConverter(typeof(UpdateCommandJsonConverter))]
    public abstract class BaseCommand
    {
        protected readonly string _onTableauId;

        protected BaseCommand(string onTableauId!!)
        {
            _onTableauId = onTableauId;
        }

        public string OnTableauId => _onTableauId;

    }
}