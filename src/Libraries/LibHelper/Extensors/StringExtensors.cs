using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibHelper.Extensors
{
	/// <summary>
	///		Métodos de extensión para cadenas
	/// </summary>
	public static class StringExtensor
	{
		/// <summary>
		///		Comprueba si dos cadenas son iguales sin tener en cuenta las mayúsculas y tratando los nulos
		/// </summary>
		public static bool EqualsIgnoreCase(this string first, string second)
		{
			if (string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(second))
				return false;
			else if (!string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
				return false;
			else if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
				return true;
			else
				return first.Equals(second, StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Comprueba si dos cadenas son iguales sin tener en cuenta los nulos
		/// </summary>
		public static bool EqualsIgnoreNull(this string first, string second)
		{
			if (string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(second))
				return false;
			else if (!string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
				return false;
			else if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
				return true;
			else
				return first.Equals(second, StringComparison.CurrentCulture);
		}

		/// <summary>
		///		Compara dos valores 
		/// </summary>
		public static int CompareIgnoreNullTo(this string first, string second)
		{
			if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
				return 0;
			else if (string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(second))
				return -1;
			else if (!string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
				return 1;
			else
				return first.CompareTo(second);
		}

		/// <summary>
		///		Comprueba si una cadena es nula o está vacía (sin espacios)
		/// </summary>
		public static bool IsEmpty(this string value)
		{
			return string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim());
		}

		/// <summary>
		///		Pasa a mayúsculas el primer carácter de una cadena 
		/// </summary>
		public static string ToUpperFirstLetter(this string value)
		{
			if (value.IsEmpty())
				return "";
			else
				return value.Left(1).ToUpper() + value.From(1);
		}

		/// <summary>
		///		Comprueba el inicio de una cadena evitando los nulos
		/// </summary>
		public static bool StartsWithIgnoreNull(this string value, string start, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
		{
			return !string.IsNullOrEmpty(value) && value.StartsWith(start, comparison);
		}

		/// <summary>
		///		Quita los espacios ignorando los valores nulos
		/// </summary>
		public static string TrimIgnoreNull(this string value)
		{
			if (!IsEmpty(value))
				return value.Trim();
			else
				return "";
		}

		/// <summary>
		///		Obtiene un valor lógico a partir de una cadena
		/// </summary>
		public static bool GetBool(this string value, bool defaultValue = false)
		{
			if (value.EqualsIgnoreCase("yes"))
				return true;
			else if (value.EqualsIgnoreCase("no"))
				return false;
			else if (value.IsEmpty() || !bool.TryParse(value, out bool result))
				return defaultValue;
			else
				return result;
		}

		/// <summary>
		///		Obtiene un valor decimal para una cadena
		/// </summary>
		public static double? GetDouble(this string value)
		{
			if (string.IsNullOrEmpty(value) || !double.TryParse(value.Replace('.', ','), out double result))
				return null;
			else
				return result;
		}

		/// <summary>
		///		Obtiene un valor decimal para una cadena
		/// </summary>
		public static double GetDouble(this string value, double defaultValue)
		{
			return GetDouble(value) ?? defaultValue;
		}

		/// <summary>
		///		Obtiene un valor entero para una cadena
		/// </summary>
		public static int? GetInt(this string value)
		{
			if (string.IsNullOrEmpty(value) || !int.TryParse(value, out int result))
				return null;
			else
				return result;
		}

		/// <summary>
		///		Obtiene un valor entero para una cadena
		/// </summary>
		public static int GetInt(this string value, int defaultValue)
		{
			return GetInt(value) ?? defaultValue;
		}

		/// <summary>
		///		Obtiene un valor largo para una cadena
		/// </summary>
		public static long GetLong(this string value, long defaultValue)
		{
			return GetLong(value) ?? defaultValue;
		}

		/// <summary>
		///		Obtiene un valor largo para una cadena
		/// </summary>
		public static long? GetLong(this string value)
		{
			if (string.IsNullOrEmpty(value) || !long.TryParse(value, out long result))
				return null;
			else
				return result;
		}

		/// <summary>
		///		Obtiene una fecha para una cadena
		/// </summary>
		public static DateTime? GetDateTime(this string value)
		{
			if (string.IsNullOrEmpty(value) || !DateTime.TryParse(value, out DateTime result))
				return null;
			else
				return result;
		}

		/// <summary>
		///		Obtiene una fecha para una cadena
		/// </summary>
		public static DateTime GetDateTime(this string value, DateTime defaultValue)
		{
			return GetDateTime(value) ?? defaultValue;
		}

		/// <summary>
		///		Convierte la fecha y hora de una cadena utilizando un formato estricto
		/// </summary>
		public static DateTime? GetDateTime(this string value, string format, System.Globalization.DateTimeStyles style = System.Globalization.DateTimeStyles.AssumeUniversal)
		{
			if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, style, out DateTime result))
				return result;
			else
				return null;
		}

		/// <summary>
		///		Convierte la fecha y hora de una cadena con un formato estricto
		/// </summary>
		public static DateTime GetDateTime(this string value, string format, DateTime defaultValue, 
										   System.Globalization.DateTimeStyles style = System.Globalization.DateTimeStyles.AssumeUniversal)
		{
			return GetDateTime(value, format, style) ?? defaultValue;
		}

		/// <summary>
		///		Obtiene el valor de un enumerado
		/// </summary>
		public static TypeEnum GetEnum<TypeEnum>(this string value, TypeEnum defaultValue) where TypeEnum : struct
		{
			if (Enum.TryParse(value, true, out TypeEnum result))
				return result;
			else
				return defaultValue;
		}

		/// <summary>
		///		Obtiene una Url a partir de una cadena
		/// </summary>
		public static Uri GetUrl(this string url, Uri uriDefault = null)
		{
			// Convierte la Url
			if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri))
				uri = uriDefault;
			// Devuelv la Url
			return uri;
		}

		/// <summary>
		///		Añade una cadena a otra con un separador si es necesario
		/// </summary>
		public static string AddWithSeparator(this string value, string add, string separator, bool withSpace = true)
		{ 
			// Añade el separador
			if (!value.IsEmpty())
				value += separator + (withSpace ? " " : "");
			else // ... evita los posibles errores si la cadena es NULL
				value = "";
			// Devuelve la cadena con el valor añadido
			return value + add;
		}

		/// <summary>
		///		Corta una cadena hasta un separador. Devuelve la parte inicial de la cadena antes del separador
		///	y deja en la cadena target, a partir del separador
		/// </summary>
		public static string Cut(this string source, string separator, out string target)
		{
			int index;
			string cut = "";

				// Inicializa los valores de salida
				target = "";
				// Si hay algo que cortar ...
				if (!source.IsEmpty())
				{ 
					// Obtiene el índice donde se encuentra el separador
					index = source.IndexOf(separator, StringComparison.CurrentCultureIgnoreCase);
					// Corta la cadena
					if (index < 0)
						cut = source;
					else
						cut = source.Substring(0, index);
					// Borra al cadena cortada
					if ((cut + separator).Length - 1 < source.Length)
						target = source.Substring((cut + separator).Length);
					else
						target = "";
				}
				// Devuelve la primera parte de la cadena
				return cut;
		}

		/// <summary>
		///		Corta una cadena por una longitud
		/// </summary>
		/// <param name="source">Cadena original</param>
		/// <param name="length">Longitud por la que se debe cortar</param>
		/// <param name="last">Segunda parte de la cadena subString(length + 1) o vacío si no queda suficiente</param>
		/// <returns>Primera parte de la cadena (0..length)</returns>
		public static string Cut(this string source, int length, out string last)
		{ 
			// Inicializa la cadena de salida
			last = "";
			// Corta la cadena
			if (!source.IsEmpty() && source.Length > length)
			{
				last = source.Substring(length);
				source = source.Substring(0, length);
			}
			// Devuelve la primera parte de la cadena
			return source;
		}

		/// <summary>
		///		Separa una cadena cuando en partes cuando el separador es una cadena (no se puede utilizar Split)
		/// </summary>
		public static List<string> SplitByString(this string source, string separator)
		{
			List<string> results = new List<string>();

				// Corta la cadena
				if (!source.IsEmpty())
					do
					{
						// Corta la cadena
						source = source.Cut(separator, out string part);
						// Añade la parte localizada a la colección y continúa con la cadena restante
						if (!source.IsEmpty())
							results.Add(source);
						// Pasa al siguiente
						source = part;
					}
					while (!source.IsEmpty());
				// Devuelve la primera parte de la cadena
				return results;
		}

		/// <summary>
		///		Elimina una cadena del inicio de otra
		/// </summary>
		public static string RemoveStart(this string source, string start)
		{
			if (source.IsEmpty() || !source.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))
				return source;
			else if (source.Length == start.Length)
				return "";
			else
				return source.Substring(start.Length);
		}

		/// <summary>
		///		Elimina una cadena al final de otra
		/// </summary>
		public static string RemoveEnd(this string source, string end)
		{
			if (source.IsEmpty() || !source.EndsWith(end, StringComparison.CurrentCultureIgnoreCase))
				return source;
			else if (source.Length == end.Length)
				return "";
			else
				return source.Substring(0, source.Length - end.Length);
		}

		/// <summary>
		///		Reemplaza una cadena teniendo en cuenta el tipo de comparación
		/// </summary>
		public static string ReplaceWithStringComparison(this string source, string search, string replace,
														 StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
		{
			// Recorre la cadena sustituyendo los valores
			if (!source.IsEmpty() && !search.EqualsIgnoreCase(replace))
			{
				int index;
				int startIndex = 0;

					do
					{
						if ((index = source.IndexOf(search, startIndex, comparison)) >= 0)
						{
							string first = source.Cut(search, out string last);

								// Añade la cadena con el valor modificado
								source = first + replace + last;
								// Busca a partir del índice
								startIndex = index + replace.Length;
						}
					}
					while (index >= 0);
			}
			// Devuelve la cadena modificada
			return source;
		}

		/// <summary>
		///		Obtiene la parte izquierda de una cadena
		/// </summary>
		public static string Left(this string source, int length)
		{
			if (IsEmpty(source) || length <= 0)
				return "";
			else if (length > source.Length)
				return source;
			else
				return source.Substring(0, length);
		}

		/// <summary>
		///		Obtiene la parte derecha de una cadena
		/// </summary>
		public static string Right(this string source, int length)
		{
			if (IsEmpty(source) || length <= 0)
				return "";
			else if (length > source.Length)
				return source;
			else
				return source.Substring(source.Length - length, length);
		}

		/// <summary>
		///		Obtiene una cadena a partir de un carácter
		/// </summary>
		public static string From(this string source, int first)
		{
			if (IsEmpty(source) || first >= source.Length)
				return "";
			else if (first <= 0)
				return source;
			else
				return source.Substring(first);
		}

		/// <summary>
		///		Obtiene la cadena media
		/// </summary>
		public static string Mid(this string source, int first, int length)
		{
			return source.From(first).Left(length);
		}

		/// <summary>
		///		Codificar caracteres en Unicode
		/// </summary>
		public static string ToUnicode(this string value)
		{
			string result = "";

				// Codifica los caracteres
				foreach (char letter in value)
				{
					if (letter > 127)
						result += "\\u" + ((int) letter).ToString("x4");
					else
						result += letter;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Concatena una serie de líneas
		/// </summary>
		public static string Concatenate(this string value, List<string> sources, string separator = "\r\n", bool withSpace = false)
		{
			string result = "";

				// Concatena las cadenas
				if (sources != null)
					foreach (string source in sources)
						result = result.AddWithSeparator(source, separator, withSpace);
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Separa una serie de cadenas
		/// </summary>
		public static List<string> SplitToList(this string value, string separator = "\r\n", bool addEmpty = false)
		{
			List<string> results = new List<string>();
			List<string> sources = value.SplitByString(separator);

				// Añade las cadenas
				foreach (string source in sources)
					if (addEmpty || (!addEmpty && !source.TrimIgnoreNull().IsEmpty()))
						results.Add(source.TrimIgnoreNull());
				// Devuelve las cadenas
				return results;
		}

		/// <summary>
		///		Normaliza la cadena quitándole los acentos
		/// </summary>
		public static string NormalizeAccents(this string value)
		{
			const string withAccents = "ÁÉÍÓÚáéíóúÀÈÌÒÙàèìòùÄËÏÖÜäëïöü";
			const string withOutAccents = "AEIOUaeiouAEIOUaeiouAEIOUaeiou";
			int index;
			string result = "";

				// Normaliza la cadena
				if (!value.IsEmpty())
					foreach (char letter in value)
						if ((index = withAccents.IndexOf(letter)) >= 0)
							result += withOutAccents [index];
						else
							result += letter;
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Extrae las cadenas que se corresponden con un patrón
		/// </summary>
		public static List<string> Extract(this string source, string start, string end)
		{
			List<string> results = new List<string>();

				// Obtiene las coincidencias
				if (!source.IsEmpty())
					try
					{
						System.Text.RegularExpressions.Match match;

							// Crea la expresión de búsqueda
							match = System.Text.RegularExpressions.Regex.Match(source, start + "(.|\n)*?" + end,
																			   System.Text.RegularExpressions.RegexOptions.IgnoreCase |
																					System.Text.RegularExpressions.RegexOptions.CultureInvariant,
																			   TimeSpan.FromSeconds(1));
							// Mientras haya una coincidencia
							while (match.Success)
							{
								string value = match.Groups[0].Value.TrimIgnoreNull();

									// Quita la cadena inicial y final
									value = value.RemoveStart(start);
									value = value.RemoveEnd(end);
									// Añade la cadena encontrada
									results.Add(value.TrimIgnoreNull());
									// Pasa a la siguiente coincidencia
									match = match.NextMatch();
							}
					}
					catch (System.Text.RegularExpressions.RegexMatchTimeoutException exception)
					{
						System.Diagnostics.Debug.WriteLine($"Regex timeOut: {exception.Message}");
					}
				// Devuelve las coincidencias
				return results;
		}
	}
}
