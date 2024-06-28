using Application.ViewEntities;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace Application.Services
{
    public class GeneratePdfService
    {
        public byte[] GeneratePdfReport(IEnumerable<ReportView> data)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 2, 1 })).UseAllAvailableWidth();

                Cell[] headerCells = {
                    new Cell().Add(new Paragraph("Date")).SetBackgroundColor(new DeviceRgb(173, 216, 230)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE),
                    new Cell().Add(new Paragraph("User Name")).SetBackgroundColor(new DeviceRgb(173, 216, 230)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE),
                    new Cell().Add(new Paragraph("Project Name")).SetBackgroundColor(new DeviceRgb(173, 216, 230)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE),
                    new Cell().Add(new Paragraph("Category Name")).SetBackgroundColor(new DeviceRgb(173, 216, 230)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE),
                    new Cell().Add(new Paragraph("Description")).SetBackgroundColor(new DeviceRgb(173, 216, 230)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE),
                    new Cell().Add(new Paragraph("Time")).SetBackgroundColor(new DeviceRgb(173, 216, 230)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)
                };

                foreach (var headerCell in headerCells)
                {
                    table.AddHeaderCell(headerCell);
                }

                bool alternate = false;

                foreach (var item in data)
                {
                    Color rowColor = alternate ?  new DeviceRgb(173, 216, 230) : ColorConstants.WHITE;
                    alternate = !alternate;

                    table.AddCell(new Cell().Add(new Paragraph(item.Date.ToString())).SetBackgroundColor(rowColor)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    table.AddCell(new Cell().Add(new Paragraph(item.UserName)).SetBackgroundColor(rowColor)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    table.AddCell(new Cell().Add(new Paragraph(item.ProjectName)).SetBackgroundColor(rowColor)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    table.AddCell(new Cell().Add(new Paragraph(item.CategoryName)).SetBackgroundColor(rowColor)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    table.AddCell(new Cell().Add(new Paragraph(item.Description)).SetBackgroundColor(rowColor)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    table.AddCell(new Cell().Add(new Paragraph(item.Time.ToString())).SetBackgroundColor(rowColor)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                }
                document.Add(table);

                document.Close();
                return stream.ToArray();
            }
        }
    }
}
