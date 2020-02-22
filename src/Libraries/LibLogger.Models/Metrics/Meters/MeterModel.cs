using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Meters
{
	/// <summary>
	///		Definición de una métrica para medir tiempos y ratios de sucesos
	/// </summary>
	public class MeterModel : MetricModel
	{
		/// <summary>
		///		Marca una métrica
		/// </summary>
		public void Mark()
		{
			Meters.Add(1, item => item + 1);
		}

		/// <summary>
		///		Obtiene las marcas de los últimos n minutos
		/// </summary>
		private long GetMarksLast(int minutes)
		{
			long counter = 0;

				// Acumula las métricas
				foreach (long value in Meters.GetMarksLast(minutes))
					counter += value;
				// Devuelve el contador
				return counter;
		}

		/// <summary>
		///		Inicializa la métrica
		/// </summary>
		public override void Reset()
		{
			Meters.Clear();
		}

		/// <summary>
		///		Clona los datos de las métricas
		/// </summary>
		internal override MetricModel Clone()
		{
			return new MeterModel
							{
								Key = Key,
								Unit = Unit.Clone(),
								Meters = Meters.Clone()
							};
		}

		/// <summary>
		///		Métricas
		/// </summary>
		private Base.TimeSerieModel<long> Meters { get; set; } = new Base.TimeSerieModel<long>();

		/// <summary>
		///		Obtiene las métricas del último minuto
		/// </summary>
		public long LastMinute 
		{ 
			get { return GetMarksLast(1); }
		}

		/// <summary>
		///		Obtiene las métricas de los últimos cinco minutos
		/// </summary>
		public long LastFiveMinutes 
		{ 
			get { return GetMarksLast(5); }
		}

		/// <summary>
		///		Obtiene las métricas de los últimos quince minutos
		/// </summary>
		public long LastFifteenMinutes 
		{ 
			get { return GetMarksLast(15); }
		}
	}
}
