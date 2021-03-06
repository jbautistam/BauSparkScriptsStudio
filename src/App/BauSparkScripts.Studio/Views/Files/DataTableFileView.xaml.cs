﻿using System;
using System.Windows.Controls;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files;

namespace Bau.SparkScripts.Studio.Views.Files
{
	/// <summary>
	///		Ventana para mostrar el contenido de un archivo parquet
	/// </summary>
	public partial class DataTableFileView : UserControl
	{
		public DataTableFileView(BaseFileViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			ViewModel.LoadFile();
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public BaseFileViewModel ViewModel { get; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}
	}
}
