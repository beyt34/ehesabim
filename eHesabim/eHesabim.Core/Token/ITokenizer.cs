using System.Collections.Generic;

namespace eHesabim.Core.Token {
    public interface ITokenizer {
        string Replace(string template, IEnumerable<Token> tokens, bool htmlEncode);
    }
}