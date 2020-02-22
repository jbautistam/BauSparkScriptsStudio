using System;

using Bau.Libraries.LibMarkupLanguage.Services.Tools;

namespace Bau.Libraries.LibMarkupLanguage.Services.JSON
{
	/// <summary>
	///		Intérprete de Json
	/// </summary>
	public class JsonParser : IParser
	{ 
		// Enumerados privados
		/// <summary>
		///		Tipos de token
		/// </summary>
		private enum TokenType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Llave de apertura</summary>
			BracketOpen,
			/// <summary>Llave de cierre</summary>
			BracketClose,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Dos puntos</summary>
			Colon,
			/// <summary>Coma</summary>
			Comma,
			/// <summary>Corchete de apertura</summary>
			BraceOpen,
			/// <summary>Corchete de cierre</summary>
			BraceClose,
			/// <summary>Cadena con el valor lógico "True"</summary>
			True,
			/// <summary>Cadena con el valor lógico "False"</summary>
			False,
			/// <summary>Valor numérico</summary>
			Numeric,
			/// <summary>Cadena con el valor "null"</summary>
			Null
		}
		// Variables privadas
		private Token _actualToken;
		private TokenType _idActualType;

		/// <summary>
		///		Interpreta un archivo
		/// </summary>
		public MLFile Parse(string fileName)
		{
			return new MLFile();
			// return ParseText(LibMarkupLanguage.Tools.FileHelper.LoadTextFile(fileName));
		}

		/// <summary>
		///		Interpreta un texto
		/// </summary>
		public MLFile ParseText(string text)
		{
			ParserTokenizer tokenizer = InitTokenizer();
			MLFile fileML = new MLFile();

				// Inicializa el contenido
				tokenizer.Init(text);
				// Interpreta el archivo
				fileML.Nodes.Add(ParseNode("Root", tokenizer, true));
				// Devuelve el archivo
				return fileML;
		}

		/// <summary>
		///		Interpreta un nodo
		/// </summary>
		private MLNode ParseNode(string name, ParserTokenizer tokenizer, bool searchBracket)
		{
			MLNode nodeML = new MLNode(name);

				// Captura el siguiente nodo
				if (!tokenizer.IsEof())
				{
					if (searchBracket)
					{   
						// Obtiene el siguiente token
						GetNextToken(tokenizer);
						// Debería ser una llave de apertura
						if (_idActualType == TokenType.BraceOpen)
							nodeML.Nodes.Add(ParseNodesArray(tokenizer));
						else if (_idActualType != TokenType.BracketOpen)
							throw new ParserException("Se esperaba una llave de apertura");
						else
							ParseNodeAttributes(tokenizer, nodeML);
					}
					else
						ParseNodeAttributes(tokenizer, nodeML);
				}
				// Devuelve el nodo interpretado
				return nodeML;
		}

		/// <summary>
		///		Interpreta los atributos de un nodo "id":"value","id":"value", ... ó "id":{object} ó "id":[array]
		/// </summary>
		private void ParseNodeAttributes(ParserTokenizer tokenizer, MLNode nodeMLParent)
		{
			bool end = false;

				// Obtiene los nodos
				while (!tokenizer.IsEof() && !end)
				{ 
					// Lee el siguiente Token, debería ser un identificador
					GetNextToken(tokenizer);
					// Comprueba que sea correcto
					if (_idActualType == TokenType.BracketClose) // ... es un objeto vacío
						end = true;
					else if (_idActualType != TokenType.String) // ... no se ha encontrado el identificador
						throw new ParserException("Se esperaba el identificador del elemento");
					else
					{
						MLAttribute attributeML = new MLAttribute();

							// Asigna el código del atributo
							attributeML.Name = _actualToken.Lexema;
							// Lee el siguiente token. Deberían ser dos puntos
							GetNextToken(tokenizer);
							// Comprueba que sea correcto
							if (_idActualType != TokenType.Colon)
								throw new ParserException("Se esperaban dos puntos (separador de identificador / valor)");
							else
							{ 
								// Lee el siguiente token...
								GetNextToken(tokenizer);
								// Interpreta el valor
								switch (_idActualType)
								{
									case TokenType.String:
									case TokenType.True:
									case TokenType.False:
									case TokenType.Numeric:
									case TokenType.Null:
											// Asigna el valor al atributo
											switch (_idActualType)
											{
												case TokenType.Null:
													attributeML.Value = "";
													break;
												case TokenType.String:
													attributeML.Value = ParseUnicode(_actualToken.Lexema);
													break;
												default:
													attributeML.Value = _actualToken.Lexema;
													break;
											}
											// Añade el atributo al nodo
											nodeMLParent.Attributes.Add(attributeML);
										break;
									case TokenType.BracketOpen: // ... definición de objeto
										MLNode nodeML = ParseNode(attributeML.Name, tokenizer, false);

											// Añade el nodo como objeto
											nodeMLParent.Nodes.Add(nodeML);
										break;
									case TokenType.BraceOpen: // ... definición de array
											nodeMLParent.Nodes.Add(ParseNodesArray(attributeML.Name, tokenizer));
										break;
									default:
										throw new ParserException($"Cadena desconocida. {_actualToken.Lexema}");
								}
							}
							// Lee el siguiente token
							GetNextToken(tokenizer);
							// Si es una coma, seguir con el siguiente atributo del nodo, si es una llave de cierre, terminar
							switch (_idActualType)
							{
								case TokenType.Comma:
									// ... no hace nada, simplemente pasa a la creación del siguiente nodo
									break;
								case TokenType.BracketClose:
									end = true;
									break;
								default:
									throw new ParserException("Cadena desconocida. " + _actualToken.Lexema);
							}
					}
				}
		}

