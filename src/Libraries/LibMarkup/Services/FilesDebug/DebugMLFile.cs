using System;

namespace Bau.Libraries.LibMarkupLanguage.Services.FilesDebug
{
	/// <summary>
	///		Depuración de <see cref="MLFile"/>
	/// </summary>
	public class DebugMLFile
	{
		/// <summary>
		///		Depura un archivo de nodos
		/// </summary>
		public string Debug(MLFile fileML)
		{
			return Debug(fileML.Nodes, 0);
		}

		/// <summary>
		///		Depura una colección de nodos
		/// </summary>
		private string Debug(MLNodesCollection nodesML, int indent)
		{
			string debug = "";

				// Depura los nodos
				foreach (MLNode nodeML in nodesML)
					debug += Debug(nodeML, indent);
				// Devuelve la cadena de depuración
				return debug;
		}

		/// <summary>
		///		Depura un nodo
		/// </summary>
		private string Debug(MLNode nodeML, int indent)
		{
			string debug = GetIndent(indent);

				// Depura el nodo	y sus atributos
				debug += nodeML.Name + " --> ";
				if (!string.IsNullOrEmpty(nodeML.Value))
					debug += nodeML.Value + " --> ";
				foreach (MLAttribute attributeML in nodeML.Attributes)
					debug += $" {attributeML.Name} = \"{Normalize(attributeML.Value)}\"";
				debug += Environment.NewLine;
				// Depura los nodos hijo
				if (nodeML.Nodes.Count > 0)
					debug += Debug(nodeML.Nodes, indent + 1);
				// Añade una separación
				// debug += Environment.NewLine;
				// Devuelve la cadena de depuración
				return debug;
		}

		/// <summary>
		///		Normaliza una cadena
		/// </summary>
		private string Normalize(string value)
		{
			// Quita los caracteres extraños
			value = value.Replace("\r", "\\r");
			value = value.Replace("\n", "\\n");
			// Devuelve la cadena
			return value;
		}

		/// <summary>
		///		Obtiene la cadena de indentación
		/// </summary>
		private string GetIndent(int indent)
		{
			return new string(' ', indent);
		}
	}
}
