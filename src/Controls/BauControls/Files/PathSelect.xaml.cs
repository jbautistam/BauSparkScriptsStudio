using System;
using System.Windows;
using System.Windows.Controls;

namespace Bau.Controls.Files
{
	/// <summary>
	///		Control de usuario para selección de un archivo
	/// </summary>
	public partial class PathSelect : UserControl
	{ 
		// Propiedades
		public static readonly DependencyProperty PathNameProperty = DependencyProperty.Register(nameof(PathName), typeof(string), typeof(PathSelect),
																								 new FrameworkPropertyMetadata(null,
																															   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
																															   null));
		// Eventos
		public event EventHandler Changed;

		public PathSelect()
		{
			InitializeComponent();
			grdPathSelect.DataContext = this;
		}

		/// <summary>
		///		Abre el cuadro de diálogo apropiado
		/// </summary>
		private void OpenDialog()
		{
			Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folder = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

				// Asigna la carpeta inicial
				folder.SelectedPath = PathName;
				folder.ShowNewFolderButton = true;
				// Muestra el diálogo
				if (folder.ShowDialog() ?? false)
					PathName = folder.SelectedPath;
		}

		/// <summary>
		///		Nombre de directorio
		/// </summary>
		public string PathName
		{
			get { return GetValue(PathNameProperty) as string; }
			set
			{
				SetValue(PathNameProperty, value);
				txtPath.Text = value;
				OnChanged();
			}
		}

		/// <summary>
		///		Lanza el evento de modificación
		/// </summary>
		protected virtual void OnChanged()
		{
			Changed?.Invoke(this, EventArgs.Empty);
		}

		private void cmdSelect_Click(object sender, RoutedEventArgs e)
		{
			OpenDialog();
		}
	}
}
