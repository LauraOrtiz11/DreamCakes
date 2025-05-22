using System;
using System.Net;
using System.Net.Mail;

namespace DreamCakes.Utilities
{
    public class EmailManagerUtility
    {
        private readonly string senderEmail = "dcakesnot@gmail.com";
        private readonly string senderPassword = "qoymshdreeneawuy";

        public bool SendEmail(string recipientEmail, string subject, string body, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail, "Dream Cakes");
                mail.To.Add(recipientEmail);
                mail.Subject = subject;
                mail.IsBodyHtml = true;

                // Crear la vista HTML con imagen embebida
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                // Ruta física de la imagen (ajusta según sea Web o consola)
                string imagePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Images/DreamCakes.jpg");

                if (!System.IO.File.Exists(imagePath))
                {
                    errorMessage = "La imagen no existe en la ruta: " + imagePath;
                    return false;
                }

                LinkedResource image = new LinkedResource(imagePath, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                image.ContentId = "dreamcakeslogo";
                image.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                htmlView.LinkedResources.Add(image);

                mail.AlternateViews.Add(htmlView);

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = $"Error de correo SMTP: {smtpEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error general al enviar correo: {ex.Message}";
                return false;
            }
        }

    }
}
