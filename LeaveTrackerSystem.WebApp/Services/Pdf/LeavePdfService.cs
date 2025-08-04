using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace LeaveTrackerSystem.WebApp.Services.Pdf
{
    public class LeavePdfService
    {
        public byte[] GenerateLeaveRequestPdf()
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            gfx.DrawString("Leave Summary PDF", new XFont("Arial", 20, XFontStyle.Bold), XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.TopCenter);

            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }
    }
}
