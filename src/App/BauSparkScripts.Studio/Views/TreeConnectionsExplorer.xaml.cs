using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Connections;

namespace Bau.SparkScripts.Studio.Views
{
	/// <summary>
	///		Arbol del explorador de conexiones
	/// </summary>
	public partial class TreeConnectionsExplorer : UserControl
	{
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
	}
}
