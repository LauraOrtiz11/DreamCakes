using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using DreamCakes.Dtos.Delivery;

namespace DreamCakes.Utilities
{
    public static class DeliveryReportGenerator
    {
        public static byte[] GeneratePaymentReceipt(DeliveryPaymentDetailsDto paymentDetails, decimal amountReceived, bool isFullPayment)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Tamaño carta (Letter) en puntos (612x792)
                var document = new Document(PageSize.LETTER, 40, 40, 40, 40);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                // Logo de la empresa
                var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "images", "image 1.png");
                if (File.Exists(logoPath))
                {
                    var logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(150f, 150f);
                    logo.Alignment = Element.ALIGN_CENTER;
                    document.Add(logo);
                }

                // Título
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                var title = new Paragraph("COMPROBANTE DE PAGO", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Información de la empresa
                var companyInfo = new Paragraph();
                companyInfo.Add(new Chunk("Dream Cakes\n", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                companyInfo.Add(new Chunk("NIT: 123456789-0\n", FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                companyInfo.Add(new Chunk("Teléfono: (123) 456-7890\n", FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                companyInfo.Add(new Chunk("Fecha: " + DateTime.Now.ToString("g") + "\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                document.Add(companyInfo);

                // Tabla de información del pago
                var table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 30, 70 });

                AddTableHeader(table, "Concepto", "Detalle");
                AddTableRow(table, "No. Pedido", paymentDetails.OrderId.ToString());
                AddTableRow(table, "Cliente", paymentDetails.CustomerName);
                AddTableRow(table, "Dirección", paymentDetails.DeliveryAddress);
                AddTableRow(table, "Fecha Entrega", paymentDetails.DeliveryDate.ToString("g"));
                AddTableRow(table, "Total Pedido", paymentDetails.TotalAmount.ToString("C"));
                AddTableRow(table, "Monto Recibido", amountReceived.ToString("C"));

                if (!isFullPayment)
                {
                    AddTableRow(table, "Saldo Pendiente", (paymentDetails.TotalAmount - amountReceived).ToString("C"));
                }

                AddTableRow(table, "Estado Pago", isFullPayment ? "COMPLETO" : "PARCIAL");
                AddTableRow(table, "Método Pago", "EFECTIVO");

                document.Add(table);

                // Pie de página
                var footer = new Paragraph("\n\nGracias por su compra!\n", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10));
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();
                return memoryStream.ToArray();
            }
        }

        private static void AddTableHeader(PdfPTable table, string header1, string header2)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);

            var cell1 = new PdfPCell(new Phrase(header1, headerFont));
            var cell2 = new PdfPCell(new Phrase(header2, headerFont));

            cell1.BackgroundColor = new BaseColor(51, 122, 183); // Azul Bootstrap
            cell2.BackgroundColor = new BaseColor(51, 122, 183);

            cell1.Padding = 5f;
            cell2.Padding = 5f;

            table.AddCell(cell1);
            table.AddCell(cell2);
        }

        private static void AddTableRow(PdfPTable table, string label, string value)
        {
            var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
            var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            table.AddCell(new Phrase(label, labelFont));
            table.AddCell(new Phrase(value, valueFont));
        }
    }
}