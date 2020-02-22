using System;

namespace Bau.Libraries.LibDbProviders.Spark
{
	/// <summary>
	///		Cadena de conexión de Spark
	/// </summary>
	public class SparkConnectionString : Base.DbConnectionStringBase
	{ 
		public SparkConnectionString(string connectionString, int timeOut = 15) : base(connectionString, timeOut) {}

		/// <summary>
		///		Asigna el valor de un parámetro
		/// </summary>
		protected override void AssignParameter(string key, string value)
		{
			if (IsEqual(key, nameof(ConnectionString)))
				ConnectionString = value;
		}
	}
}