using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace LeaveTrackerSystem.WebApp.Services.Pdf
{
    public class LeavePdfService
    {
        public byte[] GenerateLeaveRequestPdf(Dictionary<string, (int used, int remaining)> leaveSummary)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
            var fontHeader = new XFont("Arial", 12, XFontStyle.Bold);
            var fontCell = new XFont("Arial", 12, XFontStyle.Regular);
            var pen = new XPen(XColors.Black, 0.5); // Thin black border

            double y = 40;
            double rowHeight = 25;

            // Title 
            gfx.DrawString("Leave Summary Report", fontTitle, XBrushes.Black, new XRect(0, y, page.Width, rowHeight), XStringFormats.TopCenter);
            y += 40;

            // Table Widths
            double col1Width = 150;
            double col2Width = 100;
            double col3Width = 100;
            double totalTableWidth = col1Width + col2Width + col3Width + 20; // padding
            double x = (page.Width - totalTableWidth) / 2; // center table

            // Header Row
            gfx.DrawString("Leave Type", fontHeader, XBrushes.Black, new XRect(x, y, col1Width, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawString("Used", fontHeader, XBrushes.Black, new XRect(x + col1Width + 10, y, col2Width, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawString("Remaining", fontHeader, XBrushes.Black, new XRect(x + col1Width + col2Width + 20, y, col3Width, rowHeight), XStringFormats.CenterLeft);

            // line below header
            gfx.DrawLine(pen, x, y + rowHeight, x + totalTableWidth, y + rowHeight);
            y += rowHeight;

            // Table Rows
            foreach (var item in leaveSummary)
            {
                gfx.DrawString(item.Key, fontCell, XBrushes.Black, new XRect(x, y, col1Width, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(item.Value.used.ToString(), fontCell, XBrushes.Black, new XRect(x + col1Width + 10, y, col2Width, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(item.Value.remaining.ToString(), fontCell, XBrushes.Black, new XRect(x + col1Width + col2Width + 20, y, col3Width, rowHeight), XStringFormats.CenterLeft);
                y += rowHeight;
            }

            // Save to stream
            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }
    }
}
