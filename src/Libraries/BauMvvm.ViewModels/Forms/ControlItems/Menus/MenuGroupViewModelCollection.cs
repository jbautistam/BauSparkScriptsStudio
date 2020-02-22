using System;
using System.Collections.ObjectModel;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Menus
{
	/// <summary>
	///		Colección de <see cref="MenuGroupViewModel"/>
	/// </summary>
	public class MenuGroupViewModelCollection : ObservableCollection<MenuGroupViewModel>
	{
		/// <summary>
		///		Añade un elemento a la colección
		/// </summary>
		public MenuGroupViewModel Add(string name, MenuGroupViewModel.TargetMenuType targetMenu,
									  MenuGroupViewModel.TargetMainMenuItemType targetMenuItem)
		{
			MenuGroupViewModel group = new MenuGroupViewModel(name, targetMenu, targetMenuItem);

				// Añade el grupo
				Add(group);
				// Devuelve el grupo añadido
				return group;
		}

		/// <summary>
		///		Selecciona una serie de menús
		/// </summary>
		public MenuGroupViewModelCollection Select(MenuGroupViewModel.TargetMenuType targetMenuType,
												   MenuGroupViewModel.TargetMainMenuItemType targetMainMenuItemType)
		{
			MenuGroupViewModelCollection groups = new MenuGroupViewModelCollection();

				// Añade los grupos seleccionados
				foreach (MenuGroupViewModel group in this)
					if (group.TargetMenu == targetMenuType && group.TargetMenuItem == targetMainMenuItemType)
						groups.Add(group);
				// Devuelve la colección de grupos
				return groups;
		}
	}
}
