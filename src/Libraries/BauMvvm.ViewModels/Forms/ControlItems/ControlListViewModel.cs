using System;
using System.Collections.ObjectModel;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems
{
	/// <summary>
	///		ViewModel para control con una lista de elementos
	/// </summary>
	public class ControlListViewModel : BaseObservableObject
	{
		// Variables privadas
		private ControlItemViewModel _selectedItem;

		/// <summary>
		///		Añade un elemento
		/// </summary>
		public void Add(ControlItemViewModel item, bool selected)
		{
			// Añade el elemento
			Items.Add(item);
			// Indica si está seleccionado
			if (selected)
			{
				item.IsSelected = true;
				SelectedItem = item;
			}
		}

		/// <summary>
		///		Elementos
		/// </summary>
		public ObservableCollection<ControlItemViewModel> Items { get; } = new ObservableCollection<ControlItemViewModel>();

		/// <summary>
		///		Elemento seleccionado
		/// </summary>
		public ControlItemViewModel SelectedItem
		{
			get { return _selectedItem; }
			set { CheckObject(ref _selectedItem, value); }
		}
	}
}
