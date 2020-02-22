using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bau.Libraries.LibHelper.Processes
{
	/// <summary>
	///		Temporizador
	/// </summary>
	/// <remarks>
	///		Las librerías portables no tienen un temporizador. Esta clase implementa uno utilizando tareas (<see cref="Task"/>)
	/// </remarks>
	public sealed class TaskTimer : CancellationTokenSource
	{
		public TaskTimer(Action<object> callback, object state,
						 int millisecondsPeriod, bool waitForCallbackBeforeNextPeriod = true)
		{
			Task.Delay(millisecondsPeriod, Token).ContinueWith
								(async (task, s) =>
													{
														Tuple<Action<object>, object> tuple = (Tuple<Action<object>, object>) s;

															// Mientras no se cancele la tarea
															while (!IsCancellationRequested)
															{   
																// Ejecuta el callback
																if (waitForCallbackBeforeNextPeriod)
																	tuple.Item1(tuple.Item2);
																else
																	await Task.Run(() => tuple.Item1(tuple.Item2));
																// Crea una nueva tarea para que se ejecute dentro de un tiempo
																await Task.Delay(millisecondsPeriod, Token).ConfigureAwait(false);
															}
													},
								Tuple.Create(callback, state), CancellationToken.None,
								TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected override void Dispose(bool disposing)
		{ 
			// Cancela la tarea
			if (disposing)
				Cancel();
			// Llama al base
			base.Dispose(disposing);
		}
	}
}
