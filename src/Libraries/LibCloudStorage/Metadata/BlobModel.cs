using System;

namespace Bau.Libraries.LibBlobStorage.Metadata
{
	/// <summary>
	///		Clase con los datos de un blob
	/// </summary>
	public class BlobModel
	{
		/// <summary>
		///		Contenedor
		/// </summary>
		public string Container { get; set; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FullFileName { get; set; }

		/// <summary>
		///		Url
		/// </summary>
		public Uri Url { get; set; }
	}
}
