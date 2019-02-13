using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Deployer.Execution
{
    public static class Tokenizer
    {
        public static Tokenizer<LangToken> Create()
        {
            return new TokenizerBuilder<LangToken>()
                .Match(Span.Regex(@"(?:\r\n|\n)+"), LangToken.NewLine)
                .Match(Span.Regex(@"\s+"), LangToken.Space)
                .Match(Numerics.Integer, LangToken.Number)
                .Match(QuotedTextParser, LangToken.String)
                .Match(Character.EqualTo('='), LangToken.Equal)
                .Match(Character.EqualTo(':'), LangToken.Colon)
                .Match(Span.Regex(@"\w+[\d\w_]*"), LangToken.Identifier)
                .Build();
        }

        public static readonly TextParser<TextSpan> QuotedTextParser =
            from leading in Span.EqualToIgnoreCase("\"")
            from str in Span.Regex(@"(?:(?!\"").)*").OptionalOrDefault()
            from trailing in Span.EqualToIgnoreCase("\"")
            select str;
    }    
}

