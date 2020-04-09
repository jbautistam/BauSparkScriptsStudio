﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Cloud;

namespace Bau.SparkScripts.Studio.Views
{
	/// <summary>
	///		Arbol del explorador de archivos
	/// </summary>
	public partial class TreeStoragesExplorer : UserControl
	{
		public TreeStoragesExplorer(TreeStorageViewModel treeViewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = treeViewModel;
		}

		/// <summary>
		///		ViewModel del árbol de storage
		/// </summary>
		public TreeStorageViewModel ViewModel { get; }

		private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{ 
			if (trvExplorer.DataContext is TreeStorageViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
				ViewModel.SelectedNode = node;
		}

		private void trvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{ 
			if (trvExplorer.DataContext is TreeStorageViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
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