		/// <summary>
		///		Interpreta los nodos de un array
		/// </summary>
		private MLNode ParseNodesArray(ParserTokenizer tokenizer)
		{
			return ParseNodesArray("Array", tokenizer);
		}

		/// <summary>
		///		Interpreta los nodos de un array
		/// </summary>
		private MLNode ParseNodesArray(string nodeParent, ParserTokenizer tokenizer)
		{
			MLNode nodeML = new MLNode(nodeParent);
			bool end = false;
			int index = 0;

				// Obtiene el siguiente token (puede que se trate de un array vacío)
				while (!tokenizer.IsEof() && !end)
				{ 
					// Obtiene el siguiente token
					GetNextToken(tokenizer);
					// Interpreta el nodo
					switch (_idActualType)
					{
						case TokenType.BracketOpen:
								nodeML.Nodes.Add(ParseNode("Struct", tokenizer, false));
							break;
						case TokenType.BraceOpen:
								nodeML.Nodes.Add(ParseNodesArray(tokenizer));
							break;
						case TokenType.String:
						case TokenType.Numeric:
						case TokenType.True:
						case TokenType.False:
						case TokenType.Null:
								nodeML.Nodes.Add("Item", _actualToken.Lexema);
							break;
						case TokenType.Comma: // ... no hace nada, simplemente pasa al siguiente incrementando el índice
								index++;
							break;
						case TokenType.BraceClose: // ... corchete de cierre, indica que ha terminado
								end = true;
							break;
						default:
							throw new NotImplementedException($"No se ha encontrado un token válido ('{_actualToken.Lexema}')");
					}
				}
				// Si no se ha encontrado un corchete, lanza una excepción
				if (!end)
					throw new ParserException("No se ha encontrado el carácter de fin del array ']'");
				// Devuelve la colección de nodos
				return nodeML;
		}

		/// <summary>
		///		Obtiene los datos del siguiente token
		/// </summary>
		private void GetNextToken(ParserTokenizer tokenizer)
		{
			_actualToken = tokenizer.GetToken();
			_idActualType = GetIDType(_actualToken);
		}

		/// <summary>
		///		Obtiene el tipo de un token
		/// </summary>
		private TokenType GetIDType(Token token)
		{
			if (token != null && token.Definition != null)
				return (TokenType) (token.Definition.Type ?? 0);
			else
				return TokenType.Unknown;
		}

		/// <summary>
		///		Interpreta una cadena Unicode
		/// </summary>
		private string ParseUnicode(string value)
		{
			int indexOf;
			bool end = false;

			// Convierte los caracteres Unicode de la cadena
			do
			{ // Indica que ya se ha terminado
				end = true;
				// Obtiene el índice de la cadena Unicode
				indexOf = value.IndexOf("\\u", StringComparison.CurrentCultureIgnoreCase);
				// Si se ha obtenido algo, comprueba que se puedan coger cuatro caracteres
				if (indexOf >= 0 && indexOf + 6 <= value.Length)
				{
					string strUnicode = value.Substring(indexOf, 6);
					string strFirst;

					// Obtiene la parte izquierda de la cadena (con el carácter Unicode convertido)
					strFirst = value.Substring(0, indexOf) +
											(char) int.Parse(strUnicode.Substring(2), System.Globalization.NumberStyles.HexNumber);
					// Obtiene la parte derecha de la cadena
					if (value.Length >= indexOf + 6)
						value = value.Substring(indexOf + 6);
					// Obtiene la cadena completa
					value = strFirst + value;
					// Indica que debe continuar
					end = false;
				}
			}
			while (!end);
			// Convierte los saltos de línea
			value = value.Replace("\n", Environment.NewLine);
			// Devuelve la cadena convertida		
			return value;
		}

		/// <summary>
		///		Inicializa el objeto de creación de tokens
		/// </summary>
		private ParserTokenizer InitTokenizer()
		{
			ParserTokenizer tokenizer = new ParserTokenizer();

				// Asigna los tokens
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.BracketOpen, "{"));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.BracketClose, "}"));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.String, "\"", "\""));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.Colon, ":"));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.Comma, ","));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.BraceOpen, "["));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.BraceClose, "]"));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.True, "true"));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.False, "false"));
				tokenizer.TokensDefinitions.Add(GetTokenDefinition(TokenType.Null, "null"));
				tokenizer.TokensDefinitions.Add((int) TokenType.Numeric, "Numeric");
				// Devuelve el objeto de creación de tokens
				return tokenizer;
		}

		/// <summary>
		///		Definición del token
		/// </summary>
		private TokenDefinition GetTokenDefinition(TokenType idToken, string start, string end = null)
		{
			return new TokenDefinition((int) idToken, idToken.ToString(), start, end);
		}
	}
}
