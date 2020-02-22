using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Histograms
{
	/// <summary>
	///		Grupo de <see cref="HistogramModel"/>
	/// </summary>
	public class HistogramGroupModel : MetricGroupModel
	{
		/// <summary>
		///		Inicializa los valores
		/// </summary>
		public override void Reset()
		{
			Histograms.Reset();
		}

		/// <summary>
		///		Clona los objetos
		/// </summary>
		internal override MetricGroupModel Clone()
		{
			HistogramGroupModel target = new HistogramGroupModel
													{	
														Key = Key,
														Unit = Unit.Clone()
													};

				// Clona los datos
				foreach ((string key, HistogramModel histogram) in Histograms.Items.Enumerate())
					target.Histograms.Items.Add(key, histogram.Clone() as HistogramModel);
				// Devuelve el objeto
				return target;
		}

		/// <summary>
		///		Elementos
		/// </summary>
		public MetricGroupItemsModel<HistogramModel> Histograms { get; } = new MetricGroupItemsModel<HistogramModel>();
	}
}
