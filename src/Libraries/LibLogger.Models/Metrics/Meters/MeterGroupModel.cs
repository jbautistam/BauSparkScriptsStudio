using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Meters
{
	/// <summary>
	///		Grupo de <see cref="MeterModel"/>
	/// </summary>
	public class MeterGroupModel : MetricGroupModel
	{
		/// <summary>
		///		Inicializa los valores
		/// </summary>
		public override void Reset()
		{
			Meters.Reset();
		}

		/// <summary>
		///		Clona la métrica
		/// </summary>
		internal override MetricGroupModel Clone()
		{
			MeterGroupModel target = new MeterGroupModel
												{
													Key = Key,
													Unit = Unit.Clone()
												};

				// Añade las métricas
				foreach ((string key, MeterModel meter) in Meters.Items.Enumerate())
					target.Meters.Items.Add(key, meter.Clone() as MeterModel);
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Elementos
		/// </summary>
		public MetricGroupItemsModel<MeterModel> Meters { get; } = new MetricGroupItemsModel<MeterModel>();
	}
}
