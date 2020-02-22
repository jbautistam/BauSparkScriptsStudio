using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibLogger.Models.Metrics.Base
{
	/// <summary>
	///		Modelo para almacenar los datos de una serie de tiempo
	/// </summary>
	public class TimeSerieModel<TypeData>
	{
		public TimeSerieModel(int maxMinutes = 15)
		{
			MaxMinutes = Math.Abs(maxMinutes);
		}

		/// <summary>
		///		Añade un valor
		/// </summary>
		public void Add(TypeData defaultValue, Func<TypeData, TypeData> computeValue)
		{
			DateTime now = DateTime.Now;
			bool found = false;

				// Añade un valor a la lista
				for (int index = 0; index < Meters.Count; index++)
				{
					(DateTime date, TypeData value) meter = Meters[index];

						// Actualiza la métrica si está en el intervalo
						if (meter.date.Date == now.Date && meter.date.Hour == now.Hour && meter.date.Minute == now.Minute)
						{
							// Calcula el valor
							meter.value = computeValue(meter.value);
							Meters[index] = meter;
							// Indica que se ha modificado
							found = true;
						}
				}
				// Si no se ha modificado, se añade
				if (!found)
					Meters.Add((now, defaultValue));
				// Recicla
				Recycle(now);
		}

		/// <summary>
		///		Clona los elementos
		/// </summary>
		internal TimeSerieModel<TypeData> Clone()
		{
			TimeSerieModel<TypeData> target = new TimeSerieModel<TypeData>();

				// Añade los elementos
				foreach ((DateTime date, TypeData value) in Meters)
					target.Meters.Add((date, value));
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Elimina las métricas antiguas
		/// </summary>
		private void Recycle(DateTime now)
		{
			for (int index = Meters.Count - 1; index >= 0; index--)
				if (Meters[index].date < now.AddMinutes(-1 * MaxMinutes))
					Meters.RemoveAt(index);
		}

		/// <summary>
		///		Obtiene las marcas de los últimos n minutos
		/// </summary>
		public IEnumerable<TypeData> GetMarksLast(int minutes)
		{
			DateTime now = DateTime.Now;

				// Devuelve las métricas
				foreach ((DateTime date, TypeData value) meter in Meters)
					if ((now - meter.date).TotalMinutes < minutes)
						yield return meter.value;
		}

		/// <summary>
		///		Limpia los valores
		/// </summary>
		public void Clear()
		{
			Meters.Clear();
		}

		/// <summary>
		///		Número máximo de minutos que se mantienen las métricas
		/// </summary>
		public int MaxMinutes { get; }

		/// <summary>
		///		Métricas
		/// </summary>
		private List<(DateTime date, TypeData value)> Meters { get; } = new List<(DateTime date, TypeData value)>();
	}
}
