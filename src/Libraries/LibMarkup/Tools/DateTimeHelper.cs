using System;
using System.Globalization;

namespace Bau.Libraries.LibMarkupLanguage.Tools
{
	/// <summary>
	///		Proporciona métodos para generar e interpretar la información de fecha y hora
	/// </summary>
	/// <remarks>
	///     Ver <a href="http://www.ietf.org/rfc/rfc0822.txt">RFC #822</a> el estándar para mensajes de texto de ARPA
	///     y <a href="http://www.ietf.org/rfc/rfc3339.txt">RFC #3339</a>: fecha y hora en Internet para más información
	///     sobre los formatos de fecha y hora implementados
	/// </remarks>
	public static class DateTimeHelper
	{
		/// <summary>
		///		Convierte la fecha en el formato RFC-3339
		/// </summary>
		public static string ToStringRfc3339(DateTime value)
		{
			DateTimeFormatInfo daterFormat = CultureInfo.InvariantCulture.DateTimeFormat;

				// Devuelve la cadena con la fecha formateada en RFC-3339
				if (value.Kind == DateTimeKind.Local)
					return value.ToString("yyyy'-'MM'-'dd'T'HH:mm:ss.ffzzz", daterFormat);
				else
					return value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.ff'Z'", daterFormat);
		}

		/// <summary>
		///		Convierte una representación de una fecha formateada con RFC-3339 en un DateTime
		/// </summary>
		public static DateTime ParseRfc3339(string value)
		{
			if (DateTimeHelper.TryParseRfc3339(value, out DateTime result))
				return result;
			else
				throw new FormatException();
		}

		/// <summary>
		///		Convierte una cadena en formato RFC-3339 en una fecha
		///		Ejemplo de cadena en el formato: Thu, 10 Sep 2009 10:51:36 -0800
		/// </summary>
		public static bool TryParseRfc3339(string value, out DateTime result)
		{
			DateTimeFormatInfo dateFormat = CultureInfo.InvariantCulture.DateTimeFormat;
			string [] formats = { 
										dateFormat.SortableDateTimePattern,
										dateFormat.UniversalSortableDateTimePattern,
										"yyyy'-'MM'-'dd'T'HH:mm:ss'Z'",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.f'Z'",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.ff'Z'",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.fff'Z'",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.ffff'Z'",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.fffff'Z'",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.ffffff'Z'",
										"yyyy'-'MM'-'dd'T'HH:mm:szz",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.ffzzz",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.fffzzz",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.ffffzzz",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.fffffzzz",
										"yyyy'-'MM'-'dd'T'HH:mm:ss.ffffffzzz"
																 
										// 2004-03-24T10:17:34.5000000-08:00
								};

				// Inicializa el valor de salida
				result = DateTime.MinValue;
				// Quita el incremento Grenwich sobre la fecha (si existe)
				if (value.Length > 6 && (value.Substring(value.Length - 6, 1) == "-" ||
										  value.Substring(value.Length - 6, 1) == "+"))
					value = value.Substring(0, value.Length - 6);
				// Comprueba el parámetro
				if (string.IsNullOrEmpty(value))
					return false;
				else
					return DateTime.TryParseExact(value, formats, dateFormat, DateTimeStyles.AssumeUniversal, out result);
		}

		/// <summary>
		///		Reemplaza el componente de la hora RFC-822 con su desplazamiento horario equivalente
		/// </summary>
		private static string ReplaceRfc822TimeZoneWithOffset(string value)
		{   
			// Quita los espacios
			value = value.Trim();
			// Obtiene el desplazamiento horario
			if (value.EndsWith("UT", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}GMT", value.TrimEnd("UT".ToCharArray()));
			else if (value.EndsWith("EST", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-05:00", value.TrimEnd("EST".ToCharArray()));
			else if (value.EndsWith("EDT", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-04:00", value.TrimEnd("EDT".ToCharArray()));
			else if (value.EndsWith("CST", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-06:00", value.TrimEnd("CST".ToCharArray()));
			else if (value.EndsWith("CDT", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-05:00", value.TrimEnd("CDT".ToCharArray()));
			else if (value.EndsWith("MST", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-07:00", value.TrimEnd("MST".ToCharArray()));
			else if (value.EndsWith("MDT", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-06:00", value.TrimEnd("MDT".ToCharArray()));
			else if (value.EndsWith("PST", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-08:00", value.TrimEnd("PST".ToCharArray()));
			else if (value.EndsWith("PDT", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-07:00", value.TrimEnd("PDT".ToCharArray()));
			else if (value.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}GMT", value.TrimEnd("Z".ToCharArray()));
			else if (value.EndsWith("A", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}-01:00", value.TrimEnd("A".ToCharArray()));
			else if (value.EndsWith("M", StringComparison.OrdinalIgnoreCase))
				return String.Format(null, "{0}-12:00", value.TrimEnd("M".ToCharArray()));
			else if (value.EndsWith("N", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}+01:00", value.TrimEnd("N".ToCharArray()));
			else if (value.EndsWith("Y", StringComparison.OrdinalIgnoreCase))
				return string.Format(null, "{0}+12:00", value.TrimEnd("Y".ToCharArray()));
			else
				return value;
		}

		/// <summary>
		///		Convierte el valor de la fecha en la cadena correspondiente en el formato RFC-822
		/// </summary>
		public static string ToStringRfc822(DateTime date)
		{
			DateTimeFormatInfo dateFormat = CultureInfo.InvariantCulture.DateTimeFormat;

				return date.ToString(dateFormat.RFC1123Pattern, dateFormat);
		}

		/// <summary>
		///		Convierte una cadena en formato RFC-822 a una fecha
		/// </summary>
		public static DateTime ParseRfc822(string value)
		{
			if (TryParseRfc822(value, out DateTime result))
				return result;
			else
				throw new FormatException($"'{value}' no es un valor correcto en el formato RFC-822.");
		}

		/// <summary>
		///		Convierte la cadena en el formato RFC-822 en una fecha
		/// </summary>
		public static bool TryParseRfc822(string value, out DateTime result)
		{
			DateTimeFormatInfo dateFormat = CultureInfo.InvariantCulture.DateTimeFormat;
			string [] formats = { 
									dateFormat.RFC1123Pattern,
									"ddd',' d MMM yyyy HH:mm:ss zzz",
									"ddd',' dd MMM yyyy HH:mm:ss zzz"
								};

				// Comprueba el parámetro y, si es correcto, convierte el resultado
				if (string.IsNullOrEmpty(value))
				{
					result = DateTime.MinValue;
					return false;
				}
				else
					return DateTime.TryParseExact(ReplaceRfc822TimeZoneWithOffset(value),
												  formats, dateFormat,
												  DateTimeStyles.None, out result);
		}

		/// <summary>
		///		Interpreta una fecha utilizando los diferentes formatos hasta que encuentra uno correcto
		/// </summary>
		public static bool TryParseRfc(string value, out DateTime result)
		{
			return TryParseRfc3339(value, out result) || TryParseRfc822(value, out result);
		}

		/// <summary>
		///		Interpreta una fecha utilizando los diferentes formatos hasta que encuentra uno correcto
		/// </summary>
		public static DateTime ParseRfc(string value, DateTime defaultValue)
		{
			if (!string.IsNullOrEmpty(value) &&
					(TryParseRfc3339(value, out DateTime result) || TryParseRfc822(value, out result)))
				return result;
			else
				return defaultValue;
		}
	}
}
