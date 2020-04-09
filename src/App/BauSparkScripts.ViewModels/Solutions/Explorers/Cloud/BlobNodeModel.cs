﻿using System;
using System.Collections.Generic;

using Bau.Libraries.LibBlobStorage.Metadata;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Explorers.Cloud
{
	/// <summary>
	///		Clase intermedia para crear el árbol de blobs
	/// </summary>
	public class BlobNodeModel
	{
		public BlobNodeModel(string name, BlobModel blob)
		{
			Name = name;
			Blob = blob;
		}

		/// <summary>
		///		Nombre del archivo
		/// </summary>
		public string Name { get; }

		/// <summary>
		///		Blob
		/// </summary>
		public BlobModel Blob { get; }

		/// <summary>
		///		Elementos hijo
		/// </summary>
		public List<BlobNodeModel> Children { get; } = new List<BlobNodeModel>();
	}
}
