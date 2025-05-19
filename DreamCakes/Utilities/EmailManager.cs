using System;
using System.Net;
using System.Net.Mail;

namespace DreamCakes.Utilities
{
    public class EmailManager
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
                mail.Body = body;
                mail.IsBodyHtml = true;

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
