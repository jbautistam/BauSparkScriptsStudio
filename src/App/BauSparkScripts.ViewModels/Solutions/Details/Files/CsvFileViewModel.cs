using System;
using System.Data;

using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.LibParquetFiles;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para visualización de archivos CSV
	/// </summary>
	public class CsvFileViewModel : BaseFileViewModel
	{
		public CsvFileViewModel(SolutionViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "parquet") {}

		/// <summary>
		///		Carga el archivo
		/// </summary>
		protected override DataTable LoadFile(out int totalRecords)
		{
			return new LibCsvFiles.Controllers.CsvDataTableReader(FileParameters)
													.Load(FileName, ActualPage, RecordsPerPage, out totalRecords);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override void SaveFile(string targetFileName)
		{
			using (CsvReader reader = new CsvReader(FileName, FileParameters, null))
			{
				using (ParquetDataWriter writer = new ParquetDataWriter(targetFileName))
				{
					writer.Write(reader);
				}
			}
		}

		/// <summary>
		///		Abre las propiedades del archivo
		/// </summary>
		protected override void OpenFileProperties()
		{
			if (SolutionViewModel.MainViewModel.MainController.OpenDialog(new CsvFilePropertiesViewModel(SolutionViewModel, FileParameters)) == 
					BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				LoadFile();
		}

		/// <summary>
		///		Parámetros del archivo
		/// </summary>
		public LibCsvFiles.Models.FileModel FileParameters { get; } = new LibCsvFiles.Models.FileModel();
	}
}
