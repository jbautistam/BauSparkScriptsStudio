using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.BauSparkScripts.Models;
using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.Application.Repository
{
	/// <summary>
	///		Repository de <see cref="SolutionModel"/>
	/// </summary>
	internal class SolutionRepository
	{
		// Constantes privadas
		private const string TagRoot = "ScriptSolution";
		private const string TagName = "Name";
		private const string TagDescription = "Description";
		private const string TagFileParameters = "FileParameters";
		private const string TagId = "Id";
		private const string TagConnection = "Connection";
		private const string TagConnectionString = "ConnectionString";
		private const string TagFolder = "Folder";

		/// <summary>
		///		Carga los datos de una solución
		/// </summary>
		internal SolutionModel Load(string fileName)
		{
			SolutionModel solution = new SolutionModel();
			MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

				// Carga los datos de la solución
				if (fileML != null)
					foreach (MLNode rootML in fileML.Nodes)
						if (rootML.Name == TagRoot)
						{
							// Asigna las propiedades
							solution.FileName = fileName;
							solution.GlobalId = rootML.Attributes[TagId].Value;
							solution.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
							solution.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
							solution.LastParametersFileName = rootML.Nodes[TagFileParameters].Value.TrimIgnoreNull();
							// Carga los objetos
							LoadConnections(solution, rootML);
							LoadFolders(solution, rootML);
						}
				// Devuelve la solución
				return solution;
		}

		/// <summary>
		///		Carga los datos de conexión
		/// </summary>
		private void LoadConnections(SolutionModel solution, MLNode rootML)
		{
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagConnection)
				{
					ConnectionModel connection = new ConnectionModel(solution);

						// Asigna las propiedades
						connection.GlobalId = nodeML.Attributes[TagId].Value;
						connection.Name = nodeML.Nodes[TagName].Value.TrimIgnoreNull();
						connection.Description = nodeML.Nodes[TagDescription].Value.TrimIgnoreNull();
						connection.ConnectionString = nodeML.Attributes[TagConnectionString].Value.TrimIgnoreNull();
						// Añade los datos a la solución
						solution.Connections.Add(connection);
				}
		}

		/// <summary>
		///		Carga las carpetas de la solución
		/// </summary>
		private void LoadFolders(SolutionModel solution, MLNode rootML)
		{
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagFolder)
					if (!string.IsNullOrWhiteSpace(nodeML.Value) && System.IO.Directory.Exists(nodeML.Value))
						solution.Folders.Add(nodeML.Value.TrimIgnoreNull());
		}

		/// <summary>
		///		Graba los datos de una solución
		/// </summary>
		internal void Save(SolutionModel solution, string fileName)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add(TagRoot);

				// Añade los datos de la solución
				rootML.Attributes.Add(TagId, solution.GlobalId);
				rootML.Nodes.Add(TagName, solution.Name);
				rootML.Nodes.Add(TagDescription, solution.Description);
				rootML.Nodes.Add(TagFileParameters, solution.LastParametersFileName);
				// Añade los objetos
				rootML.Nodes.AddRange(GetConnectionsNodes(solution));
				rootML.Nodes.AddRange(GetFoldersNodes(solution));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
		}

		/// <summary>
		///		Obtiene los nodos para los datos de conexión de una solución
		/// </summary>
		private MLNodesCollection GetConnectionsNodes(SolutionModel solution)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los datos
				foreach (ConnectionModel connection in solution.Connections)
				{
					MLNode nodeML = nodesML.Add(TagConnection);

						// Añade los datos
						nodeML.Attributes.Add(TagId, connection.GlobalId);
						nodeML.Nodes.Add(TagName, connection.Name);
						nodeML.Nodes.Add(TagDescription, connection.Description);
						nodeML.Attributes.Add(TagConnectionString, connection.ConnectionString);
				}
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene los nodos con las carpetas de una solución
		/// </summary>
		private MLNodesCollection GetFoldersNodes(SolutionModel solution)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los datos
				foreach (string folder in solution.Folders)
					nodesML.Add(TagFolder, folder);
				// Devuelve la colección de nodos
				return nodesML;
		}
	}
}
