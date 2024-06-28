using Application.Services;
using Application.ViewEntities;
using OfficeOpenXml;
using NUnit.Framework.Legacy;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class GenerateExcelServiceTests
    {
        private GenerateExcelService _generateExcelService;

        [SetUp]
        public void Setup()
        {
            _generateExcelService = new GenerateExcelService();
        }

        [Test]
        public void GenerateExcelReport_ValidData_ReturnsByteArray()
        {
            // Arrange
            var data = new List<ReportView>
            {
                new ReportView
                {
                    Date = DateTime.Now,
                    UserName = "TestUser1",
                    ProjectName = "TestProject1",
                    CategoryName = "TestCategory1",
                    Description = "TestDescription1",
                    Time = 5
                },
                new ReportView
                {
                    Date = DateTime.Now,
                    UserName = "TestUser2",
                    ProjectName = "TestProject2",
                    CategoryName = "TestCategory2",
                    Description = "TestDescription2",
                    Time = 3
                }
            };

            // Act
            var result = _generateExcelService.GenerateExcelReport(data);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsTrue(result.Length > 0);

            using (var package = new ExcelPackage(new System.IO.MemoryStream(result)))
            {
                var worksheet = package.Workbook.Worksheets["Report"];
                ClassicAssert.IsNotNull(worksheet);

                // Check headers
                ClassicAssert.AreEqual("Date", worksheet.Cells[1, 1].Value);
                ClassicAssert.AreEqual("User Name", worksheet.Cells[1, 2].Value);
                ClassicAssert.AreEqual("Project Name", worksheet.Cells[1, 3].Value);
                ClassicAssert.AreEqual("Category Name", worksheet.Cells[1, 4].Value);
                ClassicAssert.AreEqual("Description", worksheet.Cells[1, 5].Value);
                ClassicAssert.AreEqual("Time", worksheet.Cells[1, 6].Value);

                // Check data rows
                for (int i = 0; i < data.Count; i++)
                {
                    ClassicAssert.AreEqual(data[i].Date.ToString("yyyy-MM-dd"), worksheet.Cells[i + 2, 1].Value);
                    ClassicAssert.AreEqual(data[i].UserName, worksheet.Cells[i + 2, 2].Value);
                    ClassicAssert.AreEqual(data[i].ProjectName, worksheet.Cells[i + 2, 3].Value);
                    ClassicAssert.AreEqual(data[i].CategoryName, worksheet.Cells[i + 2, 4].Value);
                    ClassicAssert.AreEqual(data[i].Description, worksheet.Cells[i + 2, 5].Value);
                    ClassicAssert.AreEqual(data[i].Time, worksheet.Cells[i + 2, 6].Value);
                }
            }
        }

        [Test]
        public void GenerateExcelReport_EmptyData_ReturnsEmptyWorksheet()
        {
            // Arrange
            var data = new List<ReportView>();

            // Act
            var result = _generateExcelService.GenerateExcelReport(data);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsTrue(result.Length > 0);

            using (var package = new ExcelPackage(new System.IO.MemoryStream(result)))
            {
                var worksheet = package.Workbook.Worksheets["Report"];
                ClassicAssert.IsNotNull(worksheet);

                // Check headers
                ClassicAssert.AreEqual("Date", worksheet.Cells[1, 1].Value);
                ClassicAssert.AreEqual("User Name", worksheet.Cells[1, 2].Value);
                ClassicAssert.AreEqual("Project Name", worksheet.Cells[1, 3].Value);
                ClassicAssert.AreEqual("Category Name", worksheet.Cells[1, 4].Value);
                ClassicAssert.AreEqual("Description", worksheet.Cells[1, 5].Value);
                ClassicAssert.AreEqual("Time", worksheet.Cells[1, 6].Value);

                // Check no data rows
                ClassicAssert.IsNull(worksheet.Cells[2, 1].Value);
                ClassicAssert.IsNull(worksheet.Cells[2, 2].Value);
                ClassicAssert.IsNull(worksheet.Cells[2, 3].Value);
                ClassicAssert.IsNull(worksheet.Cells[2, 4].Value);
                ClassicAssert.IsNull(worksheet.Cells[2, 5].Value);
                ClassicAssert.IsNull(worksheet.Cells[2, 6].Value);
            }
        }
    }
}
