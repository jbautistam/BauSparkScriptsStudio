using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Bau.Controls.WebExplorers
{
	/// <summary>
	///		Control de usuario para extensión del explorador Web
	/// </summary>
	public partial class WebExplorerExtended : UserControl
	{ 
		// Propiedades de dependencia
		public static readonly DependencyProperty HiddenScriptErrorsProperty = DependencyProperty.Register(nameof(HiddenScriptErrors), typeof(bool), typeof(WebExplorerExtended),
																										   new FrameworkPropertyMetadata() { DefaultValue = true });
		public static readonly DependencyProperty HtmlContentProperty = DependencyProperty.Register(nameof(HtmlContent), typeof(string), typeof(WebExplorerExtended),
																									new FrameworkPropertyMetadata(string.Empty,
																																  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		// Eventos públicos
		public event EventHandler<WebExplorerFunctionEventArgs> FunctionExecute;

		public WebExplorerExtended()
		{ 
			// Inicializa el componente
			InitializeComponent();
			// Inicializa el objeto que atiende las llamadas de JavaScript
			wbExplorer.ObjectForScripting = new WebExplorerScriptingHelper(this);
		}

		/// <summary>
		///		Muestra una URL
		/// </summary>
		public void ShowUrl(string url)
		{
			try
			{
				HideScriptErrors(HiddenScriptErrors);
				wbExplorer.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception.Message);
			}
		}

		/// <summary>
		///		Muestra una cadena HTML
		/// </summary>
		public void ShowHtml(string html, bool mustRemoveIFrame = true)
		{
			try
			{
				if (mustRemoveIFrame)
					html = RemoveIframe(html);
				HideScriptErrors(HiddenScriptErrors);
				wbExplorer.NavigateToString(html);
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception.Message);
			}
		}

		/// <summary>
		///		Elimina el contenido de las etiquetas iFrame que pueden bloquear el explorador
		/// </summary>
		private string RemoveIframe(string text)
		{
			string result = text;

				// Quita la etiqueta "iframe"
				while (!string.IsNullOrEmpty(result) && result.IndexOf("<iframe") >= 0)
					result = System.Text.RegularExpressions.Regex.Replace(result, "<iframe(.|\n)*?</iframe>", string.Empty);
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Llama a un método de JavaScript
		/// </summary>
		public void InvokeJavaScript(string strMethod, string strArguments)
		{
			wbExplorer.InvokeScript(strMethod, new object [] { strArguments });
		}

		/// <summary>
		///		Oculta los errores de script
		/// </summary>
		private void HideScriptErrors(bool blnHide)
		{
			Dispatcher.Invoke(new Action(() =>
										{
											var axiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

												if (axiComWebBrowser != null)
												{
													var comWebBrowser = axiComWebBrowser.GetValue(wbExplorer);

														if (comWebBrowser == null) // ... en este caso aún no se ha cargado el explorador
															wbExplorer.Loaded += (o, s) => HideScriptErrors(blnHide);
														else
															comWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comWebBrowser, new object [] { blnHide });
												}
										}
							), null);
		}

		/// <summary>
		///		Va una página atrás
		/// </summary>
		public void GoBack()
		{
			if (CanGoBack)
				wbExplorer.GoBack();
		}

		/// <summary>
		///		Va una página adelante
		/// </summary>
		public void GoForward()
		{
			if (CanGoForward)
				wbExplorer.GoForward();
		}

		/// <summary>
		///		Lanza los argumentos de una función javaScript
		/// </summary>
		internal void RaiseScriptArguments(string strJavaScriptArguments)
		{
			FunctionExecute?.Invoke(this, new WebExplorerFunctionEventArgs(strJavaScriptArguments));
		}

		/// <summary>
		///		Indica si se deben mostrar o no los errores de JavaScript
		/// </summary>
		public bool HiddenScriptErrors
		{
			get { return (bool) GetValue(HiddenScriptErrorsProperty); }
			set { SetValue(HiddenScriptErrorsProperty, value); }
		}

		/// <summary>
		///		Texto HTML a mostrar en el navegador
		/// </summary>
		public string HtmlContent
		{
			get { return (string) GetValue(HtmlContentProperty); }
			set
			{ 
				// Asigna el valor
				SetValue(HtmlContentProperty, value);
				// Muestra el HTML
				ShowHtml(value);
			}
		}

		/// <summary>
		///		Comprueba si puede ir una página hacia atrás
		/// </summary>
		public bool CanGoBack
		{
			get { return wbExplorer.CanGoBack; }
		}

		/// <summary>
		///		Comprueba si puede ir una página hacia delante
		/// </summary>
		public bool CanGoForward
		{
			get { return wbExplorer.CanGoForward; }
		}
	}
}
