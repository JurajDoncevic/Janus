using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.Metamodel
{
    public class Relationship
    {
        private string _name;
        private readonly Attribute _referencedAttribute;
        private readonly Attribute _referencingAttribute;
    }
}
