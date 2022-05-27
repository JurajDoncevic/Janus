using System.Runtime.Serialization;

namespace Janus.Commons.DataModels.Exceptions
{
    [Serializable]
    public class IncompatibleRowDataTypeException : Exception
    {
        public IncompatibleRowDataTypeException(List<string> valueKeys, List<string> typeKeys)
            : base($"Expected row data to be of type ({string.Join(",", typeKeys)}), " +
                   $"but got ({string.Join(",", valueKeys)})")
        {
        }
    }
}