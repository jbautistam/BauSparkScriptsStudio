﻿using System;

namespace Bau.Libraries.BauSparkScripts.Models.Connections
{
	/// <summary>
	///		Datos del campo
	/// </summary>
	public class ConnectionTableFieldModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Tipo de campo
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		///		Longitud del campo
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		///		Nombre completo
		/// </summary>
		public string FullName
		{
			get
			{
				string length = Length > 0 ? $"({Length:#,##0})" : string.Empty;

					return $"{Name} [{Type}{length}]";
			}
		}

		/// <summary>
		///		Indica si el campo es obligatorio
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		///		Indica si el campo es clave
		/// </summary>
		public bool IsKey { get; set; }

		/// <summary>
		///		India si el campo es identidad
		/// </summary>
		public bool IsIdentity { get; set; }
	}
}
