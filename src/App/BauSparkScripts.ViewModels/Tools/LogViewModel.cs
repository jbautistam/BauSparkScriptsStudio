using System;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Tools
{
	/// <summary>
	///		ViewModel con los datos de log
	/// </summary>
	public class LogViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Variables privadas
		private string _log;

		public LogViewModel(MainViewModel mainViewModel)
		{
			MainViewModel = mainViewModel;
			Log = string.Empty;
			MainViewModel.MainController.Logger.Logged += (sender, args) => WriteLog(args.Item);
		}

		/// <summary>
		///		Escribe la información en el log
		/// </summary>
		private void WriteLog(LogModel item)
		{
			if (Log.Length > 20_000)
				Log = Log.Substring(1000);
			Log += Environment.NewLine + item.Message;
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public MainViewModel MainViewModel { get; }

		/// <summary>
		///		Texto de log
		/// </summary>
		public string Log
		{
			get { return _log; }
			set { CheckProperty(ref _log, value); }
		}
	}
}
