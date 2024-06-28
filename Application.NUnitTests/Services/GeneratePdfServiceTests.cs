using Application.Services;
using Application.ViewEntities;
using NUnit.Framework.Legacy;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class GeneratePdfServiceTests
    {
        private GeneratePdfService _generatePdfService;

        [SetUp]
        public void Setup()
        {
            _generatePdfService = new GeneratePdfService();
        }

        [Test]
        public void GeneratePdfReport_ValidData_ReturnsByteArray()
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
            var result = _generatePdfService.GeneratePdfReport(data);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsTrue(result.Length > 0);

            // Further validation can be done to read the PDF content if necessary
        }

        [Test]
        public void GeneratePdfReport_EmptyData_ReturnsNonEmptyByteArray()
        {
            // Arrange
            var data = new List<ReportView>();

            // Act
            var result = _generatePdfService.GeneratePdfReport(data);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsTrue(result.Length > 0);

            // Further validation can be done to read the PDF content if necessary
        }
    }
}
