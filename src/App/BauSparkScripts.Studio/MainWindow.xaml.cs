using System;
using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.BauSparkScripts.ViewModels;
using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details;
using Bau.SparkScripts.Studio.Views.Files;

namespace Bau.SparkScripts.Studio
{
	/// <summary>
	///		Ventana principal de la aplicación
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{	
			// Inicializa el controlador
			MainController = new Controllers.AppController("Bau.SparkScripts.Studio", this, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
			MainController.ConfigurationController.Load();
			// Inicializa el contexto y los controles
			DataContext = ViewModel = new MainViewModel(MainController.SparkSolutionController);
			// Carga la última solución
			ViewModel.SolutionViewModel.Load();
			ViewModel.LastPathSelected = MainController.ConfigurationController.LastPathSelected;
			// Añade los paneles
			dckManager.AddPane("TreeConnectionsExplorer", "Conexiones", new Views.TreeConnectionsExplorer(ViewModel.SolutionViewModel.TreeConnectionsViewModel), 
							   null, Controls.DockLayout.DockPosition.Left);
			dckManager.AddPane("TreeFilesExplorer", "Archivos", new Views.TreeFilesExplorer(ViewModel.SolutionViewModel.TreeFoldersViewModel), 
							   null, Controls.DockLayout.DockPosition.Left);
			dckManager.AddPane("LogView", "Log", new Views.Log.LogView(ViewModel.LogViewModel), null, Controls.DockLayout.DockPosition.Bottom);
			// Abre los paneles predefinidos
			dckManager.OpenGroup(Controls.DockLayout.DockPosition.Left);
			// Asigna los manejadores de eventos
			MainController.SparkSolutionController.OpenWindowRequired += (sender, args) => OpenWindow(args);
			// Asigna los manejadores de eventos del docker de documentos
			dckManager.Closing += (sender, args) => CloseWindow(args);
			dckManager.ActiveDocumentChanged += (sender, args) => UpdateSelectedTab();
			// Cambia el tema
			SetTheme((Controls.DockLayout.DockTheme) MainController.ConfigurationController.LastThemeSelected);
		}

		/// <summary>
		///		Abre la ventana de detalles
		/// </summary>
		private void OpenWindow(IDetailViewModel detailsViewModel)
		{
			switch (detailsViewModel)
			{
				case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files.FileViewModel viewModel:
						AddTab(new FileDetailsView(viewModel), viewModel);
					break;
				case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files.ParquetFileViewModel viewModel:
						AddTab(new ParquetFileView(viewModel), viewModel);
					break;
				case Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections.ExecuteQueryViewModel viewModel:
						AddTab(new Views.Connections.ExecuteQueryView(viewModel), viewModel);
					break;
			}
		}

		/// <summary>
		///		Añade una ficha al control
		/// </summary>
		private void AddTab(UserControl control, IDetailViewModel detailsViewModel)
		{
			dckManager.AddDocument(detailsViewModel.TabId, detailsViewModel.Header, control, detailsViewModel);
		}

		/// <summary>
		///		Cierra una ficha
		/// </summary>
		private void CloseWindow(Controls.ClosingEventArgs args)
		{
			if (args.Document != null && args.Document.Tag != null && args.Document.Tag is IDetailViewModel detailViewModel)
				args.Cancel = detailViewModel.IsUpdated && !MainController.SparkSolutionController.HostController.SystemController.ShowQuestion("¿Realmente desea cerrar esta ventana?");
		}

		/// <summary>
		///		Modifica la ventana de detalles seleccionada
		/// </summary>
		private void UpdateSelectedTab()
		{
			IDetailViewModel details = null;
			
				// Obtiene los detalles de la ficha seleccionada
				if (dckManager.ActiveDocument != null)
					details = dckManager.ActiveDocument.Tag as IDetailViewModel;
				// Cambia la ficha seleccionada en el ViewModel
				ViewModel.SelectedDetailsViewModel = details;
		}

		/// <summary>
		///		Cierra todas las ventanas
		/// </summary>
		private void CloseAllWindows()
		{
			dckManager.CloseAllDocuments();
		}

		/// <summary>
		///		Cambia el tema del layout
		/// </summary>
		private void SetTheme(Controls.DockLayout.DockTheme newTheme)
		{
			// Cambia el tema
			dckManager.SetTheme(newTheme);
			// Cambia los menús
			mnuThemeAero.IsChecked = newTheme == Controls.DockLayout.DockTheme.Aero;
			mnuThemeMetro.IsChecked = newTheme == Controls.DockLayout.DockTheme.Metro;
			mnuThemeVs2010.IsChecked = newTheme == Controls.DockLayout.DockTheme.VS2010Theme;
			// Cambia la configuración
			MainController.ConfigurationController.LastThemeSelected = (int) newTheme;
		}

		/// <summary>
		///		Sale de la aplicación
		/// </summary>
		private void ExitApp()
		{
			// Graba la configuración
			if (!string.IsNullOrWhiteSpace(ViewModel.LastPathSelected))
				MainController.ConfigurationController.LastPathSelected = ViewModel.LastPathSelected;
			MainController.ConfigurationController.Save();
			// Cierra la aplicación
			Close();
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public MainViewModel ViewModel { get; private set; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.AppController MainController { get; private set; }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitForm();
		}

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ExitApp();
		}

		private void CloseAllWindows_Click(object sender, RoutedEventArgs e)
		{
			CloseAllWindows();
		}

		private void ThemeAero_Click(object sender, RoutedEventArgs e)
		{
			SetTheme(Controls.DockLayout.DockTheme.Aero);
		}

		private void ThemeMetro_Click(object sender, RoutedEventArgs e)
		{
			SetTheme(Controls.DockLayout.DockTheme.Metro);
		}

		private void ThemeVS2010_Click(object sender, RoutedEventArgs e)
		{
			SetTheme(Controls.DockLayout.DockTheme.VS2010Theme);
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			ExitApp();
		}
	}
}
