using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bau.Libraries.BauMvvm.Views.Tools
{
	/// <summary>
	///		Métodos de ayuda para Wpf
	/// </summary>
	public class ToolsWpf
	{
		/// <summary>
		///		Obtener la ventana padre de un control
		/// </summary>
		public Window GetParentWindow(DependencyObject control)
		{
			DependencyObject parent = VisualTreeHelper.GetParent(control);

				// Busca recursivamente la ventana padre
				if (parent == null)
					return null;
				else if (parent is Window)
					return parent as Window;
				else
					return GetParentWindow(parent);
		}

		/// <summary>
		///		Busca en el árbol visual el primer control de un tipo que sea padre del pasado como parámetro
		/// </summary>
		public TypeControl FindAncestor<TypeControl>(DependencyObject source) where TypeControl : DependencyObject
		{ 
			// Recorre el árbol de controles buscando el primer control padre del tipo o hasta que se
			// encuentra el nodo raíz
			do
			{   
				// Si estamos en un objeto del tipo buscado, lo devolvemos, si no, comprobamos el padre
				if (source is TypeControl)
					return source as TypeControl;
				else
					source = VisualTreeHelper.GetParent(source);
			}
			while (source != null);
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Obtiene una imagen a partir de un Uri
		/// </summary>
		/// <param name="uri">La cadena Uri debe ser del tipo "pack://application:,,,/BauControls;component/Themes/Images/Solution.png"</param>
		public Image GetImage(string uri)
		{
			Image image = new Image();

				// Asigna el origen de la imagen
				if (uri != null && !string.IsNullOrEmpty(uri))
					try
					{
						image.Source = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
					}
					catch { }
				// Devuelve la imagen
				return image;
		}
	}
}
