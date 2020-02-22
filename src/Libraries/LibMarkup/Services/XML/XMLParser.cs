using System;
using System.Xml;

namespace Bau.Libraries.LibMarkupLanguage.Services.XML
{
	/// <summary>
	///		Interpreta un archivo XML
	/// </summary>
	public class XMLParser : IParser
	{
		public XMLParser(bool includeComments = false)
		{
			IncludeComments = includeComments;
		}

		/// <summary>
		///		Carga un archivo
		/// </summary>
		public MLFile Load(string fileName)
		{
			return Load(fileName, out _);
		}

		/// <summary>
		///		Carga un archivo
		/// </summary>
		public MLFile Load(string fileName, out string error)
		{
			// Inicializa los argumentos de salida
			error = string.Empty;
			// Carga el archivo
			try
			{
				return ParseText(Files.FileTools.LoadTextFile(fileName));
			}
			catch (Exception exception)
			{
				error = $"Error al cargar el archivo {fileName}. {exception.Message}";
			}
			// Si ha llegado hasta aquí es porque ha habido algún error
			return null;
		}

		/// <summary>
		///		Interpreta un texto XML
		/// </summary>
		public MLFile ParseText(string text)
		{
			if (string.IsNullOrEmpty(text))
				return new MLFile();
			else
				return Load(XmlReader.Create(new System.IO.StringReader(text)));
		}

		/// <summary>
		///		Abre un Reader XML sin comprobación de caracteres extraños
		/// </summary>
		private XmlReader Open(string fileName)
		{
			XmlReaderSettings settings = new XmlReaderSettings();

				// Carga el documento
				settings.CheckCharacters = false;
				settings.CloseInput = true;
				settings.DtdProcessing = DtdProcessing.Ignore;
				// Devuelve el documento XML abierto
				return XmlReader.Create(fileName, settings);
		}

		/// <summary>
		///		Carga un archivo
		/// </summary>
		public MLFile Load(XmlReader xmlReader)
		{
			MLFile fileML = new MLFile();

				// Carga los datos del archivo
				while (xmlReader.Read())
					switch (xmlReader.NodeType)
					{
						case XmlNodeType.Element:
								fileML.Nodes.Add(LoadNode(xmlReader));
							break;
					}
				// Devuelve el archivo
				return fileML;
		}

		/// <summary>
		///		Carga los datos de un nodo
		/// </summary>
		private MLNode LoadNode(XmlReader xmlReader)
		{
			MLNode nodeML = new MLNode(xmlReader.Name);

				// Asigna los atributos
				nodeML.Attributes.AddRange(LoadAttributes(xmlReader));
				// Lee los nodos
				if (!xmlReader.IsEmptyElement)
				{
					bool isEnd = false;

						// Lee los datos del nodo
						while (!isEnd && xmlReader.Read())
							switch (xmlReader.NodeType)
							{
								case XmlNodeType.Element:
										nodeML.Nodes.Add(LoadNode(xmlReader));
									break;
								case XmlNodeType.Text:
								case XmlNodeType.CDATA:
										nodeML.Value = Decode(xmlReader.Value);
									break;
								case XmlNodeType.EndElement:
										isEnd = true;
									break;
							}
				}
				else
					nodeML.Value = xmlReader.Value;
				// Devuelve el nodo
				return nodeML;
		}

		/// <summary>
		///		Carga los atributos
		/// </summary>
		private MLAttributesCollection LoadAttributes(XmlReader xmlReader)
		{
			MLAttributesCollection attributes = new MLAttributesCollection();

				// Obtiene los atributos
				if (xmlReader.AttributeCount > 0)
				{ 
					// Carga los atributos
					for (int index = 0; index < xmlReader.AttributeCount; index++)
					{ 
						// Pasa al atributo
						xmlReader.MoveToAttribute(index);
						// Asigna los valores del atributo
						if (xmlReader.NodeType == XmlNodeType.Attribute)
							attributes.Add(xmlReader.Name, xmlReader.Value);
					}
					// Pasa al primer atributo de nuevo
					xmlReader.MoveToElement();
				}
				// Devuelve la colección de atributos
				return attributes;
		}

		///// <summary>
		/////		Carga los espacios de nombres
		///// </summary>
		//private MLNameSpacesCollection LoadNameSpaces(XmlAttributeCollection objColXMLAttributes)
		//{ MLNameSpacesCollection objColNameSpaces = new MLNameSpacesCollection();

		//		// Carga los espacios de nombres
		//			if (objColXMLAttributes != null)
		//				foreach (XmlAttribute objXMLAttribute in objColXMLAttributes)
		//					if (objXMLAttribute.Prefix == "xmlns")
		//						{ MLNameSpace nameSpace = new MLNameSpace(objXMLAttribute.LocalName, Decode(objXMLAttribute.InnerText));

		//								// Añade el espacio de nombres
		//									objColNameSpaces.Add(nameSpace);
		//						}
		//		// Devuelve los espacios de nombres
		//			return objColNameSpaces;
		//}

		/// <summary>
		///		Decodifica una cadena HTML
		/// </summary>
		private string Decode(string value)
		{ 
			// Quita los caracteres raros
			if (!string.IsNullOrEmpty(value))
			{
				value = value.Replace("&amp;", "&");
				value = value.Replace("&lt;", "<");
				value = value.Replace("&gt;", ">");
				value = value.Replace("&quot;", "\"");
				value = value.Replace("&aacute;", "á");
				value = value.Replace("&eacute;", "é");
				value = value.Replace("&iacute;", "í");
				value = value.Replace("&oacute;", "ó");
				value = value.Replace("&uacute;", "ú");
			}
			// Devuelve la cadena
			return value;
		}

		/// <summary>
		///		Indica si se deben incluir los comentarios en los nodos
		/// </summary>
		public bool IncludeComments { get; set; }
	}
}
