using System;
using System.Data;

using Bau.Libraries.LibParquetFiles;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para visualización de archivos parquet
	/// </summary>
	public class ParquetFileViewModel : BaseFileViewModel
	{
		public ParquetFileViewModel(SolutionViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "csv") {}

		/// <summary>
		///		Carga la página del archivo
		/// </summary>
		protected override DataTable LoadFile(out int totalRecords)
		{
			return new ParquetDataTableReader().ParquetReaderToDataTable(FileName, (ActualPage - 1) * RecordsPerPage, RecordsPerPage, out totalRecords);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override void SaveFile(string targetFileName)
		{
			LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

				// Escribe el archivo
				using (ParquetDataReader reader = new ParquetDataReader(FileName))
				{
					writer.Save(reader, targetFileName);
				}
		}

		/// <summary>
		///		Abre las propiedades del archivo
		/// </summary>
		protected override void OpenFileProperties()
		{
			//TODO --> Por ahora no hace nada
		}
	}
}