﻿using System;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		Elemento de la ejecución de archivos
	/// </summary>
	public class ExecuteFilesItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
	{
		// Enumerados públicos 
		/// <summary>
		///		Estado de ejecución del archivo
		/// </summary>
		public enum Status
		{
			/// <summary>Encolado</summary>
			Enqueued,
			/// <summary>Cancelado</summary>
			Canceled,
			/// <summary>Iniciado</summary>
			Start,
			/// <summary>Finalizado</summary>
			End,
			/// <summary>Error</summary>
			Error
		}
		// Variables privadas
		private string _path, _fileName, _statusText, _executionTime, _message;
		private DateTime? _start;
		private Status _status;

		public ExecuteFilesItemViewModel(string text, object tag, bool isBold = false, MvvmColor foreground = null) : base(text, tag, isBold, foreground)
		{
			Path = System.IO.Path.GetDirectoryName(text);
			FileName = System.IO.Path.GetFileName(text);
			StatusText = "Pendiente";
			IsChecked = true;
		}

		/// <summary>
		///		Cambia el estado
		/// </summary>
		public void SetStatus(Status newStatus, string message)
		{
			// Cambia el estado
			State = newStatus;
			switch (newStatus)
			{	
				case Status.Canceled:
						_start = null;
						StatusText = "Cancelado";
						Foreground = MvvmColor.Gray;
						IsBold = false;
					break;
				case Status.Enqueued:
						_start = null;
						StatusText = "Pendiente";
						IsBold = false;
					break;
				case Status.Start:
						StatusText = "En ejecución";
						if (_start == null)
							_start = DateTime.Now;
						IsBold = true;
					break;
				case Status.End:
						StatusText = "Finalizado";
						Foreground = MvvmColor.Navy;
						IsBold = false;
					break;
				case Status.Error:
						StatusText = "Error";
						Foreground = MvvmColor.Red;
						IsBold = false;
					break;
			}
			// Cambia el tiempo de ejecución y el mensaje
			if (_start == null)
				ExecutionTime = string.Empty;
			else if (State == Status.Start)
				ExecutionTime = $"{(DateTime.Now - _start).ToString()}";
			Message = message;
		}

		/// <summary>
		///		Directorio
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { CheckProperty(ref _path, value); }
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Estado del elemento
		/// </summary>
		public Status State
		{
			get { return _status; }
			set { CheckProperty(ref _status, value); }
		}

		/// <summary>
		///		Texto del estado
		/// </summary>
		public string StatusText
		{
			get { return _statusText; }
			set { CheckProperty(ref _statusText, value); }
		}

		/// <summary>
		///		Tiempo de ejecución
		/// </summary>
		public string ExecutionTime
		{
			get { return _executionTime; }
			set { CheckProperty(ref _executionTime, value); }
		}

		/// <summary>
		///		Mensaje
		/// </summary>
		public string Message
		{
			get { return _message; }
			set { CheckProperty(ref _message, value); }
		}
	}
}
