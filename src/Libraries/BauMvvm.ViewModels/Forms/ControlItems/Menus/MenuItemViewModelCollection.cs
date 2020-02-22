using System;
using System.Collections.ObjectModel;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Menus
{
	/// <summary>
	///		Colección de <see cref="MenuItemViewModel"/>
	/// </summary>
	public class MenuItemViewModelCollection : ObservableCollection<MenuItemViewModel>
	{
		/// <summary>
		///		Añade un separador
		/// </summary>
		public void AddSeparator()
		{
			Add(new MenuItemViewModel(null, null, null));
		}

		/// <summary>
		///		Añade un elemento a la colección
		/// </summary>
		public void Add(string text, string icon = null, BaseCommand command = null)
		{
			Add(new MenuItemViewModel(text, icon, command));
		}
	}
}
