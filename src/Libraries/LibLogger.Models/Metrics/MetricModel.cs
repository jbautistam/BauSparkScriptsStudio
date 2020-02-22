using System;

namespace Bau.Libraries.LibLogger.Models.Metrics
{
	/// <summary>
	///		Clase base con los datos de una métrica
	/// </summary>
	public abstract class MetricModel
	{
		/// <summary>
		///		Inicializa el valor
		/// </summary>
		public abstract void Reset();

		/// <summary>
		///		Clona la métrica
		/// </summary>
		internal abstract MetricModel Clone();

		/// <summary>
		///		Clave de la métrica
		/// </summary>
		public string Key { get; internal set; }

		/// <summary>
		///		Unidades de la métrica
		/// </summary>
		public MetricUnitModel Unit { get; internal set; }
	}
}
