using System;
using System.Collections.ObjectModel;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems
{
	/// <summary>
	///		ViewModel para un control de un elemento jerárquico
	/// </summary>
	public class ControlHierarchicalViewModel : ControlItemViewModel, IHierarchicalViewModel
	{
		// Variables privadas
		private bool _isExpanded;
		private ObservableCollection<IHierarchicalViewModel> _children;

		protected ControlHierarchicalViewModel(IHierarchicalViewModel parent, string text, object tag = null, 
											   bool lazyLoad = true, bool isBold = false, Media.MvvmColor foreground = null) 
								: base(text, tag, isBold, foreground)
		{
			// Asigna las propiedades
			Parent = parent;
			LazyLoad = lazyLoad;
			Children = new ObservableCollection<IHierarchicalViewModel>();
			// Si se va a tratar con una carga posterior, se añade un nodo vacío para que se muestre el signo + junto al nodo
			if (lazyLoad)
				Children.Add(new ControlHierarchicalViewModel(null, "-----", null, false));
		}

		/// <summary>
		///		Carga los elementos hijo
		/// </summary>
		public void LoadChildren()
		{
			if (IsExpanded && !IsChildrenLoaded && LazyLoad)
			{ 
				// Limpia los datos y recarga
				Children.Clear();
				LoadChildrenData();
				// Indica que se han cargado los hijos
				IsChildrenLoaded = true;
			}
		}

		/// <summary>
		///		Carga los elementos hijo
		/// </summary>
		public virtual void LoadChildrenData()
		{
		}

		/// <summary>
		///		Elemento padre
		/// </summary>
		public IHierarchicalViewModel Parent { get; }

		/// <summary>
		///		Indica si el nodo está expandido o no
		/// </summary>
		public bool IsExpanded 
		{
			get { return _isExpanded; }
			set 
			{
				if (CheckProperty(ref _isExpanded, value))
				{ 
					// Expande el elemento padre
					if (Parent != null && !Parent.IsExpanded)
						Parent.IsExpanded = true;
					// Carga los elementos hijo si no están cargados
					LoadChildren();
				}
			}
		}

		/// <summary>
		///		Indica si se han cargado los hijos
		/// </summary>
		protected bool IsChildrenLoaded { get; private set; }

		/// <summary>
		///		Indica si se debe cargar cuando se abra el nodo
		/// </summary>
		protected bool LazyLoad { get; }

		/// <summary>
		///		Elementos hijo
		/// </summary>
		public ObservableCollection<IHierarchicalViewModel> Children 
		{ 
			get { return _children; }
			set { CheckObject(ref _children, value); }
		}
	}
}
