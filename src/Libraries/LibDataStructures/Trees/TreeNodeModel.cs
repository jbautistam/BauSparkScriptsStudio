using System;
using System.Collections.Generic;
using System.Text;

namespace Bau.Libraries.LibDataStructures.Trees
{
	/// <summary>
	///		Modelo con los datos de un árbol
	/// </summary>
	public class TreeNodeModel
	{
		/// <summary>
		///		Depuración del nodo
		/// </summary>
		internal void Debug(StringBuilder builder, string indent)
		{
			// Añade la información del nodo
			builder.AppendLine($"{indent}{nameof(Id)}: {Id} - {nameof(Value)}: {Value?.ToString()}");
			// Añade la información de los atributos
			if (Attributes.Count > 0)
			{
				builder.AppendLine($"{indent}    {nameof(Attributes)}");
				foreach (Collections.ParameterModel attribute in Attributes)
					builder.AppendLine($"{indent}        {nameof(attribute.Id)}: {attribute.Id} - {nameof(attribute.Value)}: {attribute.Value?.ToString()}");
			}
			// Añade la información de los nodos hijo
			if (Nodes.Count > 0)
			{
				builder.AppendLine($"{indent}    {nameof(Nodes)}");
				foreach (TreeNodeModel node in Nodes)
					node.Debug(builder, indent + new string(' ', 8));
			}
		}

		/// <summary>
		///		Clave del nodo
		/// </summary>
		public string Id { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		///		Valor del nodo
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		///		Atributos del nodo
		/// </summary>
		public Collections.ParameterModelCollection Attributes { get; } = new Collections.ParameterModelCollection();

		/// <summary>
		///		Nodos del árbol
		/// </summary>
		public List<TreeNodeModel> Nodes { get; } = new List<TreeNodeModel>();
	}
}
