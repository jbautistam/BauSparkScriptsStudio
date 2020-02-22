using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Menus
{
	/// <summary>
	///		Clase con los datos de los menús y barras de herramientas
	/// </summary>
	public class MenuComposition
	{
		public MenuComposition()
		{
			Menus = new MenuGroupViewModelCollection();
			ToolBars = new MenuGroupViewModelCollection();
		}

		/// <summary>
		///		Menús
		/// </summary>
		public MenuGroupViewModelCollection Menus { get; }

		/// <summary>
		///		Barras de herramientas
		/// </summary>
		public MenuGroupViewModelCollection ToolBars { get; }
	}
}
