using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.Metamodel
{
    public class Schema
    {
        private readonly string _name;
        private List<Tableau> _tableaus;

        public string Name { get; set; }
        public ReadOnlyCollection<Tableau> Tableaus => _tableaus.AsReadOnly();

    }
}
