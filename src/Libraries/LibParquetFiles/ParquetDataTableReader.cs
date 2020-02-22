using System;
using System.Data;
using Parquet;
using Parquet.Data;

namespace Bau.Libraries.LibParquetFiles
{
	/// <summary>
	///     Lector de parquet sobre un DataTable
	/// </summary>
	public class ParquetDataTableReader
	{
		public DataTable ParquetReaderToDataTable(string fileName, int offset, int recordCount, out int totalRecordCount)
		{
			DataTable dataTable = new DataTable();

			using (System.IO.Stream fileReader = System.IO.File.OpenRead(fileName))
			{
				using (ParquetReader parquetReader = new ParquetReader(fileReader))
				{
					//Get list of data fields and construct the DataTable
					DataField[] dataFields = parquetReader.Schema.GetDataFields();

					// Crea las columnas en la tabla
					CreateColumns(dataTable, dataFields);
					//Read column by column to generate each row in the datatable
					totalRecordCount = 0;
					for (int rowGroup = 0; rowGroup < parquetReader.RowGroupCount; rowGroup++)
					{
						int rowsLeftToRead = recordCount;

						using (ParquetRowGroupReader groupReader = parquetReader.OpenRowGroupReader(rowGroup))
						{
							if (groupReader.RowCount > int.MaxValue)
								throw new ArgumentOutOfRangeException(string.Format("Cannot handle row group sizes greater than {0}", groupReader.RowCount));

							int rowsPassedUntilThisRowGroup = totalRecordCount;
							totalRecordCount += (int) groupReader.RowCount;

							if (offset >= totalRecordCount)
								continue;

							if (rowsLeftToRead > 0)
							{
								int numberOfRecordsToReadFromThisRowGroup = Math.Min(Math.Min(totalRecordCount - offset, recordCount), (int) groupReader.RowCount);
								rowsLeftToRead -= numberOfRecordsToReadFromThisRowGroup;

								int recordsToSkipInThisRowGroup = Math.Max(offset - rowsPassedUntilThisRowGroup, 0);

								ProcessRowGroup(dataTable, groupReader, dataFields, recordsToSkipInThisRowGroup, numberOfRecordsToReadFromThisRowGroup);
							}
						}
					}
				}
			}

			return dataTable;
		}

		/// <summary>
		///		Crea las columnas de la tabla
		/// </summary>
		private void CreateColumns(DataTable dataTable, DataField[] fields)
		{
            foreach (DataField field in fields)
                dataTable.Columns.Add(new System.Data.DataColumn(field.Name, ParquetNetTypeToCSharpType(field.DataType)));
		}

		private void ProcessRowGroup(DataTable dataTable, ParquetRowGroupReader groupReader, Parquet.Data.DataField[] fields, int skipRecords, int readRecords)
		{
			int rowBeginIndex = dataTable.Rows.Count;
			bool isFirstColumn = true;

			foreach (var field in fields)
			{
				int rowIndex = rowBeginIndex;

				int skippedRecords = 0;
				foreach (var value in groupReader.ReadColumn(field).Data)
				{
					if (skipRecords > skippedRecords)
					{
						skippedRecords++;
						continue;
					}

					if (rowIndex >= readRecords)
						break;

					if (isFirstColumn)
					{
						var newRow = dataTable.NewRow();
						dataTable.Rows.Add(newRow);
					}

					if (value == null)
						dataTable.Rows[rowIndex][field.Name] = DBNull.Value;
					else if (field.DataType == Parquet.Data.DataType.DateTimeOffset)
						dataTable.Rows[rowIndex][field.Name] = ((DateTimeOffset)value).DateTime; //converts to local time!
					else
						dataTable.Rows[rowIndex][field.Name] = value;

					rowIndex++;
				}

				isFirstColumn = false;
			}
		}

		private Type ParquetNetTypeToCSharpType(DataType type)
		{
			switch (type)
			{
				case Parquet.Data.DataType.Boolean:
					return typeof(bool);
				case Parquet.Data.DataType.Byte:
					return typeof(sbyte);
				case Parquet.Data.DataType.ByteArray:
					return typeof(sbyte[]);
				case Parquet.Data.DataType.DateTimeOffset: // tratamos dateTimeOffsets como dateTime
					return typeof(DateTime);
				case Parquet.Data.DataType.Decimal:
					return typeof(decimal);
				case Parquet.Data.DataType.Double:
					return typeof(double);
				case Parquet.Data.DataType.Float:
					return typeof(float);
				case Parquet.Data.DataType.Short:
				case Parquet.Data.DataType.Int16:
				case Parquet.Data.DataType.Int32:
				case Parquet.Data.DataType.UnsignedInt16:
					return typeof(int);
				case Parquet.Data.DataType.Int64:
					return typeof(long);
				case Parquet.Data.DataType.UnsignedByte:
					return typeof(byte);
				default:
					return typeof(string);
			}
		}
		/*
				private List<string> GetDataTableColumns(DataTable datatable)
				{
					List<string> columns = new List<string>(datatable != null ? datatable.Columns.Count : 0);
					if (datatable != null)
					{
						foreach (DataColumn column in datatable.Columns)
						{
							columns.Add(column.ColumnName);
						}
					}
					return columns;
				}

				private string CleanCSVValue(string value, bool alwaysEncloseInQuotes = false)
				{
					if (!string.IsNullOrWhiteSpace(value))
					{
						//In RFC 4180 we escape quotes with double quotes
						string formattedValue = value.Replace("\"", "\"\"");

						//Enclose value with quotes if it contains commas,line feeds or other quotes
						if (formattedValue.Contains(",") || formattedValue.Contains("\r") || formattedValue.Contains("\n") || formattedValue.Contains("\"\"") || alwaysEncloseInQuotes)
							formattedValue = string.Concat("\"", formattedValue, "\"");

						return formattedValue;
					}
					else
						return string.Empty;
				}
		*/
	}
}
