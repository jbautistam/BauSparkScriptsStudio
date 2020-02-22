using System;

namespace Bau.Libraries.LibLogger.Models.Metrics
{
	/// <summary>
	///		Elementos de un grupo
	/// </summary>
	public class MetricGroupItemsModel<TypeData> where TypeData : MetricModel, new()
	{
		/// <summary>
		///		Inicializa los grupos
		/// </summary>
		public void Reset()
		{
			foreach ((string _, TypeData item) in Items.Enumerate())
				item.Reset();
		}

		/// <summary>
		///		Indizador del grupo
		/// </summary>
		public TypeData this[string key]
		{
			get 
			{ 
				if (!Items.Contains(key))
					return Items.Add(key, new TypeData());
				else
					return Items[key]; 
			}
			set { Items.Add(key, value); }
		}

		/// <summary>
		///		Número de elementos
		/// </summary>
		public int Count 
		{
			get { return Items.Count; }
		}

		/// <summary>
		///		Elementos del grupo
		/// </summary>
		public Base.NormalizedModelDictionary<TypeData> Items { get; } = new Base.NormalizedModelDictionary<TypeData>();
	}
}
