using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees
{
	/// <summary>
	///		ViewModel para la presentación de un árbol
	/// </summary>
	public abstract class TreeViewModel<TypeData> : BaseObservableObject where TypeData : ControlHierarchicalViewModel
	{
		// Variables privadas
		private ObservableCollection<TypeData> _children = new ObservableCollection<TypeData>();
		private TypeData _selectedNode;

		/// <summary>
		///		Carga los nodos
		/// </summary>
		public void LoadNodes()
		{
			// Limpia los nodos
			Children.Clear();
			// Carga los nodos hijo
			LoadNodesData();
		}

		/// <summary>
		///		Carga los nodos
		/// </summary>
		protected abstract void LoadNodesData();

		/// <summary>
		///		Convierte la colección de <see cref="Children"/> a <see cref="ObservableCollection{T}"/>
		/// </summary>
		protected List<TypeData> ConvertNodes()
		{
			return ConvertNodes(Children);
		}

		/// <summary>
		///		Convierte la colección de <see cref="Children"/> a <see cref="ObservableCollection{T}"/>
		/// </summary>
		protected List<TypeData> ConvertNodes<TypeItem>(ObservableCollection<TypeItem> nodes) 
					where TypeItem : IHierarchicalViewModel
		{
			var converted = new List<TypeData>();

				// Convierte los elementos
				foreach (TypeItem node in nodes)
					converted.Add(node as TypeData);
				// Devuelve la lista de elementos convertida
				return converted;
		}

		/// <summary>
		///		Nodos
		/// </summary>
		public ObservableCollection<TypeData> Children
		{
			get { return _children; }
			set { CheckObject(ref _children, value); }
		}

		/// <summary>
		///		Nodo seleccionado
		/// </summary>
		public TypeData SelectedNode
		{
			get { return _selectedNode; }
			set { CheckObject(ref _selectedNode, value); }
		}
	}
}
