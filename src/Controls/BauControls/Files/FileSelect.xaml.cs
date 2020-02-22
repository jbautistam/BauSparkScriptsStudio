using System;
using System.Windows;
using System.Windows.Controls;

namespace Bau.Controls.Files
{
	/// <summary>
	///		Control de usuario para selección de un archivo
	/// </summary>
	public partial class FileSelect : UserControl
	{
		/// <summary>
		///		Modo de selección del archivo
		/// </summary>
		public enum ModeType
		{
			/// <summary>Cargar</summary>
			Load,
			/// <summary>Grabar</summary>
			Save
		}

		// Propiedades
		public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(FileSelect),
																								 new FrameworkPropertyMetadata(string.Empty,
																															   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty PathBaseProperty = DependencyProperty.Register(nameof(PathBase), typeof(string), typeof(FileSelect),
																								 new FrameworkPropertyMetadata(string.Empty,
																															   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty MaskProperty = DependencyProperty.Register(nameof(Mask), typeof(string), typeof(FileSelect),
																							 new FrameworkPropertyMetadata("Todos los archivos (*.*)|*.*",
																														   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(ModeType), typeof(FileSelect),
																							 new FrameworkPropertyMetadata(ModeType.Load,
																														   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		// Eventos
		public event EventHandler Changed;
		//	public static readonly RoutedEvent ChangedEvent = EventManager.RegisterRoutedEvent("Changed", RoutingStrategy.Bubble, 
		//																					   typeof(RoutedPropertyChangedEventHandler<string>), 
		//																					   typeof(FileSelect));

		public FileSelect()
		{
			InitializeComponent();
			grdFileSelect.DataContext = this;
		}

		/// <summary>
		///		Abre el cuadro de diálogo apropiado
		/// </summary>
		private void OpenDialog()
		{
			string path = null, extension = null;
			string fileName = FileName;

				// Obtiene el directorio
				if (!string.IsNullOrEmpty(fileName))
				{
					path = System.IO.Path.GetDirectoryName(fileName);
					fileName = System.IO.Path.GetFileName(fileName);
					extension = System.IO.Path.GetExtension(fileName);
				} 
				else
					path = PathBase;
				// Abre el cuadro de diálogo apropiado
				if (Mode == ModeType.Load)
					fileName = OpenDialogLoad(path, Mask, fileName, extension);
				else
					fileName = OpenDialogSave(path, Mask, fileName, extension);
				// Asigna el nombre de archivo
				if (!string.IsNullOrEmpty(fileName))
					FileName = fileName;
		}

		/// <summary>
		///		Abre el cuadro de diálogo de carga de archivos
		/// </summary>
		private string OpenDialogLoad(string defaultPath, string filter, string defaultFileName = null, string defaultExtension = null)
		{
			Microsoft.Win32.OpenFileDialog file = new Microsoft.Win32.OpenFileDialog();

				// Asigna las propiedades
				file.InitialDirectory = defaultPath;
				file.FileName = defaultFileName;
				file.DefaultExt = defaultExtension;
				file.Filter = filter;
				// Muestra el cuadro de diálogo
				if (file.ShowDialog() ?? false)
					return file.FileName;
				else
					return null;
		}

		/// <summary>
		///		Abre el cuadro de diálogo de grabación de archivos
		/// </summary>
		private string OpenDialogSave(string defaultPath, string filter, string defaultFileName = null, string defaultExtension = null)
		{
			Microsoft.Win32.SaveFileDialog file = new Microsoft.Win32.SaveFileDialog();

				// Asigna las propiedades
				file.InitialDirectory = defaultPath;
				file.FileName = defaultFileName;
				file.DefaultExt = defaultExtension;
				file.Filter = filter;
				// Muestra el cuadro de diálogo
				if (file.ShowDialog() ?? false)
					return file.FileName;
				else
					return null;
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return GetValue(FileNameProperty) as string; }
			set
			{
				SetValue(FileNameProperty, value);
				OnChanged();
			}
		}

		/// <summary>
		///		Directorio base
		/// </summary>
		public string PathBase
		{
			get { return GetValue(PathBaseProperty) as string; }
			set
			{
				SetValue(PathBaseProperty, value);
				OnChanged();
			}
		}

		/// <summary>
		///		Máscara de archivo
		/// </summary>
		public string Mask
		{
			get { return GetValue(MaskProperty) as string; }
			set { SetValue(MaskProperty, value); }
		}

		/// <summary>
		///		Modo de selección de archivo
		/// </summary>
		public ModeType Mode
		{
			get
			{
				object property = GetValue(ModeProperty);

					if (property != null && property is FileSelect.ModeType)
						return (FileSelect.ModeType) ((int) property);
					else
						return ModeType.Load;
			}
			set { SetValue(ModeProperty, ((int) value).ToString()); }
		}

		/// <summary> 
		///		Lanza el evento Changed
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
