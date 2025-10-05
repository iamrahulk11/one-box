using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace helpers
{
    public class excel
    {
        public void generateAndSaveFile(DataTable data_table, string directory_path, string file_name)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            //printing headers in excel
            for (int row = 1; row <= data_table.Rows.Count; row++)
            {
                var rowData = data_table.Rows[row - 1];

                for (int col = 1; col <= rowData.Table.Columns.Count; col++)
                {
                    worksheet.Cell(row, col).Value = rowData.Table.Columns[col - 1].ToString();
                }
            }


            for (int row = 1; row <= data_table.Rows.Count; row++)
            {
                var rowData = data_table.Rows[row - 1];

                for (int col = 1; col <= rowData.Table.Columns.Count; col++)
                {
                    worksheet.Cell(row + 1, col).Value = rowData[col - 1].ToString();
                }
            }

            if (!Directory.Exists(directory_path))
                Directory.CreateDirectory(directory_path);

            workbook.SaveAs($"{directory_path}//{file_name}");
        }

        public async Task<DataTable> readExcelFromMemoryIntoDataTableAsync(IFormFile excel_file)
        {
            // Create a new DataTable to store the data
            DataTable dataTable = new DataTable();

            using (var memoryStream = new MemoryStream())
            {
                await excel_file.CopyToAsync(memoryStream);

                using (var workBook = new XLWorkbook(memoryStream))
                {
                    // Assuming the data is in the first worksheet, you can change the worksheet name or index as needed
                    var workSheet = workBook.Worksheet(1);

                    // Add columns to the DataTable based on the headers in the Excel file
                    foreach (var cell in workSheet.FirstRow().CellsUsed())
                    {
                        dataTable.Columns.Add(cell.Value.ToString());
                    }

                    // Loop through the rows and populate the DataTable
                    foreach (var row in workSheet.RowsUsed().Skip(1)) // Skip the header row
                    {
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < row.CellsUsed().Count(); i++)
                        {
                            dataRow[i] = row.Cell(i + 1).Value.ToString();
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            return dataTable;

        }

        public void generateFromDictionary(Dictionary<string, string> dictionary, string file_path)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            int col = 1; // Starts with 1           
            foreach (var key in dictionary.Keys)
            {
                // worksheet.Column(col++).Value = key;                
                worksheet.Cell(1, col++).Value = key;
            }
            workbook.SaveAs(file_path);
        }

    }
}
