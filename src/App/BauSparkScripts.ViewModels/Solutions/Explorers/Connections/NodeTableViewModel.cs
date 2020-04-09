﻿using System;

using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de tabla
	/// </summary>
	public class NodeTableViewModel : BaseTreeNodeViewModel
	{
		public NodeTableViewModel(BaseTreeViewModel trvTree, NodeConnectionViewModel parent, ConnectionTableModel table) : 
					base(trvTree, parent, table.FullName, NodeType.Table, IconType.Table, table, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
		{
			Table = table;
		}

		/// <summary>
		///		Obtiene el texto de la cadena SQL asociada a la tabla
		/// </summary>
		public string GetSqlSelect(bool fullSql)
		{
			if (TreeViewModel is TreeConnectionsViewModel trvTree)
				return trvTree.GetSqlSelectText(this, fullSql);
			else
				return string.Empty;
		}

		/// <summary>
		///		Carga los nodos de la tabla
		/// </summary>
		protected override void LoadNodes()
		{
			foreach (ConnectionTableFieldModel field in Table.Fields)
				Children.Add(new NodeTableFieldViewModel(TreeViewModel, this, field));
		}

		/// <summary>
		///		Tabla asociada al nodo
		/// </summary>
		public ConnectionTableModel Table { get; }
	}
}
