using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Gauges
{
	/// <summary>
	///		Grupo de <see cref="GaugeModel"/>
	/// </summary>
	public class GaugeGroupModel : MetricGroupModel
	{
		/// <summary>
		///		Inicializa los valores
		/// </summary>
		public override void Reset()
		{
			Gauges.Reset();
		}

		/// <summary>
		///		Clona los elementos del grupo
		/// </summary>
		internal override MetricGroupModel Clone()
		{
			GaugeGroupModel target = new GaugeGroupModel
												{
													Key = Key,
													Unit = Unit.Clone()
												};

				// Clona las métricas
				foreach ((string key, GaugeModel gauge) in Gauges.Items.Enumerate())
					target.Gauges.Items.Add(key, gauge.Clone() as GaugeModel);
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Elementos
		/// </summary>
		public MetricGroupItemsModel<GaugeModel> Gauges { get; } = new MetricGroupItemsModel<GaugeModel>();
	}
}
