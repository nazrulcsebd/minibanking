using Microsoft.Extensions.Configuration;
using CodeBonds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CodeBonds.Utility
{
    public class EmailAgent
    {
        public EmailAgent(string toEmail = "")
        {
            var smtp = Startup.StaticConfig.GetSection("smtp").GetChildren()
                    .Select(item => new KeyValuePair<string, string>(item.Key, item.Value));

            SmtpConfig.From = smtp.Where(o => o.Key == "from").FirstOrDefault().Value;
            SmtpConfig.To = string.IsNullOrEmpty(toEmail) ? smtp.Where(o => o.Key == "to").FirstOrDefault().Value : toEmail;
            SmtpConfig.Host = smtp.Where(o => o.Key == "host").FirstOrDefault().Value;
            SmtpConfig.Port = Int32.Parse(smtp.Where(o => o.Key == "port").FirstOrDefault().Value);
            SmtpConfig.EnableSsl = true;
            NetworkCredential credentials = new NetworkCredential(smtp.Where(o => o.Key == "username").FirstOrDefault().Value,
                smtp.Where(o => o.Key == "password").FirstOrDefault().Value);
            SmtpConfig.Credentials = credentials;
        }

        public async Task SendEmail(MailMessage message)
        {
            var smtpClient = new SmtpClient
            {
                Host = SmtpConfig.Host,
                Port = SmtpConfig.Port,
                EnableSsl = SmtpConfig.EnableSsl,
                Credentials = SmtpConfig.Credentials
            };

            try
            {
                var client = new SmtpClient();
                client.Host = smtpClient.Host;
                client.Port = smtpClient.Port;
                client.Credentials = smtpClient.Credentials;
                client.EnableSsl = smtpClient.EnableSsl;

                message.IsBodyHtml = true;
                message.From = new MailAddress(SmtpConfig.From);
                message.To.Add(SmtpConfig.To);
                await client.SendMailAsync(message);
                client.Dispose();
                message.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

            }

        }
    }

    class SmtpConfig
    {
        public static string From { get; set; }
        public static string To { get; set; }
        public static string Host { get; set; }
        public static int Port { get; set; }
        public static bool EnableSsl { get; set; }
        public static NetworkCredential Credentials { get; set; }
    }
}
