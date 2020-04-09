﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.BauSparkScripts.Models.Cloud;
using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibBlobStorage.Metadata;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Cloud
{
	/// <summary>
	///		ViewModel de un nodo de contenedor del storage
	/// </summary>
	public class NodeStorageContainerViewModel : BaseTreeNodeAsyncViewModel
	{
		public NodeStorageContainerViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, StorageModel storage, 
											 string container) 
					: base(trvTree, parent, container, NodeType.StorageContainer, IconType.Path, container, true, 
						   true, MvvmColor.Green)
		{
			Storage = storage;
			Container = container;
		}

		/// <summary>
		///		Obtiene los nodos hijo de un contenedor
		/// </summary>
		protected override async Task<List<BaseTreeNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
		{
			List<BaseTreeNodeViewModel> nodes = new List<BaseTreeNodeViewModel>();

				// Carga los blobs
				try
				{
					ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(Storage.GetNormalizedConnectionString());
					List<BlobNodeModel> tree = TransformToTree(await manager.ListBlobsAsync(Container, string.Empty));

						// Carga la lista dependiendo del tipo de nodo
						nodes = GetBlobsFromContainer(tree);
				}
				catch (Exception exception)
				{
					nodes.Add(new NodeMessageViewModel(TreeViewModel, this, $"Error al cargar los archivos del contenedor. {exception.Message}"));
					TreeViewModel.SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al cargar los archivos del contenedor. {exception.Message}");
				}
				// Devuelve la colección de nodos
				return nodes;
		}

		/// <summary>
		///		Obtiene los blobs de un contenedor
		/// </summary>
		private List<BaseTreeNodeViewModel> GetBlobsFromContainer(List<BlobNodeModel> tree)
		{
			List<BaseTreeNodeViewModel> nodes = new List<BaseTreeNodeViewModel>();

				// Transforma el árbol de blob en un árbol de nodos
				nodes.AddRange(GetTreeNodes(tree, Parent));
				// Devuelve la colección de nodos
				return nodes;
		}

		/// <summary>
		///		Añade un nodo al árbol de nodos reproduciendo la estructura de directorios
		/// </summary>
		private List<BaseTreeNodeViewModel> GetTreeNodes(List<BlobNodeModel> blobs, IHierarchicalViewModel parent)
		{
			List<BaseTreeNodeViewModel> nodes = new List<BaseTreeNodeViewModel>();

				// Añade los nodos
				foreach (BlobNodeModel blob in blobs)
				{
					BaseTreeNodeViewModel node = new NodeStorageContainerFileViewModel(TreeViewModel, parent, Storage, blob, blob.Children.Count > 0);

						// Añade los nodos hijo
						if (blob.Children.Count > 0)
						{
							List<BaseTreeNodeViewModel> children = GetTreeNodes(blob.Children, node);

								foreach (BaseTreeNodeViewModel child in children)
									node.Children.Add(child);
						}
						// Añade el nodo a la lista
						nodes.Add(node);
				}
				// Devuelve la colección de nodos
				return nodes;
		}

		/// <summary>
		///		Transforma una lista de elementos en un árbol
		/// </summary>
		private List<BlobNodeModel> TransformToTree(List<BlobModel> items)
		{
			List<BlobNodeModel> tree = new List<BlobNodeModel>();

				// Ordena los elementos
				items.Sort((first, second) => first.FullFileName.CompareTo(second.FullFileName));
				// Convierte los elementos en un árbol
				foreach (BlobModel item in items)
					AddNode(tree, null, item, item.FullFileName.Split('/'), 0);
				// Devuelve el árbol de elementos
				return tree;

				void AddNode(List<BlobNodeModel> nodes, BlobNodeModel parent, BlobModel blob, string[] parts, int index)
				{
					if (parts.Length > index)
					{
						BlobNodeModel previous = null;

							// Busca un nodo con el mismo nombre
							foreach (BlobNodeModel child in nodes)
								if (child.Name.Equals(parts[index], StringComparison.CurrentCultureIgnoreCase))
									previous = child;
							// Si se ha encontrado, se añade en alguno de sus hijos
							if (previous != null)
								AddNode(previous.Children, previous, blob, parts, index + 1);
							else
							{
								BlobNodeModel blobNode = new BlobNodeModel(parts[index], blob);

									// Añade el blob al árbol
									if (parent != null)
										parent.Children.Add(blobNode);
									else
										nodes.Add(blobNode);
									// Añade los nodos hijo
									AddNode(blobNode.Children, blobNode, blob, parts, index + 1);
							}
					}
				}
		}

		/// <summary>
		///		Storage
		/// </summary>
		public StorageModel Storage { get; }

		/// <summary>
		///		Nombre del contenedor
		/// </summary>
		public string Container { get; }

		/// <summary>
		///		Blob en el contenedor
		/// </summary>
		public BlobModel Blob { get; }
	}
}
