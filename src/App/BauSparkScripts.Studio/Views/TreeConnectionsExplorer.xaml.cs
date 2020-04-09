﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.Views.Forms.Trees;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Connections;

namespace Bau.SparkScripts.Studio.Views
{
	/// <summary>
	///		Arbol del explorador de conexiones
	/// </summary>
	public partial class TreeConnectionsExplorer : UserControl
	{
		// Variables privadas
		private Point _startDrag;
		private DragDropTreeExplorerController _dragDropController = new DragDropTreeExplorerController();

		public TreeConnectionsExplorer(TreeConnectionsViewModel treeViewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = treeViewModel;
		}

		/// <summary>
		///		ViewModel del árbol de soluciones
		/// </summary>
		public TreeConnectionsViewModel ViewModel { get; }

		private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{ 
			if (trvExplorer.DataContext is TreeConnectionsViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
			{
				ViewModel.SelectedNode = node;
				//ViewModel.OpenDetailsCommand.Execute(null);
			}
		}

		private void trvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{ 
			if (trvExplorer.DataContext is TreeConnectionsViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
			{
				ViewModel.SelectedNode = node;
				ViewModel.OpenPropertiesCommand.Execute(null);
			}
		}

		private void trvExplorer_MouseDown(object sender, MouseButtonEventArgs e)
		{ 
			if (e.ChangedButton == MouseButton.Left)
				ViewModel.SelectedNode = null;
		}

		private void trvExplorer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_startDrag = e.GetPosition(null);
		}

		private void trvExplorer_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Point pntMouse = e.GetPosition(null);
				Vector vctDifference = _startDrag - pntMouse;

					if (pntMouse.X < trvExplorer.ActualWidth - 50 &&
							(Math.Abs(vctDifference.X) > SystemParameters.MinimumHorizontalDragDistance ||
							 Math.Abs(vctDifference.Y) > SystemParameters.MinimumVerticalDragDistance))
						_dragDropController.InitDragOperation(trvExplorer, trvExplorer.SelectedItem as IHierarchicalViewModel);
			}
		}
	}
}
