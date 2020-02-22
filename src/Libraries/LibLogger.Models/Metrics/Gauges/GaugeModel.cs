using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Gauges
{
	/// <summary>
	///		Métrica para almacenar un valor
	/// </summary>
	public class GaugeModel : MetricModel
	{
		// Variables privadas
		private	bool _first = true;

		/// <summary>
		///		Asigna el valor
		/// </summary>
		public void Assign(double value)
		{	
			// Asigna el valor
			Value = value;
			// Si es la primera vez, asigna el máximo y mínimo, si no, aplica las condiciones
			if (_first)
			{
				// Asigna los valores
				Maximum = value;
				Minimum = value;
				// Indica que ya no es la primera vez
				_first = false;
			}
			else
			{
				if (value > Maximum)
					Maximum = value;
				if (value < Minimum)
					Minimum = value;
			}
		}

		/// <summary>
		///		Clona los datos
		/// </summary>
		internal override MetricModel Clone()
		{
			return new GaugeModel
							{
								Key = Key,
								Unit = Unit.Clone(),
								Value = Value,
								Maximum = Maximum,
								Minimum = Minimum
							};
		}

		/// <summary>
		///		Inicializa el valor
		/// </summary>
		public override void Reset()
		{
			_first = true;
			Value = 0;
			Maximum = 0;
			Minimum = 0;
		}

		/// <summary>
		///		Valor del gauge
		/// </summary>
		public double Value { get; private set; }

		/// <summary>
		///		Valor máximo
		/// </summary>
		public double Maximum { get; private set; }

		/// <summary>
		///		Valor mínimo
		/// </summary>
		public double Minimum { get; private set; }
	}
}
