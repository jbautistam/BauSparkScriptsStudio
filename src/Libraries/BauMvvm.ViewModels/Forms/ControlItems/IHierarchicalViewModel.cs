using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems
{
	/// <summary>
	///		Interface para los elementos jerárquicos
	/// </summary>
	public interface IHierarchicalViewModel
	{
		/// <summary>
		///		ID del elemento
		/// </summary>
		string ID { get; }

		/// <summary>
		///		Método para cargar los nodos hijo
		/// </summary>
		void LoadChildren();

		/// <summary>
		///		Indica si el nodo está abierto o no
		/// </summary>
		bool IsExpanded { get; set; }

		/// <summary>
		///		Indica si el nodo está chequeado
		/// </summary>
		bool IsChecked { get; set; }

		/// <summary>
		///		Elemento padre
		/// </summary>
		IHierarchicalViewModel Parent { get; }

		/// <summary>
		///		Elementos hijo
		/// </summary>
		System.Collections.ObjectModel.ObservableCollection<IHierarchicalViewModel> Children { get; }
	}
}
