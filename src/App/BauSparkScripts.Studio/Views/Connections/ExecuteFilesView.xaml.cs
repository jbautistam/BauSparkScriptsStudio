﻿using System;
using System.Windows.Controls;

using Bau.Libraries.BauMvvm.Views.Forms.Trees;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections;

namespace Bau.SparkScripts.Studio.Views.Connections
{
	/// <summary>
	///		Ventana para ejecutar una serie de archivos
	/// </summary>
	public partial class ExecuteFilesView : UserControl
	{
		public ExecuteFilesView(ExecuteFilesViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ExecuteFilesViewModel ViewModel { get; }
	}
}
