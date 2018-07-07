using System.Linq;
using System.Text;

namespace FileWatcher.Utilities
{
    public static class StringExtensions
    {
        public static string ToActorName(this string input)
        {
            // Actor paths MUST: not start with$, include only ASCII letters and can only contain these special characters: -_.*$+:@&=,!~';.
            if (input[0].Equals('$')) input = input.Remove(0, 1);
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(input)).RemoveSpecialCharacters();
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            var sb = new StringBuilder();
            var allowedSpecials = new[] {'-', '_', '.', '*', '$', '+', ':', '@', '&', '=', ',', '!', '~', '\'', ';'};
            foreach (var c in str)
                if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || allowedSpecials.Contains(c))
                    sb.Append(c);
            return sb.ToString();
        }
    }
}
