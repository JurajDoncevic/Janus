namespace Janus.Commons.SelectionExpressions;

public class Expressions
{
    /// <summary>
    /// NOT logical operator
    /// </summary>
    /// <param name="expression">Operand expression</param>
    /// <returns></returns>
    public static NotOperator NOT(SelectionExpression expression)
        => new NotOperator(expression);

    /// <summary>
    /// AND logical operator
    /// </summary>
    /// <param name="leftOperand">Left operand expression</param>
    /// <param name="rightOperand">Right operand expression</param>
    /// <returns></returns>
    public static AndOperator AND(SelectionExpression leftOperand, SelectionExpression rightOperand)
        => new AndOperator(leftOperand, rightOperand);

    /// <summary>
    /// OR logical operator
    /// </summary>
    /// <param name="leftOperand">Left operand expression</param>
    /// <param name="rightOperand">Right operand expression</param>
    /// <returns></returns>
    public static OrOperator OR(SelectionExpression leftOperand, SelectionExpression rightOperand)
        => new OrOperator(leftOperand, rightOperand);

    /// <summary>
    /// NEQ (not equal) comparison operator
    /// </summary>
    /// <param name="attributeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static NotEqualAs NEQ(string attributeId, object value)
    => new NotEqualAs(attributeId, value);

    /// <summary>
    /// EQ (equal) comparison operator
    /// </summary>
    /// <param name="attributeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static EqualAs EQ(string attributeId, object value)
        => new EqualAs(attributeId, value);

    /// <summary>
    /// GE (greater or equal) comparison operator
    /// </summary>
    /// <param name="attributeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static GreaterOrEqualThan GE(string attributeId, object value)
        => new GreaterOrEqualThan(attributeId, value);

    /// <summary>
    /// GT (greater than) comparison operator
    /// </summary>
    /// <param name="attributeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static GreaterThan GT(string attributeId, object value)
        => new GreaterThan(attributeId, value);

    /// <summary>
    /// LE (lesser or equal) comparison operator
    /// </summary>
    /// <param name="attributeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LesserOrEqualThan LE(string attributeId, object value)
        => new LesserOrEqualThan(attributeId, value);

    /// <summary>
    /// LT (lesser than) comparison operator
    /// </summary>
    /// <param name="attributeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LesserThan LT(string attributeId, object value)
        => new LesserThan(attributeId, value);

    /// <summary>
    /// TRUE literal
    /// </summary>
    /// <returns></returns>
    public static TrueLiteral TRUE()
        => new TrueLiteral();

    /// <summary>
    /// FALSE literal
    /// </summary>
    /// <returns></returns>
    public static FalseLiteral FALSE()
        => new FalseLiteral();
}
