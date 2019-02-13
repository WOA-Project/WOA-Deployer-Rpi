using System.Collections.Generic;
using System.IO;
using Superpower;

namespace Deployer.Execution
{
    public class ScriptParser : IScriptParser
    {
        private readonly Tokenizer<LangToken> tokenizer;

        public ScriptParser(Tokenizer<LangToken> tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public Script Parse(string input)
        {
            var tokenList = tokenizer.Tokenize(input);
            return Parsers.Script.Parse(tokenList);
        }

        static IEnumerable<string> Lines(string str)
        {
            using (var reader = new StringReader(str))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}