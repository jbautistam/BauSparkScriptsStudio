using System;
using System.Collections.Generic;

using Bau.Libraries.LibLogger.Models.Log;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Bau.Libraries.LibLogger.Writer.AppInsights
{
	/// <summary>
	///		Clase para mostrar el log en una consola
	/// </summary>
	public class LogAppInsigthsWriter : Interfaces.LogWriterBase
	{
		// Variables privadas
		private TelemetryClient _client;

		public LogAppInsigthsWriter(string appName, string instrumentationKey, List<LogModel.LogType> typesToLog = null) : base(1, typesToLog) 
		{
			AppName = appName;
			InstrumentationKey = instrumentationKey;
		}

		/// <summary>
		///		Escribe los datos del contexto
		/// </summary>
		public override void Flush(List<LogModel> logItems)
		{
			if (_client != null) // ... se consulta directamente el objeto global para que no se cree si no se había creado nunca
				try
				{
					// Envía el log
					Client.Flush();
					// Espera un momento a que termine el envío (sí, realmente hace falta)
					System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
				}
				catch (Exception exception)
				{
					System.Diagnostics.Trace.TraceError($"Error when flush appinsights '{exception.Message}'");
				}
		}

		/// <summary>
		///		Escribe un mensaje informativo
		/// </summary>
		protected override void WriteStream(LogModel log)
		{
			try
			{
				// Lanza el evento
				Client.TrackEvent(Convert(log));
				// Lanza la excepción si es necesario
				if (log.Exception != null || log.Type == LogModel.LogType.Error)
					Client.TrackException(new AppException(log));
			}
			catch (Exception exception)
			{
				System.Diagnostics.Trace.TraceError($"Error when raise appinsight event '{exception.Message}'");
			}
		}

		/// <summary>
		///		Crea un evento para AppInsights
		/// </summary>
		private Microsoft.ApplicationInsights.DataContracts.EventTelemetry Convert(LogModel log)
		{
			Microsoft.ApplicationInsights.DataContracts.EventTelemetry telemetry = new Microsoft.ApplicationInsights.DataContracts.EventTelemetry();

				// Asigna las propiedades del contexto
				telemetry.Context.User.AccountId = Environment.UserName;
				telemetry.Context.User.AuthenticatedUserId = Environment.UserDomainName;
				telemetry.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
				telemetry.Context.Device.OemName = Environment.MachineName;
				telemetry.Context.Cloud.RoleInstance = AppName;
				telemetry.Context.Cloud.RoleName = AppName;
				// Asigna las propiedades del log
				telemetry.Name = AppName;
				telemetry.Properties.Add(nameof(log.Id), log.Id.ToString());
				if (log.Parent != null)
					telemetry.Properties.Add(nameof(log.Parent), log.Parent.Id.ToString());
				telemetry.Properties.Add(nameof(log.Level), log.Level.ToString());
				telemetry.Properties.Add(nameof(log.Type), log.Type.ToString());
				telemetry.Properties.Add(nameof(log.Message), log.Message);
				telemetry.Properties.Add(nameof(log.FileName), log.FileName);
				telemetry.Properties.Add(nameof(log.MethodName), log.MethodName);
				telemetry.Properties.Add(nameof(log.LineNumber), log.LineNumber.ToString());
				telemetry.Properties.Add(nameof(log.CreatedAt), log.CreatedAt.ToString());
				// Añade los parámetros de progreso
				if (log.Type == LogModel.LogType.Progress)
				{
					telemetry.Properties.Add(nameof(log.ActualProgress), log.ActualProgress.ToString());
					telemetry.Properties.Add(nameof(log.TotalProgress), log.TotalProgress.ToString());
				}
				// Asigna los parámetros adicionales
				foreach (KeyValuePair<string, object> parameter in log.Parameters)
					telemetry.Properties.Add($"Parameter_{parameter.Key}", parameter.Value?.ToString());
				// Devuelve el evento
				return telemetry;
		}

		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		public string AppName { get; }

		/// <summary>
		///		Clave de AppInsights
		/// </summary>
		private string InstrumentationKey { get; }

		/// <summary>
		///		Cliente de telemetría
		/// </summary>
		private TelemetryClient Client 
		{
			get
			{
				// Crea el cliente si no existía
				if (_client == null)
				{
					TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();

						// Asigna la clave de instrumentación
						configuration.InstrumentationKey = InstrumentationKey;
						// Crea el cliente
						_client = new TelemetryClient(configuration);
				}
				// Devuelve el cliente
				return _client;
			}
		}
	}
}
