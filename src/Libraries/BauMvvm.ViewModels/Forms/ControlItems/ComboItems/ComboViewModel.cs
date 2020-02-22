using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems
{
	/// <summary>
	///		ViewModel para un combo
	/// </summary>
	public class ComboViewModel : BaseObservableObject
	{ 
		// Variables privadas
		private ComboItem _selectedItem;

		public ComboViewModel(BaseObservableObject viewModelParent)
		{
			ViewModelParent = viewModelParent;
		}

		/// <summary>
		///		Añade un elemento
		/// </summary>
		public void AddItem(int? id, string text, object tag = null)
		{
			Items.Add(id, text, tag);
		}

		/// <summary>
		///		Elemento seleccionado
		/// </summary>
		public ComboItem SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				if (CheckObject(ref _selectedItem, value))
					ViewModelParent.IsUpdated = true;
			}
		}

		/// <summary>
		///		Elementos
		/// </summary>
		public ComboItemsCollection Items { get; } = new ComboItemsCollection();

		/// <summary>
		///		ID del elemento seleccionado
		/// </summary>
		public int? SelectedID
		{
			get
			{
				if (SelectedItem == null)
					return null;
				else
					return SelectedItem.ID;
			}
			set
			{
				foreach (ComboItem item in Items)
					if (item.ID == value)
						SelectedItem = item;
			}
		}

		/// <summary>
		///		Objeto asociado al elemento seleccionado
		/// </summary>
		public object SelectedTag
		{
			get
			{
				if (SelectedItem == null)
					return null;
				else
					return SelectedItem.Tag;
			}
			set
			{
				foreach (ComboItem item in Items)
					if ((value == null && item.Tag == null) ||
							(value != null && ReferenceEquals(item.Tag, value)))
						SelectedItem = item;
			}
		}

		/// <summary>
		///		Texto seleccionado
		/// </summary>
		public string SelectedText
		{
			get
			{
				if (SelectedItem == null)
					return null;
				else
					return SelectedItem.Text;
			}
			set
			{
				foreach (ComboItem item in Items)
					if (!string.IsNullOrEmpty(item.Text) && item.Text.Equals(value, StringComparison.CurrentCultureIgnoreCase))
						SelectedItem = item;
			}
		}

		/// <summary>
		///		ViewModel padre
		/// </summary>
		public BaseObservableObject ViewModelParent { get; }
	}
}
