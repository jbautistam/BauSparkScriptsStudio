﻿using System;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details
{
	/// <summary>
	///		Interface para los viewModel de detalles
	/// </summary>
	public interface IDetailViewModel
	{
		/// <summary>
		///		Graba los datos
		/// </summary>
		void SaveDetails(bool newName);

		/// <summary>
		///		Título de la ficha
		/// </summary>
		string Header { get; }

		/// <summary>
		///		Id de la ficha
		/// </summary>
		string TabId { get; }

		/// <summary>
		///		Indica si se ha modificado el ViewModel
		/// </summary>
		bool IsUpdated { get; set; }
	}
}
