using System;

namespace Bau.Libraries.LibMarkupLanguage.Services.Tools
{
	/// <summary>
	///		Clase de interpretación de cadenas como token
	/// </summary>
	internal class ParserTokenizer
	{ 
		// Constantes privadas
		private const string CharsNumeric = "0123456789+-.";
		private static string [] Spaces = new string [] { " ", "\r", "\n", "\t" };
		// Variables privadas
		private string _content;
		private int _line, _charIndex;

		/// <summary>
		///		Inicializa los valores a interpretar
		/// </summary>
		internal void Init(string content)
		{ 
			// Inicializa la cadena
			_content = content;
			// Inicializa las variables
			_charIndex = 0;
			_line = 1;
		}

		/// <summary>
		///		Obtiene un token
		/// </summary>
		internal Token GetToken()
		{
			Token token = new Token();

				// Quita los espacios
				ConsumeSpaces();
				// Recoge un token
				if (IsEof())
					token.SystemType = Token.TokenType.Eof;
				else
				{
					bool end = false;
					TokenDefinition tokenNumeric = TokensDefinitions.SearchNumeric();

						// Inicializa el tipo de token
						token.SystemType = Token.TokenType.Unknown;
						// Busca el token
						foreach (TokenDefinition definition in TokensDefinitions)
							if (!end)
							{
								if (tokenNumeric != null && NextCharIsNumeric())
								{ 
									// Asigna el tipo
									token.SystemType = Token.TokenType.Numeric;
									token.Definition = tokenNumeric;
									// Asigna el número
									token.Lexema = ConsumeNumeric();
									// Indica que ha terminado
									end = true;
								}
								else if (!definition.IsNumeric && GetChars(definition.Start.Length) == definition.Start)
								{ 
									// Asigna el tipo
									token.SystemType = Token.TokenType.Defined;
									token.Definition = definition;
									// Asigna el contenido
									token.Lexema = definition.Start;
									// Consume la cadena
									Consume(definition.Start);
									// Busca hasta el final
									if (!string.IsNullOrEmpty(definition.End))
									{   
										// En el lexema mete el contenido
										if (definition.End == "\"")
											token.Lexema = ConsumeString();
										else
										{ 
											// Obtiene el contenido
											token.Lexema = ConsumeTo(definition.End);
											// Consume la cadena de fin
											Consume(definition.End);
										}
									}
									// Indica que se ha terminado
									end = true;
								}
							}
						// Si no se ha terminado consume hasta el siguiente espacio
						if (!end)
							token.Lexema = ConsumeTo(Spaces);
				}
				// Devuelve el token
				return token;
		}

		/// <summary>
		///		Comprueba si el siguiente carácter es numérico
		/// </summary>
		private bool NextCharIsNumeric()
		{
			return CharsNumeric.IndexOf(GetChars(1)) >= 0;
		}

		/// <summary>
		///		Consume una cadena teniendo en cuenta los saltos
		/// </summary>
		private string ConsumeString()
		{
			string value = "";
			bool end = false;

				// Busca las comillas finales
				while (!end && !IsEof())
				{ 
					// Se salta el carácter que vaya después de una \
					if (_content [_charIndex] == '\\')
					{
						_charIndex++;
						if (!IsEof())
						{ 
							// Añade el siguiente carácter
							switch (_content [_charIndex])
							{
								case 'u':
								case 'n':
								case 't':
								case 'r':
									value += "\\" + _content [_charIndex];
									break;
								default:
									value += _content [_charIndex];
									break;
							}
							// Incrementa el índice del carácter
							_charIndex++;
						}
					}
					else
					{ 
						// Comprueba si ha llegado al final o añade el carácter
						if (_content [_charIndex] == '\"')
							end = true;
						else
							value += _content [_charIndex];
						// Incrementa el índice de caracteres
						_charIndex++;
					}
				}
				// Devuelve la cadena
				return value;
		}

		/// <summary>
		///		Devuelve la cadena que se encuentra hasta encontrar otra
		/// </summary>
		private string ConsumeTo(string value)
		{
			return ConsumeTo(new string [] { value });
		}

