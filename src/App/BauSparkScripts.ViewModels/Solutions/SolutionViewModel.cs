using System;

using Bau.Libraries.BauSparkScripts.Models;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions
{
	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public class SolutionViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Variables privadas
		private Explorers.Connections.TreeConnectionsViewModel _treeConnectionsViewModel;
		private Explorers.Files.TreeFilesViewModel _treeFoldersViewModel;
		private Explorers.Cloud.TreeStorageViewModel _treeStoragesViewModel;
		private Details.Connections.ConnectionExecutionViewModel _connectionsViewModel;

		public SolutionViewModel(MainViewModel mainViewModel)
		{
			MainViewModel = mainViewModel;
			TreeConnectionsViewModel = new Explorers.Connections.TreeConnectionsViewModel(this);
			TreeFoldersViewModel = new Explorers.Files.TreeFilesViewModel(this);
			TreeStoragesViewModel = new Explorers.Cloud.TreeStorageViewModel(this);
			ConnectionExecutionViewModel = new Details.Connections.ConnectionExecutionViewModel(this);
		}

		/// <summary>
		///		Carga un archivo de solución
		/// </summary>
		public void Load()
		{
			// Carga la solución
			Solution = MainViewModel.Manager.LoadConfiguration();
			// Carga los exploradores
			TreeConnectionsViewModel.Load();
			ConnectionExecutionViewModel.Initialize();
			TreeFoldersViewModel.Load();
			TreeStoragesViewModel.Load();
		}

		/// <summary>
		///		ViewModel de la ventana principal
		/// </summary>
		public MainViewModel MainViewModel { get; }

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionModel Solution { get; private set; }

		/// <summary>
		///		ViewModel del árbol de conexiones
		/// </summary>
		public Explorers.Connections.TreeConnectionsViewModel TreeConnectionsViewModel
		{
			get { return _treeConnectionsViewModel; }
			set { CheckObject(ref _treeConnectionsViewModel, value); }
		}

		/// <summary>
		///		ViewModel del árbol de carpetas
		/// </summary>
		public Explorers.Files.TreeFilesViewModel TreeFoldersViewModel
		{
			get { return _treeFoldersViewModel; }
			set { CheckObject(ref _treeFoldersViewModel, value); }
		}

		/// <summary>
		///		ViewModel del árbol de storage
		/// </summary>
		public Explorers.Cloud.TreeStorageViewModel TreeStoragesViewModel
		{
			get { return _treeStoragesViewModel; }
			set { CheckObject(ref _treeStoragesViewModel, value); }
		}

		/// <summary>
		///		ViewModel de los datos de conexiones
		/// </summary>
		public Details.Connections.ConnectionExecutionViewModel ConnectionExecutionViewModel
		{
			get { return _connectionsViewModel; }
			set { CheckObject(ref _connectionsViewModel, value); }
		}
	}
}
