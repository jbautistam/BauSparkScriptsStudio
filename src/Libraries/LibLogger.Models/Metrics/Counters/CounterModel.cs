using System;

namespace Bau.Libraries.LibLogger.Models.Metrics.Counters
{
	/// <summary>
	///		Contador
	/// </summary>
	public class CounterModel : MetricModel
	{
		/// <summary>
		///		Incrementa un contador
		/// </summary>
		public void Increment(long increment = 1)
		{
			Sum(increment);
		}

		/// <summary>
		///		Decrementa un contador
		/// </summary>
		public void Decrement(long decrement = 1)
		{
			Sum(-1 * Math.Abs(decrement));
		}

		/// <summary>
		///		Suma un valor a un contador
		/// </summary>
		public void Sum(long value)
		{
			Value += value;
		}

		/// <summary>
		///		Inicializa el valor
		/// </summary>
		public override void Reset()
		{
			Value = 0;
		}

		/// <summary>
		///		Clona el contador
		/// </summary>
		internal override MetricModel Clone()
		{
			return new CounterModel
							{
								Key = Key,
								Unit = Unit.Clone(),
								Value = Value
							};
		}

		/// <summary>
		///		Valor del contador
		/// </summary>
		public long Value { get; private set; }
	}
}