		/// <summary>
		///		Devuelve la cadena que se encuentra hasta encontrar otra
		/// </summary>
		private string ConsumeTo(string [] values)
		{
			string value = "";
			bool end = false;

				// Consume la cadena hasta encontrar alguna de las pasadas como parámetros
				do
				{ 
					// Comprueba si la siguiente cadena es la especificada
					foreach (string searchValue in values)
						if (GetChars(searchValue.Length).Equals(searchValue, StringComparison.CurrentCultureIgnoreCase))
							end = true;
					// Si no se ha terminado añade el siguiente carácter
					if (!end)
					{
						value += GetChars(1);
						Consume(1);
					}
				}
				while (!end && !IsEof());
				// Devuelve la cadena
				return value;
		}

		/// <summary>
		///		Devuelve una cadena con números
		/// </summary>
		private string ConsumeNumeric()
		{
			string value = "";
			bool end = false;

				// Consume la cadena hasta encontrar algo que no es un dígito
				do
				{
					string nextChar = GetChars(1);

						// Si es numérico, lo pasa a la cadena, si no indica que se ha terminado
						if (CharsNumeric.IndexOf(nextChar) >= 0)
						{ 
							// Asigna el cáracter
							value += nextChar;
							// Lo quita de la cadena pendiente
							Consume(1);
						}
						else
							end = true;
				}
				while (!end && !IsEof());
				// Devuelve la cadena
				return value;
		}

		/// <summary>
		///		Consume los espacios
		/// </summary>
		private void ConsumeSpaces()
		{
			while (!IsEof() && IsSpace(GetChars()))
			{ 
				// Si es un salto de línea incrementa el número de línea
				if (GetChars() == "\n")
					_line++;
				// Consume el carácter
				Consume(1);
			}
		}

		/// <summary>
		///		Obtiene un carácter (pero no lo consume)
		/// </summary>
		private string GetChars()
		{
			return GetChars(1);
		}

		/// <summary>
		///		Obtiene n caracteres (pero no los consume)
		/// </summary>
		private string GetChars(int length)
		{
			if (_charIndex + length < _content.Length)
				return GetCharsSkipBar(_content, _charIndex, length);
			else
				return GetCharsSkipBar(_content, _charIndex, 1);
		}

		/// <summary>
		///		Obtiene una cadena de n caracteres saltándose los caracteres \
		/// </summary>
		private string GetCharsSkipBar(string content, int indexStart, int length)
		{
			int read = 0;
			string value = "";

				// Recorre la cadena obteniendo los caracteres
				while (read < length && indexStart < content.Length)
				{ 
					// Si no es un carácter de \ añade el contenido
					if (content [indexStart] != '\\')
					{
						value += content [indexStart];
						read++;
					}
					// Pasa al siguiente carácter
					indexStart++;
				}
				// Devuelve el valor
				return value;
		}

		/// <summary>
		///		Consume los caracteres de la cadena
		/// </summary>
		private void Consume(string value)
		{
			Consume(value.Length);
		}

		/// <summary>
		///		Consume un número de caracteres
		/// </summary>
		private void Consume(int length)
		{
			int read = 0;

				while (read < length && _charIndex < _content.Length)
				{ 
					// Incrementa el índice de lectura cuando no está en una barra
					if (_content [_charIndex] != '\\')
						read++;
					// Incrementa el índice global
					_charIndex++;
				}
		}

		/// <summary>
		///		Indica si se ha llegado al fin de archivo
		/// </summary>
		internal bool IsEof()
		{
			return _charIndex >= _content.Length;
		}

		/// <summary>
		///		Comprueba si el carácter es un espacio
		/// </summary>
		private bool IsSpace(string charCompare)
		{ 
			// Comprueba si es un espacio
			foreach (string value in Spaces)
				if (charCompare == value)
					return true;
			// Si ha llegado hasta aquí es porque no es un espacio
			return false;
		}

		/// <summary>
		///		Definiciones de token
		/// </summary>
		internal TokenDefinitionsCollection TokensDefinitions { get; } = new TokenDefinitionsCollection();
	}
}
