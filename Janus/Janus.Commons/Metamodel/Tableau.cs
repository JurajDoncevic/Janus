using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.Metamodel
{
    public class Tableau
    {
        private readonly string _name;
        private List<Attribute> _attributes;
        private readonly Schema _schema;
        private List<Relationship> _relationships;
    }
}
