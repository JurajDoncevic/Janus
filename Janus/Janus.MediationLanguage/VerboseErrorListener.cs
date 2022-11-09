using Antlr4.Runtime.Misc;
using Antlr4.Runtime;

namespace Janus.MediationLanguage;
public class VerboseErrorListener : Antlr4.Runtime.BaseErrorListener
{
    public List<string> Errors { get; set; }
    public bool HasErrors
    {
        get
        {
            return Errors.Count != 0;
        }
    }
    public VerboseErrorListener() : base()
    {
        Errors = new List<string>();
    }
    public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {
        Errors.Add(string.Format("Syntax error in line {0},{1} at {2}. {3}", line.ToString(), charPositionInLine.ToString(), offendingSymbol.Text, msg));
        base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
    }
}