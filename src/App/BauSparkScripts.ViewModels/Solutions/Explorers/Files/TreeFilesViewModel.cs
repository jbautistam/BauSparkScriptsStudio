using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Files
{
	/// <summary>
	///		ViewModel de un árbol de archivos
	/// </summary>
	public class TreeFilesViewModel : BaseTreeViewModel
	{	
		// Variables privadas
		private NodeFileViewModel _nodeToCopy;

		public TreeFilesViewModel(SolutionViewModel solutionViewModel) : base(solutionViewModel)
		{ 
			NewFolderCommand = new BaseCommand(_ => CreateFolder(), _ => CanCreateFileOrFolder())
										.AddListener(this, nameof(SelectedNode));
			NewFileCommand = new BaseCommand(_ => CreateFile(), _ => CanCreateFileOrFolder())
										.AddListener(this, nameof(SelectedNode));
			ProcessScriptCommand = new BaseCommand(_ => ExecuteScript(), _ => CanExecuteScript())
										.AddListener(this, nameof(SelectedNode));
			CopyCommand = new BaseCommand(_ => CopyFile(), _ => CanExecuteAction(nameof(CopyCommand)))
										.AddListener(this, nameof(SelectedNode));
			PasteCommand = new BaseCommand(_ => PasteFile(), _ => CanExecuteAction(nameof(PasteCommand)))
										.AddListener(this, nameof(SelectedNode));
			SeeAtExplorerCommand = new BaseCommand(_ => OpenFileExplorer());
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
			BaseTreeNodeViewModel.NodeType type = GetSelectedNodeType();

				// Devuelve el valor que indica si puede ejecutar la acción
				switch (action)
				{
					case nameof(CopyCommand):
						return type == BaseTreeNodeViewModel.NodeType.File;
					case nameof(PasteCommand):
						return _nodeToCopy != null && SelectedNode != null && 
									(SelectedNode is NodeFolderRootViewModel || ((SelectedNode as NodeFileViewModel)?.IsFolder ?? false));
					default:
						return true;
				}
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
				else if (node.FileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Files.CsvFileViewModel(SolutionViewModel, node.FileName));
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
						path = nodeFolder.FileName;
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
		///		Copia un archivo
		/// </summary>
		private void CopyFile()
		{
			if (SelectedNode is NodeFileViewModel node)
				_nodeToCopy = node;
			else
				_nodeToCopy = null;
		}

		/// <summary>
		///		Pega una carpeta / archivo
		/// </summary>
		private void PasteFile()
		{
			if (_nodeToCopy != null)
			{
				string target = GetSelectedPath();
				string source = _nodeToCopy.FileName;
					
					// Copia el directorio o el archivo
					if (System.IO.Directory.Exists(source))
					{
						if (target.StartsWith(source, StringComparison.CurrentCultureIgnoreCase))
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"No se pude copiar {source} sobre {target}");
						else
						{
							// Obtiene el nombre del directorio destino
							target = LibHelper.Files.HelperFiles.GetConsecutivePath(target, System.IO.Path.GetFileName(source));
							// Copia el directorio
							LibHelper.Files.HelperFiles.CopyPath(source, target);
						}
					}
					else
					{
						// Obtiene el nombre del archivo
						target = LibHelper.Files.HelperFiles.GetConsecutiveFileName(target, System.IO.Path.GetFileName(source));
						// Copia el archivo
						LibHelper.Files.HelperFiles.CopyFile(source, target);
					}
					// Actualiza el árbol
					Load();
					// ... y vacía el nodo de copia
					_nodeToCopy = null;
			}	
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
				SolutionViewModel.Solution.RemoveFolder(item.FileName);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Actualiza el árbol
				Load();
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
		///		Ejecuta un script
		/// </summary>
		private void ExecuteScript()
		{
			List<string> files = GetFilesFromPath(".sql");

				// Ejecuta los archivos (si ha encontrado alguno)
				if (files.Count == 0)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se encuentra ningún archivo SQL para ejecutar");
				else
					SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Connections.ExecuteFilesViewModel(SolutionViewModel, files));
		}

		/// <summary>
		///		Obtiene los archivos SQL de un directorio (o el archivo seleccionado
		/// </summary>
		private List<string> GetFilesFromPath(string extension)
		{
			List<string> files = new List<string>();
			string path = GetSelectedFile();

				// Obtiene el archivo seleccionado o los archivos de un directorio
				if (!string.IsNullOrWhiteSpace(path))
					files.Add(path);
				else
				{
					// Obtiene la lista de todos los archivos
					files = LibHelper.Files.HelperFiles.ListRecursive(GetSelectedFolder(), $"*{extension}");
					// Quita los archivos que no coincidan con la máscara
					for (int index = files.Count - 1; index	>= 0; index--)
						if (!files[index].EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
							files.RemoveAt(index);
					// Ordena los archivos
					files.Sort((first, second) => first.CompareIgnoreNullTo(second));
				}
				// Devuelve la colección de archivos
				return files;
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
		///		Obtiene el directorio seleccionado
		/// </summary>
		private string GetSelectedPath()
		{
			string path = string.Empty;

				// Obtiene el directorio
				if (SelectedNode != null)
				{
					if (SelectedNode is NodeFileViewModel fileNode)
					{
						if (fileNode.IsFolder)
							path = fileNode.FileName;
						else
							path = System.IO.Path.GetDirectoryName(fileNode.FileName);
					}
					else if (SelectedNode is NodeFolderRootViewModel filePath)
						path = filePath.FileName;
				}
				// Devuelve el directorio seleccionado
				return path;
		}

		/// <summary>
		///		Abre el archivo en el explorador
		/// </summary>
		private void OpenFileExplorer()
		{
			string file = GetSelectedFile();
			string path;

				// Obtiene el directorio a abrir
				if (!string.IsNullOrWhiteSpace(file))
					path = System.IO.Path.GetDirectoryName(file);
				else
					path = GetSelectedPath();
				// Abre el explorador sobre el directorio
				if (!string.IsNullOrWhiteSpace(path) && System.IO.Directory.Exists(path))
					SolutionViewModel.MainViewModel.MainController.OpenExplorer(path);
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
		///		Comando para copiar un nodo
		/// </summary>
		public BaseCommand CopyCommand { get; }

		/// <summary>
		///		Comando para pegar un nodo
		/// </summary>
		public BaseCommand PasteCommand { get; }

		/// <summary>
		///		Comando para abrir en el explorador
		/// </summary>
		public BaseCommand SeeAtExplorerCommand { get; }
	}
}
