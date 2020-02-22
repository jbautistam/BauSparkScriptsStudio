using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.Interfaces
{
	/// <summary>
	///		Interface para el ViewModel de los formularios
	/// </summary>
	public interface IFormViewModel
	{
		/// <summary>
		///		Menús del ViewModel
		/// </summary>
		ControlItems.Menus.MenuComposition MenuCompositionData { get; }

		/// <summary>
		///		Comando para grabar un elemento
		/// </summary>
		BaseCommand SaveCommand { get; }

		/// <summary>
		///		Comando para borrar un elemento
		/// </summary>
		BaseCommand DeleteCommand { get; }

		/// <summary>
		///		Comando para actualizar un elemento
		/// </summary>
		BaseCommand RefreshCommand { get; }
	}
}
