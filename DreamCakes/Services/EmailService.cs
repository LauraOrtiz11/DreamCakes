using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Services
{
    public class EmailService
    {
        public void SendStatusUpdateNotification(string email, int orderId, string statusName)
        {
            try
            {
                // Configurar el mensaje
                string subject = $"Actualización de tu pedido #{orderId}";
                string body = $@"
                <h2>Estado de tu pedido actualizado</h2>
                <p>El estado de tu pedido #<strong>{orderId}</strong> ha cambiado a: <strong>{statusName}</strong></p>
                <p>Gracias por confiar en nosotros.</p>
                <p><em>Equipo de DreamCakes</em></p>";

                // Aquí iría el código real para enviar el correo
                // Ejemplo con SmtpClient (simplificado):
                /*
                using (var client = new SmtpClient("smtp.tudominio.com"))
                {
                    var mail = new MailMessage("notificaciones@dreamcakes.com", email)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    client.Send(mail);
                }
                */

                // Por ahora solo lo registramos
                System.IO.File.AppendAllText(
                    "email_logs.txt",
                    $"{DateTime.Now}: Enviado a {email} - {subject}\n");
            }
            catch
            {
                // Registrar error
            }
        }
    }
}