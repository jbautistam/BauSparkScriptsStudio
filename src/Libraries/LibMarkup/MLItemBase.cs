using System;

namespace Bau.Libraries.LibMarkupLanguage
{
	/// <summary>
	///		Base para nodos y atributos
	/// </summary>
	public class MLItemBase
	{ 
		// Constantes privadas
		internal const string Yes = "yes";
		internal const string No = "no";
		internal const string True = "true";
		internal const string False = "false";

		public MLItemBase()
		{
		}

		public MLItemBase(string name) : this(null, name, null) { }

		public MLItemBase(string name, bool value) : this(null, name, value) { }

		public MLItemBase(string prefix, string name, bool value) : this(prefix, name, Format(value)) { }

		public MLItemBase(string name, double value) : this(null, name, value) { }

		public MLItemBase(string prefix, string name, double dblValue) : this(prefix, name, Format(dblValue)) { }

		public MLItemBase(string name, DateTime dtmValue) : this(null, name, dtmValue) { }

		public MLItemBase(string prefix, string name, DateTime dtmValue) : this(prefix, name, Format(dtmValue)) { }

		public MLItemBase(string name, string value) : this(null, name, value) { }

		public MLItemBase(string prefix, string name, string value)
		{
			Prefix = prefix;
			Name = name;
			if (!string.IsNullOrWhiteSpace(Name) && Name.IndexOf(':') >= 0)
			{
				string [] arrname = name.Split(':');

				if (arrname.Length > 1)
				{
					Prefix = arrname[0];
					Name = arrname[1];
				}
			}
			Value = value;
		}

		/// <summary>
		///		Formatea un valor lógico
		/// </summary>
		internal static string Format(bool value)
		{
			if (value)
				return Yes;
			else
				return No;
		}

		/// <summary>
		///		Formatea un valor numérico
		/// </summary>
		internal static string Format(double? value)
		{
			if (value == null)
				return "";
			else
				return value.ToString().Replace(',', '.');
		}

		/// <summary>
		///		Formatea un valor de tipo fecha
		/// </summary>
		internal static string Format(DateTime? value)
		{
			if (value == null)
				return "";
			else
				return string.Format("{0:yyyy-MM-dd HH:mm:ss}", value);
		}

		/// <summary>
		///		Prefijo del espacio de nombres
		/// </summary>
		public string Prefix { get; set; }

		/// <summary>
		///		Nombre del elemento
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Valor del elemento
		/// </summary>
		public string Value { get; set; }
	}
}
