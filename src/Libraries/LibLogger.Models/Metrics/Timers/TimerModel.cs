using System;

using Bau.Libraries.LibLogger.Models.Metrics.Base;
using Bau.Libraries.LibLogger.Models.Metrics.Histograms;

namespace Bau.Libraries.LibLogger.Models.Metrics.Timers
{
	/// <summary>
	///		Definición de una métrica para medir tiempos y ratios de sucesos
	/// </summary>
	public class TimerModel : MetricModel
	{
		/// <summary>
		///		Marca una métrica
		/// </summary>
		public void Mark(double value)
		{
			Meters.Add(CreateNewHistogram(value), 
					   item => {
									item.Add(value);
									return item;
							   }
					);
		}

		/// <summary>
		///		Crea un nuevo histograma
		/// </summary>
		private HistogramModel CreateNewHistogram(double value)
		{
			HistogramModel histogram = new HistogramModel();

				// Añade el valor al histograma
				histogram.Add(value);
				// Devuelve el histograma creado
				return histogram;
		}

		/// <summary>
		///		Obtiene las marcas de los últimos n minutos
		/// </summary>
		private HistogramModel GetMarksLast(int minutes)
		{
			HistogramModel histogram = new HistogramModel();

				// Obtiene el histograma con los elementos del intervalo
				foreach (HistogramModel item in Meters.GetMarksLast(minutes))
					histogram.Values.AddRange(item.Values);
				// Devuelve el histograma
				return histogram;
		}

		/// <summary>
		///		Clona el objeto
		/// </summary>
		internal override MetricModel Clone()
		{
			return new TimerModel
							{
								Key = Key,
								Unit = Unit.Clone(),
								Meters = Meters.Clone()
							};
		}

		/// <summary>
		///		Inicializa la métrica
		/// </summary>
		public override void Reset()
		{
			Meters.Clear();
		}

		/// <summary>
		///		Métricas
		/// </summary>
		private TimeSerieModel<HistogramModel> Meters { get; set; } = new TimeSerieModel<HistogramModel>();

		/// <summary>
		///		Obtiene las métricas del último minuto
		/// </summary>
		public HistogramModel LastMinute 
		{ 
			get { return GetMarksLast(1); }
		}

		/// <summary>
		///		Obtiene las métricas de los últimos cinco minutos
		/// </summary>
		public HistogramModel LastFiveMinutes 
		{ 
			get { return GetMarksLast(5); }
		}

		/// <summary>
		///		Obtiene las métricas de los últimos quince minutos
		/// </summary>
		public HistogramModel LastFifteenMinutes 
		{ 
			get { return GetMarksLast(15); }
		}
	}
}
