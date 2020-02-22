using System;

namespace Bau.Libraries.LibMarkupLanguage
{
	/// <summary>
	///		Archivo para documentos en lenguage XML
	/// </summary>
	public class MLFile
	{ 
		// Enumerados publicos
		public enum EncodingMode
		{
			UTF8
		}

		public MLFile()
		{
			Encoding = EncodingMode.UTF8;
			Version = "1.0";
		}

		/// <summary>
		///		Codificación del archivo
		/// </summary>
		public EncodingMode Encoding { get; set; }

		/// <summary>
		///		Versión del archivo
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		///		Nodos
		/// </summary>
		public MLNodesCollection Nodes { get; } = new MLNodesCollection();
	}
}
