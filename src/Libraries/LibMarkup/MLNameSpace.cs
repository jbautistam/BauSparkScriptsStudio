using System;

namespace Bau.Libraries.LibMarkupLanguage
{
	/// <summary>
	///		Espacio de nombres
	/// </summary>
	public class MLNameSpace
	{
		public MLNameSpace() : this(null, null) { }

		public MLNameSpace(string prefix, string nameSpace)
		{
			Prefix = prefix;
			NameSpace = nameSpace;
		}

		/// <summary>
		///		Espacio de nombres
		/// </summary>
		public string NameSpace { get; set; }

		/// <summary>
		///		Prefijo del espacio de nombres
		/// </summary>
		public string Prefix { get; set; }
	}
}
