using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibLogger.Models.Metrics.Histograms
{
	/// <summary>
	///		Métrica con un histograma
	/// </summary>
	public class HistogramModel : MetricModel
	{
		/// <summary>
		///		Añade un valor al histograma
		/// </summary>
		public void Add(double value)
		{
			Values.Add(value);
		}

		/// <summary>
		///		Inicializa el histograma
		/// </summary>
		public override void Reset()
		{
			Values.Clear();
		}

		/// <summary>
		///		Clona la métrica
		/// </summary>
		internal override MetricModel Clone()
		{
			HistogramModel target = new HistogramModel
											{
												Key = Key,
												Unit = Unit.Clone()
											};

				// Rellena los valores
				foreach (double value in Values)
					target.Values.Add(value);
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Valores
		/// </summary>
		internal List<double> Values { get; } = new List<double>();

		/// <summary>
		///		Cuenta el número de elementos
		/// </summary>
		public int Count
		{
			get { return Values.Count; }
		}

		/// <summary>
		///		Suma
		/// </summary>
		public double Total
		{
			get
			{
				double sum = 0;

					// Acumula los valores
					foreach (double value in Values)
						sum += value;
					// Devuelve la suma
					return sum;
			}
		}

		/// <summary>
		///		Media
		/// </summary>
		public double Average
		{
			get
			{
				if (Values.Count == 0)
					return 0;
				else
					return Total / Values.Count;
			}
		}

		/// <summary>
		///		Valor máximo
		/// </summary>
		public double Maximum
		{
			get
			{
				if (Values.Count == 0)
					return 0;
				else
				{
					double maximum = Values[0];

						// Obtiene el valor máximo
						foreach (double value in Values)
							if (value > maximum)
								maximum = value;
						// Devuelve el valor máximo
						return maximum;
				}
			}
		}

		/// <summary>
		///		Valor mínimo
		/// </summary>
		public double Minimum
		{
			get
			{
				if (Values.Count == 0)
					return 0;
				else
				{
					double minimum = Values[0];

						// Obtiene el valor mínimo
						foreach (double value in Values)
							if (value < minimum)
								minimum = value;
						// Devuelve el valor mínimo
						return minimum;
				}
			}
		}
	}
}
