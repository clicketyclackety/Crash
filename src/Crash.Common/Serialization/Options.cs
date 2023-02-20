using System.Text.Json.Serialization;
using System.Text.Json;

namespace Crash.Common.Serialization
{
    
    internal static class Options
    {

        static Options()
        {
            Default = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true, // TODO : Should this be avoided? Does it add extra memory?
            };
        }

        internal readonly static JsonSerializerOptions Default;

    }

}
