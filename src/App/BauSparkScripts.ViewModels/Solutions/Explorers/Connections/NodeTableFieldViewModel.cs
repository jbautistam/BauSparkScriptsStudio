using System;

using Bau.Libraries.BauSparkScripts.Models.Connections;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de un campo de una tabla
	/// </summary>
	public class NodeTableFieldViewModel : BaseTreeNodeViewModel
	{
		public NodeTableFieldViewModel(BaseTreeViewModel trvTree, NodeTableViewModel parent, ConnectionTableFieldModel field) : 
					base(trvTree, parent, field.FullName, NodeType.Table, IconType.Field, field, false)
		{
			Field = field;
			if (field.IsKey)
				Icon = IconType.Key;
		}

		/// <summary>
		///		Carga los nodos del campo
		/// </summary>
		protected override void LoadNodes()
		{
		}

		/// <summary>
		///		Campo asociado al nodo
		/// </summary>
		public ConnectionTableFieldModel Field { get; }
	}
}
