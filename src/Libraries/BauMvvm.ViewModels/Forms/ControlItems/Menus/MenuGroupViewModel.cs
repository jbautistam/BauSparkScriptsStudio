using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Menus
{
	/// <summary>
	///		Grupo de elementos de menú
	/// </summary>
	public class MenuGroupViewModel
	{   
		// Enumerados publicos
		/// <summary>
		///		Tipo de destino del menú
		/// </summary>
		public enum TargetMenuType
		{
			/// <summary>Barrra principal de menús</summary>
			MainMenu,
			/// <summary>Otras opciones</summary>
			Other
		}
		/// <summary>
		///		Tipo de destino del menú dentro del menú principal
		/// </summary>
		public enum TargetMainMenuItemType
		{
			/// <summary>Opción de menú Abrir</summary>
			FileOpenItems,
			/// <summary>Opción de menú Nuevo</summary>
			FileNewItems,
			/// <summary>Opción de menú en Archivos</summary>
			FileAdditionalItems,
			/// <summary>Opción de menú Herramientas</summary>
			Tools,
			/// <summary>Otras opciones de menú</summary>
			Other
		}

		public MenuGroupViewModel(string name, TargetMenuType targetMenu, TargetMainMenuItemType targetMenuItem)
		{
			Name = name;
			TargetMenu = targetMenu;
			TargetMenuItem = targetMenuItem;
			MenuItems = new MenuItemViewModelCollection();
		}

		/// <summary>
		///		Nombre del grupo
		/// </summary>
		public string Name { get; }

		/// <summary>
		///		Destino del grupo del menú
		/// </summary>
		public TargetMenuType TargetMenu { get; }

		/// <summary>
		///		Opción del menú
		/// </summary>
		public TargetMainMenuItemType TargetMenuItem { get; }

		/// <summary>
		///		Elementos del menú
		/// </summary>
		public MenuItemViewModelCollection MenuItems { get; }
	}
}
