using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Files
{
	/// <summary>
	///		ViewModel de un árbol de archivos
	/// </summary>
	public class TreeFilesViewModel : BaseTreeViewModel
	{	
		public TreeFilesViewModel(SolutionViewModel solutionViewModel) : base(solutionViewModel)
		{ 
			NewFolderCommand = new BaseCommand(_ => CreateFolder(), _ => CanCreateFileOrFolder())
										.AddListener(this, nameof(SelectedNode));
			NewFileCommand = new BaseCommand(_ => CreateFile(), _ => CanCreateFileOrFolder())
										.AddListener(this, nameof(SelectedNode));
			ExportCommand = new BaseCommand(_ => Export());
			ProcessScriptCommand = new BaseCommand(_ => ExecuteScript(), _ => CanExecuteScript())
										.AddListener(this, nameof(SelectedNode));
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			foreach (string path in SolutionViewModel.Solution.Folders)
				Children.Add(new NodeFolderRootViewModel(this, null, path));
		}

		/// <summary>
		///		Añade una carpeta al explorador
		/// </summary>
		internal void AddFolderToExplorer()
		{
			// Selecciona la carpeta
			SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogSelectPath(string.Empty, out string folder);
			// Añade la carpeta a la solución
			if (!string.IsNullOrWhiteSpace(folder) && System.IO.Directory.Exists(folder))
			{
				// Añade la carpeta a la solución
				SolutionViewModel.Solution.AddFolder(folder);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Carga el árbol
				Load();
			}
		}

		/// <summary>
		///		Comprueba si se puede crear una carpeta o un achivo
		/// </summary>
		private bool CanCreateFileOrFolder()
		{
			return !string.IsNullOrWhiteSpace(GetSelectedFolder());
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción general
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			return true;
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetSelectedNodeType())
			{
				case BaseTreeNodeViewModel.NodeType.File:
						OpenFile();
					break;
			}
		}

		/// <summary>
		///		Abre un archivo
		/// </summary>
		private void OpenFile()
		{
			if (SelectedNode is NodeFileViewModel node && !node.IsFolder)
			{
				if (node.FileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.ParquetFileViewModel(SolutionViewModel, node.FileName));
				else
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.FileViewModel(SolutionViewModel, node.FileName));
			}
		}

		/// <summary>
		///		Crea una carpeta
		/// </summary>
		private void CreateFolder()
		{
			string path = GetSelectedFolder();

				if (!string.IsNullOrWhiteSpace(path))
				{
					string fileName = "Nuevo directorio";

						if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowInputString("Nombre del directorio", ref fileName) 
										== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
						{
							// Quita los espacios
							fileName = fileName.TrimIgnoreNull();
							// Crea el directorio  y actualiza el árbol
							if (!string.IsNullOrWhiteSpace(fileName) &&
									LibHelper.Files.HelperFiles.MakePath(System.IO.Path.Combine(path, fileName)))
								Load();
						}
				}
		}

		/// <summary>
		///		Crea un archivo
		/// </summary>
		private void CreateFile()
		{
			string path = GetSelectedFolder();

				if (!string.IsNullOrWhiteSpace(path))
				{
					string fileName = "Nuevo archivo.sql";

						if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowInputString("Nombre del archivo", ref fileName) 
										== BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
						{
							// Quita los espacios
							fileName = fileName.TrimIgnoreNull();
							// Crea el archivo y actualiza el árbol
							if (!string.IsNullOrWhiteSpace(fileName))
							{
								LibHelper.Files.HelperFiles.SaveTextFile(System.IO.Path.Combine(path, fileName), string.Empty, System.Text.Encoding.UTF8);
								Load();
							}
						}
				}
		}

		/// <summary>
		///		Obtiene la carpeta seleccionada
		/// </summary>
		private string GetSelectedFolder()
		{
			string path = string.Empty;

				// Obtiene la carpeta del nodo
				if (SelectedNode != null)
				{
					if (SelectedNode is NodeFolderRootViewModel nodeFolder)
						path = nodeFolder.Path;
					else if (SelectedNode is NodeFileViewModel pathNode)
					{
						if (pathNode.IsFolder)
							path = pathNode.FileName;
						else
							path = System.IO.Path.GetDirectoryName(pathNode.FileName);
					}
				}
				// Devuelve la carpeta
				return path;
		}

		/// <summary>
		///		Borra el elemento seleccionado
		/// </summary>
		protected override void DeleteItem()
		{	
			switch (SelectedNode)
			{
				case NodeFolderRootViewModel item:
						DeleteRoot(item);
					break;
				case NodeFileViewModel item:
						DeleteFile(item.FileName);
					break;
			}
		}

		/// <summary>
		///		Borra una carpeta de la solución
		/// </summary>
		private void DeleteRoot(NodeFolderRootViewModel item)
		{
			if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea quitar la carpeta '{item.Text}' de la solución?"))
			{
				// Elimina la carpeta
				SolutionViewModel.Solution.RemoveFolder(item.Path);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
			}
		}

		/// <summary>
		///		Borra un directorio o archivo del sistema
		/// </summary>
		private void DeleteFile(string fileName)
		{
			if (System.IO.Directory.Exists(fileName))
			{
				if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea eliminar el directorio {System.IO.Path.GetFileName(fileName)}?"))
				{
					// Elimina el directorio
					LibHelper.Files.HelperFiles.KillPath(fileName);
					// Actualiza el árbol
					Load();
				}
			}
			else if (System.IO.File.Exists(fileName))
			{
				if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea eliminar el archivo {System.IO.Path.GetFileName(fileName)}?"))
				{
					// Elimina el archivo
					LibHelper.Files.HelperFiles.KillFile(fileName);
					// Actualiza el árbol
					Load();
				}
			}
		}

		/// <summary>
		///		Exporta un directorio a Notebooks
		/// </summary>
		private void Export()
		{
			if (string.IsNullOrWhiteSpace(SolutionViewModel.ConnectionExecutionViewModel.FileNameParameters))
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un archivo de parámetros");
			else
			{
				string path = GetSelectedFolder();

					if (string.IsNullOrWhiteSpace(path))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una carpeta");
					else if (!System.IO.Directory.Exists(path))
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se encuentra el directorio");
					else if (SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogSelectPath
									(SolutionViewModel.MainViewModel.LastPathSelected,
									 out string targetPath) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					{
						(LibDataStructures.Collections.NormalizedDictionary<object> parameters, string error) = SolutionViewModel.ConnectionExecutionViewModel.GetParameters();

							if (!string.IsNullOrWhiteSpace(error))
								SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
							else
							{
								// Ejecuta la exportación
								SolutionViewModel.MainViewModel.Manager.ExportToDataBricks(path, targetPath, parameters);
								// Limpia el log
								SolutionViewModel.MainViewModel.Manager.Logger.Flush();
							}
					}
			}
		}

		/// <summary>
		///		Ejecuta un script
		/// </summary>
		private void ExecuteScript()
		{
			string fileName = GetSelectedFile();

				if (string.IsNullOrEmpty(fileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un archivo o carpeta");
				else if (!System.IO.File.Exists(fileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se encuentra el archivo");
				else if (!fileName.EndsWith(".sql"))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Sólo se pueden ejecutar archivos SQL");
				else
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"Ejecutar: {fileName}");
		}

		/// <summary>
		///		Comprueba si se puede ejecutar un script
		/// </summary>
		private bool CanExecuteScript()
		{
			string fileName = GetSelectedFile();

				return (!string.IsNullOrWhiteSpace(fileName) && fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase)) ||
							!string.IsNullOrWhiteSpace(GetSelectedFolder());
		}

		/// <summary>
		///		Obtiene el archivo seleccionado
		/// </summary>
		private string GetSelectedFile()
		{
			if (SelectedNode != null && SelectedNode is NodeFileViewModel fileNode && !fileNode.IsFolder)
				return fileNode.FileName;
			else
				return string.Empty;
		}

		/// <summary>
		///		Comando para crear una nueva carpeta
		/// </summary>
		public BaseCommand NewFolderCommand { get; }

		/// <summary>
		///		Comando para crear un nuevo archivo
		/// </summary>
		public BaseCommand NewFileCommand { get; }

		/// <summary>
		///		Comando para procesar un script
		/// </summary>
		public BaseCommand ProcessScriptCommand { get; }

		/// <summary>
		///		Comando para exportar
		/// </summary>
		public BaseCommand ExportCommand { get; }
	}
}
