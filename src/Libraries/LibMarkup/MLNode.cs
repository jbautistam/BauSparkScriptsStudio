using System;

namespace Bau.Libraries.LibMarkupLanguage
{
	/// <summary>
	///		Datos de un nodo
	/// </summary>
	public class MLNode : MLItemBase
	{
		public MLNode() : this(null, null) { }

		public MLNode(string name) : this(name, null) { }

		public MLNode(string name, string value) : base(name, value) {}

		/// <summary>
		///		Atributos
		/// </summary>
		public MLAttributesCollection Attributes { get; } = new MLAttributesCollection();

		/// <summary>
		///		Nodos
		/// </summary>
		public MLNodesCollection Nodes { get; } = new MLNodesCollection();

		/// <summary>
		///		Espacios de nombres
		/// </summary>
		public MLNameSpacesCollection NameSpaces { get; } = new MLNameSpacesCollection();
	}
}
