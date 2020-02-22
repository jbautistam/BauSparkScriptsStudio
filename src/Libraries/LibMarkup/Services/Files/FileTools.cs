using System;
using System.IO;

namespace Bau.Libraries.LibMarkupLanguage.Services.Files
{
	/// <summary>
	///		Clase de ayuda para manejo de archivos
	/// </summary>
	internal static class FileTools
	{
		/// <summary>
		///		Elimina un archivo (no tiene en cuenta las excepciones)
		/// </summary>
		internal static bool KillFile(string fileName)
		{	
			// Quita el atributo de sólo lectura
			//! Está en un try separado porque puede dar problemas de "Acceso denegado"
			try
			{ 
				File.SetAttributes(fileName, FileAttributes.Normal);
			}
			catch {}
			// Borra el archivo
			try
			{ 
				// Elimina el archivo
				File.Delete(fileName);
				// Indica que se ha borrado correctamente
				return true;
			}
			catch (Exception exception)
			{ 
				// Muestra el mensaje
				System.Diagnostics.Debug.WriteLine(exception.Message);
				// Indica que no se ha podido borrar
				return false;
			}
		}	

		/// <summary>
		///		Crea un directorio sin tener en cuenta las excepciones
		/// </summary>
		internal static bool MakePath(string path)
		{ 
			return MakePath(path, out _);
		}

		/// <summary>
		///		Crea un directorio sin tener en cuenta las excepciones
		/// </summary>
		internal static bool MakePath(string path, out string error)
		{ 
			bool made = false;

				// Inicializa los argumentos de salida
				error = "";
				// Crea el directorio
				try
				{ 
					if (path.StartsWith("\\") || ExistsDrive(path))
					{ 
						if (Directory.Exists(path)) // ... si ya existía, intentar crearlo devolvería un error
							made = true;
						else // ... intenta crea el directorio
						{ 
							Directory.CreateDirectory(path);
							made = true;
						}
					}
					else
						error = "No existe la unidad para crear: " + path;
				}
				catch (Exception exception)
				{ 
						error = "Error en la creación del directorio " + path + Environment.NewLine +
									exception.Message + Environment.NewLine + exception.StackTrace;
				}
				// Devuelve el valor que indica si se ha creado
				return made;
		}

		/// <summary>
		///		Comprueba si existe una unidad
		/// </summary>
		private static bool ExistsDrive(string path)
		{ 
			// Comprueba si existe la unidad
			if (!string.IsNullOrEmpty(path))
			{ 
				DriveInfo [] drives = DriveInfo.GetDrives();
						  
					// Quita los espacios
					path = path.Trim();
					// Comprueba si existe la unidad
					foreach (DriveInfo drvDrive in drives)
						if (drvDrive.IsReady && path.StartsWith(drvDrive.Name, StringComparison.CurrentCultureIgnoreCase))
							return true;
			}
			// Si ha llegado hasta aquí es porque la unidad no existe
			return false;
		}
		
		/// <summary>
		/// 	Carga un archivo de texto en una cadena
		/// </summary>
		internal static string LoadTextFile(string fileName)
		{ 
			return LoadTextFile(fileName, GetFileEncoding(fileName, System.Text.Encoding.UTF8));
		}

		/// <summary>
		/// 	Carga un archivo de texto con un encoding determinado en una cadena
		/// </summary>
		internal static string LoadTextFile(string fileName, System.Text.Encoding encoding)
		{	
			System.Text.StringBuilder content = new System.Text.StringBuilder();

				// Carga el archivo
				using (StreamReader file = new StreamReader(fileName, encoding))
				{ 
					string data;

						// Lee los datos
						while ((data = file.ReadLine()) != null)
						{ 
							// Le añade un salto de línea si es necesario
							if (content.Length > 0)
								content.Append("\n");
							// Añade la línea leída
							content.Append(data);
						}
						// Cierra el stream
						file.Close();
				}
				// Devuelve el contenido
				return content.ToString();
		}

		/// <summary>
		///		Obtiene la codificación de un archivo
		/// </summary>
		private static System.Text.Encoding GetFileEncoding(string fileName, System.Text.Encoding encodingDefault = null)
		{ 
			byte[] buffer = new byte[5];

				// Lee los primeros cinco bytes del archivo
				using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{ 
					file.Read(buffer, 0, 5);
				}
				// Obtiene la codificación a partir de los bytes iniciales del archivo
				if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
					return System.Text.Encoding.UTF8;
				else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
					return System.Text.Encoding.Unicode;
				else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xFE && buffer[3] == 0xFF)
					return System.Text.Encoding.UTF32;
				else if (buffer[0] == 0x2B && buffer[1] == 0x2F && buffer[2] == 0x76)
					return System.Text.Encoding.UTF7;
				else if (encodingDefault == null)
					return System.Text.Encoding.UTF8;
				else
					return encodingDefault;
		}

		/// <summary>
		/// 	Graba una cadena en un archivo de texto
		/// </summary>
		internal static void SaveTextFile(string fileName, string text)
		{	
			SaveTextFile(fileName, text, System.Text.Encoding.UTF8);
		}

		/// <summary>
		/// 	Graba una cadena en un archivo de texto
		/// </summary>
		internal static void SaveTextFile(string fileName, string text, string encoding)
		{	
			SaveTextFile(fileName, text, System.Text.Encoding.GetEncoding(encoding));
		}

		/// <summary>
		/// 	Graba una cadena en un archivo de texto
		/// </summary>
		internal static void SaveTextFile(string fileName, string text, System.Text.Encoding encoding)
		{	
			using (StreamWriter file = new StreamWriter(fileName, false, encoding))
			{ 
				// Escribe la cadena
				file.Write(text);
				// Cierra el stream
				file.Close();
			}
		}		
	}
}