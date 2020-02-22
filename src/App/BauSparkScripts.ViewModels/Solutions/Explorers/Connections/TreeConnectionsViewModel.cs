using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel para el árbol de conexiones
	/// </summary>
	public class TreeConnectionsViewModel : BaseTreeViewModel
	{
		public TreeConnectionsViewModel(SolutionViewModel solutionViewModel) : base(solutionViewModel)
		{
			NewConnectionCommand = new BaseCommand(parameter => OpenConnection(null), parameter => CanExecuteAction(nameof(NewConnectionCommand)))
										.AddListener(this, nameof(SelectedNode));
			NewQueryCommand = new BaseCommand(parameter => OpenQuery(null));
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			// Ordena las conexiones
			SolutionViewModel.Solution.Connections.SortByName();
			// Añade los nodos
			foreach (Models.Connections.ConnectionModel connection in SolutionViewModel.Solution.Connections)
				Children.Add(new NodeConnectionViewModel(this, null, connection));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			BaseTreeNodeViewModel.NodeType nodeType = GetSelectedNodeType();

				switch (action)
				{
					case nameof(NewConnectionCommand):
					case nameof(NewQueryCommand):
						return true;
					case nameof(OpenPropertiesCommand):
						return nodeType == BaseTreeNodeViewModel.NodeType.Connection || nodeType == BaseTreeNodeViewModel.NodeType.Table;
					default:
						return false;
				}
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetSelectedNodeType())
			{
				case BaseTreeNodeViewModel.NodeType.Connection:
						OpenConnection((SelectedNode as NodeConnectionViewModel)?.Tag as Models.Connections.ConnectionModel);
					break;
				case BaseTreeNodeViewModel.NodeType.Table:
						OpenQuery((SelectedNode as NodeTableViewModel)?.Tag as Models.Connections.ConnectionTableModel);
					break;
			}
		}

		/// <summary>
		///		Abre un modelo de conexión
		/// </summary>
		internal void OpenConnection(Models.Connections.ConnectionModel connection)
		{
			SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Connections.ConnectionViewModel(SolutionViewModel, connection));
		}

		/// <summary>
		///		Abre una consulta
		/// </summary>
		private void OpenQuery(Models.Connections.ConnectionTableModel table)
		{
			SolutionViewModel.MainViewModel.MainController.OpenWindow(new Details.Connections.ExecuteQueryViewModel(SolutionViewModel, GetQuery(table)));
		}

		/// <summary>
		///		Obtiene una consulta sobre una tabla
		/// </summary>
		private string GetQuery(Models.Connections.ConnectionTableModel table)
		{
			if (table == null)
				return string.Empty;
			else
			{
				string fields = string.Empty;

					// Obtiene la lista de campos
					foreach (Models.Connections.ConnectionTableFieldModel field in table.Fields)
						fields = fields.AddWithSeparator($"`{field.Name}`", ",");
					// Devuelve la consulta
					return $"SELECT {fields}{Environment.NewLine}\tFROM {table.FullName}";
			}
		}

		/// <summary>
		///		Borra el elemento seleccionado
		/// </summary>
		protected override void DeleteItem()
		{	
			switch (SelectedNode?.Tag)
			{
				case Models.Connections.ConnectionModel item:
						DeleteConnection(item);
					break;
			}
		}

		/// <summary>
		///		Borra un proceso de conexión
		/// </summary>
		private void DeleteConnection(Models.Connections.ConnectionModel connection)
		{
			if (SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Realmente desea borrar los datos de conexión {connection.Name}?"))
			{
				// Borra la conexión
				SolutionViewModel.Solution.Connections.Remove(connection);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
			}
		}

		/// <summary>
		///		Comando de nueva conexión
		/// </summary>
		public BaseCommand NewConnectionCommand { get; }

		/// <summary>
		///		Comando para crear una nueva consulta
		/// </summary>
		public BaseCommand NewQueryCommand { get; }
	}
}