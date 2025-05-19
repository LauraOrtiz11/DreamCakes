using System;
using DreamCakes.Utilities;

namespace DreamCakes.Services
{
    public class EmailService
    {
        private readonly EmailManager emailManager;

        public EmailService()
        {
            emailManager = new EmailManager();
        }

        public void SendStatusUpdateNotification(string email, int orderId, string statusName)
        {
            string subject = $"Actualización de tu pedido #{orderId}";
            string body = $@"
                <h2>Estado de tu pedido actualizado</h2>
                <p>El estado de tu pedido #<strong>{orderId}</strong> ha cambiado a: <strong>{statusName}</strong></p>
                <p>Gracias por confiar en nosotros.</p>
                <p><em>Equipo de DreamCakes</em></p>";

            string errorMessage;
            bool result = emailManager.SendEmail(email, subject, body, out errorMessage);

            if (!result)
            {
                // Registrar el error si el envío falla
                System.IO.File.AppendAllText(
                    "email_errors.txt",
                    $"{DateTime.Now}: Error al enviar a {email} - {subject}\n{errorMessage}\n");
            }
        }
    }
}
