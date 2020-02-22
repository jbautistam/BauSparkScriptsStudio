using System;
using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;

namespace Bau.Libraries.BauMvvm.Views.Forms.Trees
{
	/// <summary>
	///		Controlador de dragDrop de un árbol
	/// </summary>
	public class DragDropTreeExplorerController
	{
		public DragDropTreeExplorerController(string keyDataObject = "BaseNodeViewModel")
		{
			KeyDataObject = keyDataObject;
		}

		/// <summary>
		///		Inicia la operación de Drag & Drop
		/// </summary>
		public void InitDragOperation(TreeView tree, IHierarchicalViewModel node)
		{
			if (node != null)
				DragDrop.DoDragDrop(tree, new DataObject(KeyDataObject, node), DragDropEffects.Move);
		}

		/// <summary>
		///		Obtiene el nodo de archivo que se está arrastrando
		/// </summary>
		public IHierarchicalViewModel GetDragDropFileNode(IDataObject dataObject)
		{
			IHierarchicalViewModel node = null;

				// Obtiene los datos que se están arrastrando
				if (dataObject.GetDataPresent(KeyDataObject))
					node = dataObject.GetData(KeyDataObject) as IHierarchicalViewModel;
				// Devuelve los datos del nodo que se está arrastrando
				return node;
		}

		/// <summary>
		///		Trata el evento de entrada en un control
		/// </summary>
		public void TreatDragEnter(DragEventArgs e)
		{
			if (GetDragDropFileNode(e.Data) != null)
				e.Effects = DragDropEffects.Copy;
			else
				e.Effects = DragDropEffects.None;
		}

		/// <summary>
		///		Clave del objeto utilizado para almacenar los datos a arrastrar y copiar
		/// </summary>
		public string KeyDataObject { get; }
	}
}
