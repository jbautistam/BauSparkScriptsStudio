using System;

namespace Bau.Libraries.LibLogger.Models.Metrics
{
	/// <summary>
	///		Clase con los datos de una unidad asociada a una <see cref="MetricModel"/>
	/// </summary>
	public class MetricUnitModel
	{
		// Unidades métricas predeterminadas
        public static readonly MetricUnitModel None = new MetricUnitModel(string.Empty);
        public static readonly MetricUnitModel Requests = new MetricUnitModel("Requests");
        public static readonly MetricUnitModel Commands = new MetricUnitModel("Commands");
        public static readonly MetricUnitModel Calls = new MetricUnitModel("Calls");
        public static readonly MetricUnitModel Events = new MetricUnitModel("Events");
        public static readonly MetricUnitModel Errors = new MetricUnitModel("Errors");
        public static readonly MetricUnitModel Results = new MetricUnitModel("Results");
        public static readonly MetricUnitModel Items = new MetricUnitModel("Items");
		public static readonly MetricUnitModel MegaBytes = new MetricUnitModel("Mb");
        public static readonly MetricUnitModel KiloBytes = new MetricUnitModel("Kb");
        public static readonly MetricUnitModel Bytes = new MetricUnitModel("bytes");
        public static readonly MetricUnitModel Percent = new MetricUnitModel("%");
        public static readonly MetricUnitModel Threads = new MetricUnitModel("Threads");

		public MetricUnitModel(string unit)
		{
			Unit = unit;
		}

		/// <summary>
		///		Operador de casting
		/// </summary>
        public static implicit operator MetricUnitModel(string name)
        {
            return new MetricUnitModel(name);
        }

		/// <summary>
		///		Clona el objeto
		/// </summary>
		internal MetricUnitModel Clone()
		{
			return new MetricUnitModel(Unit);
		}

		/// <summary>
		///		Unidades
		/// </summary>
		public string Unit { get; }
	}
}
