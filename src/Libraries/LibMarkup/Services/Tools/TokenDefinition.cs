using System;

namespace Bau.Libraries.LibMarkupLanguage.Services.Tools
{
	/// <summary>
	///		Definición de un token
	/// </summary>
	internal class TokenDefinition : IComparable<TokenDefinition>
	{
		internal TokenDefinition(int? type, string name) : this(type, name, null, null)
		{
		}

		internal TokenDefinition(int? type, string name, string strStart) : this(type, name, strStart, null)
		{
		}

		internal TokenDefinition(int? type, string name, string strStart, string strEnd)
		{
			Type = type;
			Name = name;
			Start = strStart;
			End = strEnd;
		}

		/// <summary>
		///		Compara con otra definíción de contenido
		/// </summary>
		public int CompareTo(TokenDefinition definition)
		{
			if (definition == null)
				return -1;
			else if (Start == definition.Start)
				if (End == null)
					return -1;
				else
					return End.CompareTo(definition.End);
			else
				return Start.CompareTo(definition.Start);
		}

		/// <summary>
		///		Tipo de token
		/// </summary>
		internal int? Type { get; set; }

		/// <summary>
		///		Nombre del token
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Indica si es un token sobre datos numéricos
		/// </summary>
		internal bool IsNumeric
		{
			get { return string.IsNullOrEmpty(Start); }
		}

		/// <summary>
		///		Cadena de inicio
		/// </summary>
		internal string Start { get; set; }

		/// <summary>
		///		Cadena de fin
		/// </summary>
		internal string End { get; set; }
	}
}
