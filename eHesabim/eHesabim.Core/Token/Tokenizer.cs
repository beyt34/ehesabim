using System;
using System.Collections.Generic;
using System.Web;

namespace eHesabim.Core.Token {
    public class Tokenizer : ITokenizer {
        private readonly StringComparison stringComparison;

        public Tokenizer() {
            stringComparison = StringComparison.OrdinalIgnoreCase;
        }

        public string Replace(string template, IEnumerable<Token> tokens, bool htmlEncode) {
            if (string.IsNullOrWhiteSpace(template)) {
                throw new ArgumentNullException("template");
            }

            if (tokens == null) {
                throw new ArgumentNullException("tokens");
            }

            foreach (var token in tokens) {
                if (token == null || token.Value == null) {
                    continue;
                }

                var tokenValue = token.Value;
                ////do not encode URLs
                if (htmlEncode) {
                    tokenValue = HttpUtility.HtmlEncode(tokenValue);
                }

                template = Replace(template, string.Format(@"%{0}%", token.Key), tokenValue);
            }

            return template;
        }

        private string Replace(string original, string pattern, string replacement) {
            if (stringComparison == StringComparison.Ordinal) {
                return original.Replace(pattern, replacement);
            }

            int position0, position1;
            var count = position0 = 0;
            var inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            var chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = original.IndexOf(pattern, position0, stringComparison)) != -1) {
                for (var i = position0; i < position1; ++i) {
                    chars[count++] = original[i];
                }

                foreach (var t in replacement) {
                    chars[count++] = t;
                }

                position0 = position1 + pattern.Length;
            }

            if (position0 == 0) {
                return original;
            }

            for (var i = position0; i < original.Length; ++i) {
                chars[count++] = original[i];
            }

            return new string(chars, 0, count);
        }
    }
}
