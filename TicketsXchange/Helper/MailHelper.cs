
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Net.Mail;
using SendGrid;
using SendGrid.SmtpApi;
using System.Threading;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace TicketsXchange.Helper
{
    public class EmailHelper
    {
        public static void SendActivationMail(string mail, string activationCode)
        {
            string body = "Please click the link to verify your Ticketsxchange account. https://ticketsxchange.com/Home/Confirm?activationCode=" + activationCode;
            SendMail(mail, "Welcome TicketsXchange!", body);
        }
        public static void SendSubscribeMail(string mail)
        {
            string body = "You can easily buy and sell unwanted tickets.";
            SendMail(mail, "Thank you for subscribing TicketsXchange!", body);
        }
        public static void SendForgotPasswordMail(string mail, string activationCode)
        {
            string body = "Please click the link to reset your password. https://ticketsxchange.com/Home/Forgot?activationCode=" + activationCode;
            SendMail(mail, "Reset your password.", body);
        }
        public static void SendSTMPMail(string mail, string subject, string body)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(mail, "You"));
            msg.From = new MailAddress("info@ticketsxchange.co.uk", "TxcDev2019$");
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("info@ticketsxchange.co.uk", "TxcDev2019$");
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public static void SendMail(string mail, string subject, string body)
        {
            Execute(mail, subject, body).Wait();
        }

        static async Task Execute(string mail, string subject, string body)
        {
            var client = new SendGridClient("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");
            var from = new EmailAddress("info@ticketsxchange.co.uk", "Sher Abbas Khan");
            var to = new EmailAddress(mail, "User");
            var plainTextContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, body);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
       
    }
}