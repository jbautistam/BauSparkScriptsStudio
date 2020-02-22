using System;
using System.Windows.Data;

using Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers;

namespace Bau.SparkScripts.Studio.Converters
{
	/// <summary>
	///		Conversor de iconos
	/// </summary>
	public class FileIconConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un tipo en un icono
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			if (value is BaseTreeNodeViewModel.IconType icon)
				return GetIcon(icon);
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(BaseTreeNodeViewModel.IconType icon)
		{
			switch (icon)
			{ 
				case BaseTreeNodeViewModel.IconType.Connection:
					return "/BauSparkScripts.Studio;component/Resources/Images/Connection.png";
				case BaseTreeNodeViewModel.IconType.Project:
					return "/BauSparkScripts.Studio;component/Resources/Images/Project.png";
				case BaseTreeNodeViewModel.IconType.Path:
					return "/BauSparkScripts.Studio;component/Resources/Images/FolderNode.png";
				case BaseTreeNodeViewModel.IconType.File:
					return "/BauSparkScripts.Studio;component/Resources/Images/File.png";
				case BaseTreeNodeViewModel.IconType.Table:
					return "/BauSparkScripts.Studio;component/Resources/Images/Table.png";
				case BaseTreeNodeViewModel.IconType.Key:
					return "/BauSparkScripts.Studio;component/Resources/Images/Key.png";
				case BaseTreeNodeViewModel.IconType.Field:
					return "/BauSparkScripts.Studio;component/Resources/Images/Field.png";
				default:
					return null;
			}
		}

		/// <summary>
		///		Convierte un valor de vuelta
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			throw new NotImplementedException();
		}
	}
}
