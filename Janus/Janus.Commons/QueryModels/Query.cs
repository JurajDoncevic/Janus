using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels
{
    public class Query
    {
        private Option<Projection> _projection;
        private Option<Selection> _selection;
        private Option<Joining> _joining;
        private string _onTableauId;

        public Query(string OnTableuId, Option<Projection> projection, Option<Selection> selection, Option<Joining> joining)
        {
            _onTableauId = OnTableuId;
            _projection = projection;
            _selection = selection;
            _joining = joining;
        }

        public Option<Projection> Projection => _projection;

        public Option<Selection> Selection => _selection;

        public Option<Joining> Joining => _joining;

        public string OnTableauId { get => _onTableauId; set => _onTableauId = value; }
    }
}
