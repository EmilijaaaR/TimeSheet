using Application.ViewEntities;
using OfficeOpenXml;

namespace Application.Services
{
    public class GenerateExcelService
    {
        public byte[] GenerateExcelReport(IEnumerable<ReportView> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Report");
                worksheet.Cells[1, 1].Value = "Date";
                worksheet.Cells[1, 2].Value = "User Name";
                worksheet.Cells[1, 3].Value = "Project Name";
                worksheet.Cells[1, 4].Value = "Category Name";
                worksheet.Cells[1, 5].Value = "Description";
                worksheet.Cells[1, 6].Value = "Time";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = item.UserName;
                    worksheet.Cells[row, 3].Value = item.ProjectName;
                    worksheet.Cells[row, 4].Value = item.CategoryName;
                    worksheet.Cells[row, 5].Value = item.Description;
                    worksheet.Cells[row, 6].Value = item.Time;
                    row++;
                }

                return package.GetAsByteArray();
            }
        }
    }
}
