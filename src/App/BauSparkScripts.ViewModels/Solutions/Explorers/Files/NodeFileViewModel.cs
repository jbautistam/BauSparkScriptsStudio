using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Files
{
	/// <summary>
	///		ViewModel de un nodo de archivo
	/// </summary>
	public class NodeFileViewModel : BaseTreeNodeViewModel
	{
		// Variables privadas
		private string _fileName;
		private bool _isFolder;

		public NodeFileViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, string fileName, bool isFolder) 
					: base(trvTree, parent, string.Empty, NodeType.File, isFolder ? IconType.Path : IconType.File, 
						   fileName, isFolder, isFolder,
						   isFolder ? MvvmColor.Navy : MvvmColor.Black)
		{
			FileName = fileName;
			IsFolder = isFolder;
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				Text = System.IO.Path.GetFileName(FileName);
				ToolTipText = FileName;
			}
			else
				Text = "...";
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void LoadNodes()
		{
			if (!string.IsNullOrWhiteSpace(FileName) && System.IO.Directory.Exists(FileName))
			{
				// Carga los directorios
				foreach (string fileName in System.IO.Directory.EnumerateDirectories(FileName))
					AddNode(fileName, true);
				// Carga los archivos
				foreach (string fileName in System.IO.Directory.EnumerateFiles(FileName))
					AddNode(fileName, false);
			}
		}

		/// <summary>
		///		Añade un nodo
		/// </summary>
		private void AddNode(string fileName, bool isFolder)
		{
			Children.Add(new NodeFileViewModel(TreeViewModel, this, fileName, isFolder));
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Indica si se trata de una carpeta
		/// </summary>
		public bool IsFolder
		{
			get { return _isFolder; }
			set { CheckProperty(ref _isFolder, value); }
		}
	}
}
