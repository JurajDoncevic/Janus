using Janus.Commons.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels;

public interface IInsertCommandBuilder
{
    IInsertCommandBuilder WithInstatiation(Func<InstantiationBuilder, InstantiationBuilder> configuration);
    InsertCommand Build();
}

public interface IUpdateCommandBuilder
{
    IUpdateCommandBuilder WithMutation(Func<MutationBuilder, MutationBuilder> configuration);
    IUpdateCommandBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration);
    UpdateCommand Build();
}

public class CommandBuilder : IInsertCommandBuilder, IUpdateCommandBuilder
{ 

    public IInsertCommandBuilder WithInstatiation(Func<InstantiationBuilder, InstantiationBuilder> configuration)
    {
        throw new NotImplementedException();
    }

    public IUpdateCommandBuilder WithMutation(Func<MutationBuilder, MutationBuilder> configuration)
    {
        throw new NotImplementedException();
    }

    public IUpdateCommandBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration)
    {
        throw new NotImplementedException();
    }

    public UpdateCommand Build()
    {
        throw new NotImplementedException();
    }

    InsertCommand IInsertCommandBuilder.Build()
    {
        throw new NotImplementedException();
    }
}

public class MutationBuilder
{
}

public class InstantiationBuilder
{
}
