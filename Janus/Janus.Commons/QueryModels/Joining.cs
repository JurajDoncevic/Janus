﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels
{
    public class Joining
    {
        private List<Join> _joins;

        public IReadOnlyCollection<Join> Joins => _joins;

        public Joining(List<Join> joins)
        {
            _joins = joins;
        }

        public Joining()
        {
            _joins = new List<Join>();
        }

        internal bool AddJoin(Join join)
        {
            // TODO: check logic for joins
            _joins.Add(join);
            return true;
        }
    }

    public class Join
    {
        private readonly string _primaryKeyTableauId;
        private readonly string _primaryKeyAttributeId;
        private readonly string _foreignKeyTableauId;
        private readonly string _foreignKeyAttributeId;

        public string PrimaryKeyTableauId => _primaryKeyTableauId;

        public string PrimaryKeyAttributeId => _primaryKeyAttributeId;

        public string ForeignKeyTableauId => _foreignKeyTableauId;

        public string ForeignKeyAttributeId => _foreignKeyAttributeId;

        internal Join(string primaryKeyTableauId!!, string primaryKeyAttributeId!!, 
                      string foreignKeyTableauId!!, string foreignKeyAttributeId!!)
        {
            _primaryKeyTableauId = primaryKeyTableauId;
            _primaryKeyAttributeId = primaryKeyAttributeId;
            _foreignKeyTableauId = foreignKeyTableauId;
            _foreignKeyAttributeId = foreignKeyAttributeId;
        }
    }
}
