using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.SelectionExpressions;

public class SelectionExpressions
{
    public static NotOperation NOT(SelectionExpression expression)
        => new NotOperation(expression);

    public static AndOperation AND(SelectionExpression leftOperand, SelectionExpression rightOperand)
        => new AndOperation(leftOperand, rightOperand);

    public static OrOperation OR(SelectionExpression leftOperand, SelectionExpression rightOperand)
        => new OrOperation(leftOperand, rightOperand);

    public static NotEqualAs NEQ(string attributeId, object value)
    => new NotEqualAs(attributeId, value);

    public static EqualAs EQ(string attributeId, object value)
        => new EqualAs(attributeId, value);

    public static GreaterOrEqualThan GE(string attributeId, object value)
        => new GreaterOrEqualThan(attributeId, value);

    public static GreaterThan GT(string attributeId, object value)
        => new GreaterThan(attributeId, value);

    public static LesserOrEqualThan LE(string attributeId, object value)
        => new LesserOrEqualThan(attributeId, value);

    public static LesserThan LT(string attributeId, object value)
        => new LesserThan(attributeId, value);
}
