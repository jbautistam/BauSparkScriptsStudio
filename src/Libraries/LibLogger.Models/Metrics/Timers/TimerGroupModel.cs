using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Timers
{
	/// <summary>
	///		Grupo de <see cref="TimerModel"/>
	/// </summary>
	public class TimerGroupModel : MetricGroupModel
	{
		/// <summary>
		///		Clona los datos
		/// </summary>
		internal override MetricGroupModel Clone()
		{
			TimerGroupModel target = new TimerGroupModel
												{
													Key = Key,
													Unit = Unit.Clone()
												};

				// Añade los datos clonados
				foreach ((string key, TimerModel timer) in Timers.Items.Enumerate())
					target.Timers.Items.Add(key, timer.Clone() as TimerModel);
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Inicializa los valores
		/// </summary>
		public override void Reset()
		{
			Timers.Reset();
		}

		/// <summary>
		///		Elementos
		/// </summary>
		public MetricGroupItemsModel<TimerModel> Timers { get; } = new MetricGroupItemsModel<TimerModel>();
	}
}
