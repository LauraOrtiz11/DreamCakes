using System;
using DreamCakes.Utilities;

namespace DreamCakes.Services
{
    public class EmailService
    {
        private readonly EmailManagerUtility emailManager;

        public EmailService()
        {
            emailManager = new EmailManagerUtility();
        }

        public void SendStatusUpdateNotification(string email, int orderId, string statusName)
        {
            string subject = $"🍰 Actualización de tu pedido #{orderId}";
            string body = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #fff8f9;
                    color: #4a154b;
                    padding: 20px;
                }}
                .container {{
                    border: 1px solid #f2d9e6;
                    border-radius: 10px;
                    padding: 20px;
                    max-width: 600px;
                    margin: auto;
                    background-color: #ffffff;
                }}
                .header {{
                    text-align: center;
                    color: #6a0572;
                }}
                .status {{
                    background-color: #fce4ec;
                    color: #880e4f;
                    padding: 10px;
                    border-radius: 8px;
                    font-weight: bold;
                    text-align: center;
                    margin: 20px 0;
                }}
                .footer {{
                    text-align: center;
                    font-size: 0.9em;
                    color: #999;
                    margin-top: 20px;
                }}
                .logo {{
                    display: block;
                    margin: 30px auto 10px auto;
                    width: 180px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2 class='header'>🎉 ¡Tu pedido ha sido actualizado!</h2>
                <p>Hola,</p>
                <p>Queremos informarte que el estado de tu pedido <strong>#{orderId}</strong> ha cambiado a:</p>
                <div class='status'>📦 {statusName}</div>
                <p>Gracias por seguir confiando en <strong>DreamCakes</strong>. 💜</p>
                <div class='footer'>
                    <p>Con cariño,<br/>El equipo de DreamCakes</p>
                    <img src='cid:dreamcakeslogo' alt='DreamCakes Logo' class='logo' />
                </div>
            </div>
        </body>
        </html>";

            string errorMessage;
            bool result = emailManager.SendEmail(email, subject, body, out errorMessage);

            if (!result)
            {
                System.IO.File.AppendAllText(
                    "email_errors.txt",
                    $"{DateTime.Now}: Error al enviar a {email} - {subject}\n{errorMessage}\n");
            }
        }

    }
}
