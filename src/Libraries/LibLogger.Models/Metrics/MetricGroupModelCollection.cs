using System;

namespace Bau.Libraries.LibLogger.Models.Metrics
{
	/// <summary>
	///		Colección de <see cref="MetricGroupModel"/>
	/// </summary>
	public class MetricGroupModelCollection : Base.NormalizedModelDictionary<MetricGroupModel>
	{
		public MetricGroupModelCollection(ContextModel context)
		{
			Context = context;
		}

		/// <summary>
		///		Añade una serie de grupos de métricas
		/// </summary>
		internal void AddRange(MetricGroupModelCollection groups)
		{
			foreach ((string key, MetricGroupModel group) in groups.Enumerate())
				Add(key, group);
		}

		/// <summary>
		///		Crea un grupo de métricas de un tipo
		/// </summary>
		public TypeData Create<TypeData>(string key, MetricUnitModel unit) where TypeData : MetricGroupModel, new()
		{
			TypeData group = new TypeData();
			
				// Asigna las propiedades básicas del grupo
				group.Key = key;
				group.Unit = unit;
				// Añade el valor
				Add(key, group);
				// Devuelve el valor
				return group;
		}

		/// <summary>
		///		Obtiene un grupo de métricas
		/// </summary>
		public TypeData GetGroup<TypeData>(string key) where TypeData : MetricGroupModel
		{
			return this[key] as TypeData;
		}

		/// <summary>
		///		Inicializa todos los grupos
		/// </summary>
		public void Reset()
		{
			foreach ((string _, MetricGroupModel group) in Enumerate())
				group.Reset();
		}

		/// <summary>
		///		Contexto de la métrica
		/// </summary>
		public ContextModel Context { get; }
	}
}
