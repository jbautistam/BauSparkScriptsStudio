using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Counters
{
	/// <summary>
	///		Grupo de <see cref="CounterModel"/>
	/// </summary>
	public class CounterGroupModel : MetricGroupModel
	{
		/// <summary>
		///		Clona los datos
		/// </summary>
		internal override MetricGroupModel Clone()
		{
			CounterGroupModel target = new CounterGroupModel
												{
													Key = Key,
													Unit = Unit.Clone()
												};

				// Clona los contadores
				foreach ((string key, CounterModel counter) in Counters.Items.Enumerate())
					target.Counters.Items.Add(key, counter.Clone() as CounterModel);
				// Devuelve la colección de objetos
				return target;
		}

		/// <summary>
		///		Inicializa los contadores
		/// </summary>
		public override void Reset()
		{
			Counters.Reset();
		}

		/// <summary>
		///		Contadores
		/// </summary>
		public MetricGroupItemsModel<CounterModel> Counters { get; } = new MetricGroupItemsModel<CounterModel>();
	}
}
