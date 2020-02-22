using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Files;

namespace Bau.SparkScripts.Studio.Views
{
	/// <summary>
	///		Arbol del explorador de archivos
	/// </summary>
	public partial class TreeFilesExplorer : UserControl
	{
		public TreeFilesExplorer(TreeFilesViewModel treeViewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = treeViewModel;
		}

		/// <summary>
		///		ViewModel del árbol de archivos
		/// </summary>
		public TreeFilesViewModel ViewModel { get; }

		private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{ 
			if (trvExplorer.DataContext is TreeFilesViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
			{
				ViewModel.SelectedNode = node;
				//ViewModel.OpenDetailsCommand.Execute(null);
			}
		}

		private void trvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{ 
			if (trvExplorer.DataContext is TreeFilesViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
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
