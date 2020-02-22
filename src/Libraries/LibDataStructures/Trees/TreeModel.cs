using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDataStructures.Trees
{
	/// <summary>
	///		Modelo para los datos de un árbol
	/// </summary>
	public class TreeModel
	{
		/// <summary>
		///		Depuración del árbol
		/// </summary>
		public void Debug(System.Text.StringBuilder builder, string indent)
		{
			foreach (TreeNodeModel node in Nodes)
				node.Debug(builder, indent);
		}

		/// <summary>
		///		Nodos del árbol
		/// </summary>
		public List<TreeNodeModel> Nodes { get; } = new List<TreeNodeModel>();
	}
}
