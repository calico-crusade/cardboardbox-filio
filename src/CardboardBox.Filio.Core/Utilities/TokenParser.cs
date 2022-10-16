namespace CardboardBox.Filio.Core.Utilities
{
	public class TokenParser
	{
		private readonly TokenParserConfig _config;
		private int _currentIndex = 0;

		public string Input { get; }

		public char StartToken => _config.StartToken;
		public char EndToken => _config.EndToken;
		public char MidToken => _config.MidToken;
		public char EscapeToken => _config.EscapeToken;

		public TokenParser(string input, TokenParserConfig? config = null)
		{
			Input = input;
			_config = config ?? new();
		}

		public IEnumerable<TokenOutput> Tokens()
		{
			_currentIndex = 0;
			var token = FindNextToken();
			if (token == null) yield break;

			while(true)
			{
				yield return token;
				token = FindNextToken();
				if (token == null)
					break;
			}
		}

		public int IndexOf(char token)
		{
			return Input.IndexOf(token, _currentIndex);
		}

		public int IndexOf(char token, int startIndex)
		{
			if (startIndex >= Input.Length) return -1;
			return Input.IndexOf(token, startIndex);
		}

		public bool PreviousWas(char token, int index)
		{
			var i = index - 1;
			if (i < 0) return false;

			return Input[i] == token;
		}

		public TokenOutput? FindNextToken()
		{
			var ts = IndexOf(StartToken);
			if (ts == -1) return null;

			if (PreviousWas(EscapeToken, ts))
			{
				_currentIndex = ts + 1;
				return FindNextToken();
			}
			
			var te = IndexOf(EndToken, ts);
			if (te == -1) return null;

			_currentIndex = te;

			var len = te - ts;
			var token = Input.Substring(ts + 1, len - 1);

			var tm = token.IndexOf(MidToken);
			if (tm == -1) return new TokenOutput(token, null, ts, len + 1);

			var tag = token.Substring(0, tm);
			var format = token.Substring(tm + 1);
			return new TokenOutput(tag, format, ts, len + 1);
		}

		public record class TokenOutput(string Tag, string? Options, int StartIndex, int Length);
	}

	public class TokenParserConfig
	{
		public char StartToken { get; set; } = '{';
		public char EndToken { get; set; } = '}';
		public char MidToken { get; set; } = ':';
		public char EscapeToken { get; set; } = '\\';

		public TokenParserConfig() { }

		public TokenParserConfig(char startToken, char endToken, char midToken, char escapeToken)
		{
			StartToken = startToken;
			EndToken = endToken;
			MidToken = midToken;
			EscapeToken = escapeToken;
		}
	}
}
