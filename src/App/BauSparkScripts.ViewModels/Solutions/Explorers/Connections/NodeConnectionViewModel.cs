using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de conexión
	/// </summary>
	public class NodeConnectionViewModel : BaseTreeNodeViewModel
	{
		public NodeConnectionViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, ConnectionModel connection) : 
					base(trvTree, parent, connection.Name, NodeType.Connection, IconType.Connection, connection, true, true, MvvmColor.Red)
		{
			Connection = connection;
		}

		/// <summary>
		///		Carga los nodos de la conexión
		/// </summary>
		protected override void LoadNodes()
		{
			// Carga el esquema de las conexiones
			try
			{
				TreeViewModel.SolutionViewModel.MainViewModel.Manager.LoadSchema(Connection);
			}
			catch (Exception exception)
			{
				Children.Add(new NodeTableViewModel(TreeViewModel, this, 
													new ConnectionTableModel 
															{
																Name = "No se puede cargar el esquema de la conexión" 
															}
													));
				System.Diagnostics.Trace.TraceError($"Error when load schema {exception.Message}");
			}
			// Muestra las tablas
			foreach (ConnectionTableModel table in Connection.Tables)
				Children.Add(new NodeTableViewModel(TreeViewModel, this, table));
		}

		/// <summary>
		///		Conexión asociada al nodo
		/// </summary>
		public ConnectionModel Connection { get; }
	}
}
