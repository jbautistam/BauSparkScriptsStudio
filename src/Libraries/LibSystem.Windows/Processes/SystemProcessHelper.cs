﻿using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Bau.Libraries.LibSystem.Processes
{
	/// <summary>
	///		Clase de ayuda para el tratamiento de procesos (ejecutables)
	/// </summary>
	public class SystemProcessHelper
	{
		/// <summary>
		///		Ejecuta la aplicación asociada a un tipo de documento
		/// </summary>
		public void ExecuteApplicationForFile(string fileName)
		{
			ExecuteApplication(null, fileName);
		}

		/// <summary>
		///		Ejecuta una aplicación pasándole los argumentos especificados
		/// </summary>
		public bool ExecuteApplication(string executable, string arguments = null, bool checkPrevious = false)
		{
			bool executed = false;

				// Ejecuta la aplicación
				if (!checkPrevious || !CheckProcessing(executable))
				{
					Process process = new Process();

						// Inicializa las propiedades del proceso
						process.StartInfo.UseShellExecute = true;
						process.StartInfo.RedirectStandardOutput = false;
						if (string.IsNullOrEmpty(executable))
						{
							process.StartInfo.FileName = arguments ?? "";
							process.StartInfo.Arguments = "";
						}
						else
						{
							process.StartInfo.FileName = executable;
							process.StartInfo.Arguments = arguments ?? "";
						}
						process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
						process.StartInfo.CreateNoWindow = true;
						// Ejecuta el proceso
						executed = process.Start();
				}
				// Devuelve el valor que indica si se ha ejecutado
				return executed;
		}

		/// <summary>
		///		Comprueba si se está procesando una aplicación
		/// </summary>
		public bool CheckProcessing(string executable)
		{
			return GetActiveProcesses(executable).Count > 0;
		}

		/// <summary>
		///		Obtiene los procesos activos de un ejecutable
		/// </summary>
		public List<Process> GetActiveProcesses(string executable)
		{
			List<Process> objColProcess = new List<Process>();

				// Obtiene los procesos del ejecutable
				if (!string.IsNullOrEmpty(executable))
				{
					string name = System.IO.Path.GetFileNameWithoutExtension(executable);

						// Recorre los procesos comprobando el nombre de archivo y/o el nombre de proceso
						foreach (Process process in Process.GetProcesses())
							if (process.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase) ||
									process.StartInfo.FileName.Equals(executable, StringComparison.CurrentCultureIgnoreCase))
								objColProcess.Add(process);
				}
				// Devuelve la colección de procesos
				return objColProcess;
		}

		/// <summary>
		///		Elimina un proceso de memoria
		/// </summary>
		public bool Kill(Process process, out string error)
		{ 
			// Inicializa los argumentos de salida
			error = "";
			// Elimina el proceso de memoria
			try
			{
				process.Kill();
			}
			catch (Exception exception)
			{
				error = exception.Message;
			}
			// Devuelve el valor que indica si se ha eliminado el proceso
			return string.IsNullOrWhiteSpace(error);
		}
	}
}
