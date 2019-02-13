using Superpower;
using Superpower.Parsers;

namespace Deployer.Execution
{
    public static class Parsers
    {
        public static TokenListParser<LangToken, string> String => Token.EqualTo(LangToken.String).Select(x =>
        {
            var stringValue = x.ToStringValue();
            return stringValue.Substring(1, stringValue.Length-2);
        });
        public static TokenListParser<LangToken, string> Identifier => Token.EqualTo(LangToken.Identifier).Select(x => x.ToStringValue());
        public static TokenListParser<LangToken, string> Number => Token.EqualTo(LangToken.Number).Select(x => x.ToStringValue());

        public static TokenListParser<LangToken, string> Value => String.Or(Number).Or(Identifier);

        public static TokenListParser<LangToken, Argument> Argument =>
            from t in Value
            select new Argument(t);

        public static TokenListParser<LangToken, Argument[]> Arguments =>
            from _ in Token.EqualTo(LangToken.Space)
            from t in Argument.ManyDelimitedBy(Token.EqualTo(LangToken.Space))
            select t;

        public static TokenListParser<LangToken, Command> Command =>
            from name in Identifier
            from args in Arguments.OptionalOrDefault()
            select new Command(name, args ?? new Argument[0]);
        
        public static TokenListParser<LangToken, Sentence> Sentence => CommandSentence;
        
        private static TokenListParser<LangToken, Sentence> CommandSentence => Command.Select(x => new Sentence(x));

        public static TokenListParser<LangToken, Script> Script =>
            from cmds in Sentence.ManyDelimitedBy(Token.EqualTo(LangToken.NewLine))
                .AtEnd()
            select new Script(cmds);
    }    
}

