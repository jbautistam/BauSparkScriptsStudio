﻿using System;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauSparkScripts.Application;
using Bau.Libraries.BauSparkScripts.Models.Connections;
using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.BauSparkScripts.ViewModels
{
	/// <summary>
	///		ViewModel principal
	/// </summary>
	public class MainViewModel : BaseObservableObject
	{
		// Constantes privadas
		internal const string MaskFiles = "Archivos de solución (*.dbsln)|*.dbsln|Todos los archivos (*.*)|*.*";
		// Variables privadas
		private string _text;
		private Solutions.Details.IDetailViewModel _selectedDetailsViewModel;
		private Tools.LogViewModel _logViewModel;

		public MainViewModel(Controllers.ISparkSolutionController mainController)
		{
			// Título de la aplicación
			Text = mainController.AppName;
			// Asigna las propiedades
			Instance = this;
			MainController = mainController;
			Manager = new SolutionManager(mainController.Logger, mainController.Configuration.ConfigurationPath);
			// Inicializa los objetos
			SolutionViewModel = new Solutions.SolutionViewModel(this);
			// Inicializa el log
			LogViewModel = new Tools.LogViewModel(this);
			// Asigna los comandos
			AddConnectionCommand = new BaseCommand(_ => SolutionViewModel.TreeConnectionsViewModel.OpenConnection(null));
			AddFolderToExplorerCommand = new BaseCommand(_ => SolutionViewModel.TreeFoldersViewModel.AddFolderToExplorer());
			SaveCommand = new BaseCommand(parameter => Save(false), parameter => CanSave())
									.AddListener(this, nameof(SelectedDetailsViewModel));
			SaveAsCommand = new BaseCommand(parameter => Save(true), parameter => CanSave())
									.AddListener(this, nameof(SelectedDetailsViewModel));
			RefreshCommand = new BaseCommand(parameter => Refresh());
		}

		/// <summary>
		///		Graba la solución y actualiza el árbol
		/// </summary>
		internal void SaveSolution(bool refresh = true)
		{
			// Graba la solución
			Manager.SaveSolution(SolutionViewModel.Solution);
			// Actualiza los árboles (si es necesario)
			if (refresh)
				Refresh();
		}

		/// <summary>
		///		Actualiza el árbol
		/// </summary>
		internal void Refresh()
		{
			SolutionViewModel.Load();
		}

		/// <summary>
		///		Comprueba si puede guardar el contenido de la ventana
		/// </summary>
		private bool CanSave()
		{
			return SelectedDetailsViewModel != null;
		}

		/// <summary>
		///		Graba el viewModel activo
		/// </summary>
		private void Save(bool newName)
		{
			if (SelectedDetailsViewModel != null)
				SelectedDetailsViewModel.SaveDetails(newName);
		}

		/// <summary>
		///		Ejecuta el script
		/// </summary>
		internal async Task ExecuteScriptAsync(ConnectionModel connection, NormalizedDictionary<object> parameters)
		{
			if (SelectedDetailsViewModel != null && SelectedDetailsViewModel is Solutions.Details.Files.FileViewModel fileViewModel)
				await fileViewModel.ExecuteScriptAsync(connection, parameters);
		}

		/// <summary>
		///		Solicita al usuario un nombre de archivos. Guarda el directorio seleccionado para que la próxima vez que se consulte
		///	por un archivo, se vaya directamente a ese directorio
		/// </summary>
		internal string OpenDialogSave(string suggestedFileName, string mask, string defaultExtension)
		{
			string path = GetPath(suggestedFileName);
			string fileName = MainController.HostController.DialogsController.OpenDialogSave(path, mask, suggestedFileName, defaultExtension);

				// Si se ha escogido un nombre de archivo se guarda el último directorio seleccionado
				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fileName)))
					LastPathSelected = System.IO.Path.GetDirectoryName(fileName);
				// Devuelve el nombre de archivo
				return fileName;
		}

		/// <summary>
		///		Obtiene el directorio inicial de grabación de un archivo
		/// </summary>
		private string GetPath(string suggestedFileName)
		{
			string path = string.Empty;

				// Obtiene el directorio
				if (!string.IsNullOrWhiteSpace(suggestedFileName))
				{
					path = System.IO.Path.GetDirectoryName(suggestedFileName);
					if (string.IsNullOrWhiteSpace(path) || path.Equals(suggestedFileName, StringComparison.CurrentCultureIgnoreCase))
						path = LastPathSelected;
				}
				// Si no se ha definido ningún directorio, se coge el de la solución
				if (string.IsNullOrWhiteSpace(path))
					path = SolutionViewModel.Solution.Path;
				// Devuelve el directorio
				return path;
		}

		/// <summary>
		///		Instancia principal 
		/// </summary>
		public MainViewModel Instance { get; private set; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.ISparkSolutionController MainController { get; }

		/// <summary>
		///		Manager de solución
		/// </summary>
		internal SolutionManager Manager { get; }

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public Solutions.SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		ViewModel de detalles seleccionado en la ventana principal
		/// </summary>
		public Solutions.Details.IDetailViewModel SelectedDetailsViewModel
		{
			get { return _selectedDetailsViewModel; }
			set { CheckObject(ref _selectedDetailsViewModel, value); }
		}

		/// <summary>
		///		Título de la ventana
		/// </summary>
		public string Text 
		{
			get { return _text; }
			set { CheckProperty(ref _text, value); }
		}

		/// <summary>
		///		ViewModel de log
		/// </summary>
		public Tools.LogViewModel LogViewModel
		{
			get { return _logViewModel; }
			set { CheckProperty(ref _logViewModel, value); }
		}

		/// <summary>
		///		Ultimo directorio seleccionado al abrir / grabar un archivo
		/// </summary>
		public string LastPathSelected { get; set; }

		/// <summary>
		///		Comando para añadir una conexión al explorador
		/// </summary>
		public BaseCommand AddConnectionCommand { get; }
		
		/// <summary>
		///		Comando para añadir una carpeta al explorador
		/// </summary>
		public BaseCommand AddFolderToExplorerCommand { get; }

		/// <summary>
		///		Comando para grabar el elemento actual
		/// </summary>
		public BaseCommand SaveCommand { get; }

		/// <summary>
		///		Comando para grabar el elemento actual con un nuevo nombre
		/// </summary>
		public BaseCommand SaveAsCommand { get; }

		/// <summary>
		///		Comando para actualizar los datos
		/// </summary>
		public BaseCommand RefreshCommand { get; }
	}
}
