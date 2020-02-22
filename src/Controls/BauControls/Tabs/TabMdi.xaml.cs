using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Bau.Controls.Tabs
{
	/// <summary>
	///		Control de ficha para simular ventanas MDI
	/// </summary>
	public partial class TabMdi : UserControl
	{
		// Eventos públicos
		public event EventHandler<CloseTabEvntArgs> Close;
		public event EventHandler SelectedTabChanged;
		// Variables privadas
		private Dictionary<string, TabItem> tabs = new Dictionary<string, TabItem>();

		public TabMdi()
		{
			InitializeComponent();
		}

		/// <summary>
		///		Añade una ficha al control
		/// </summary>
		public TabItem AddTab(string id, string header, UserControl control, object tag = null)
		{
			TabItem tab = new TabItem();
			ClosableTabHeader tabHeader = new ClosableTabHeader();

				// Asigna las propiedades a la cabecera
				tabHeader.Header = header;
				tabHeader.Close += (sender, args) => CloseWindow(tab, tag);
				// Asigna las propiedades a la ficha
				tab.Header = tabHeader;
				tab.Content = control;
				tab.Tag = tag;
				// Añade la ficha al control y la colección
				if (!string.IsNullOrWhiteSpace(id))
					tabs.Add(id.ToUpper(), tab);
				tabMdi.Items.Add(tab);
				// y lo selecciona
				tabMdi.SelectedItem = tab;
				tab.GotFocus += (sender, args) => UpdateSelectedItem();
				UpdateSelectedItem();
				// Devuelve el elemento añadido
				return tab;
		}

		/// <summary>
		///		Obtiene la ficha asociada a un id
		/// </summary>
		public TabItem GetTabItem(string id)
		{
			if (!string.IsNullOrWhiteSpace(id) && tabs.ContainsKey(id.ToUpper()))
				return tabs[id.ToUpper()];
			else
				return null;
		}

		/// <summary>
		///		Selecciona una ficha
		/// </summary>
		public void SelectTabItem(string id)
		{
			if (!string.IsNullOrWhiteSpace(id) && tabs.ContainsKey(id.ToUpper()))
			{
				SelectedItem = tabs[id.ToUpper()];
				if (SelectedItem != null)
					SelectedItem.IsSelected = true;
				UpdateSelectedItem();
			}
		}

		/// <summary>
		///		Modifica la ficha seleccionada
		/// </summary>
		private void UpdateSelectedItem()
		{
			SelectedItem = tabMdi.SelectedItem as TabItem;
			SelectedTabChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///		Cierra una ficha
		/// </summary>
		private void CloseWindow(TabItem tab, object tag)
		{
			CloseTabEvntArgs evntArgs = new CloseTabEvntArgs(tab, tag);

				// Lanza el evento
				Close?.Invoke(this, evntArgs);
				// Borra la ficha si es necesario
				if (!evntArgs.Cancel)
				{
					// Borra la ficha y el diccionario
					tabMdi.Items.Remove(tab);
					DeleteTab(tab);
					// Cambia el elemento seleccionado
					UpdateSelectedItem();
				}
		}

		/// <summary>
		///		Borra la ficha en el diccionario
		/// </summary>
		private void DeleteTab(TabItem tab)
		{
			string id = string.Empty;

				// Busca la ficha en el diccionario
				foreach (KeyValuePair<string, TabItem> item in tabs)
					if (item.Value.Equals(tab))
						id = item.Key;
				// Borra la ficha del diccionario
				if (!string.IsNullOrWhiteSpace(id))
					tabs.Remove(id);
		}

		/// <summary>
		///		Elemento seleccionado
		/// </summary>
		public TabItem SelectedItem { get; set; }
	}
}
