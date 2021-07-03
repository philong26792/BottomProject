using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Bottom_API.Helpers
{
    public interface IMailUtility
    {
        void SendMail(string toMail, string subject, string content, string filePath);

        Task SendMailAsync(string toMail, string subject, string content, string filePath);
        Task SendListMailAsync(List<string> toMail, string subject, string content, string filePath);
    }
    public class MailUtility : IMailUtility
    {
        private readonly IConfiguration _configuration;
        public MailUtility(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMail(string toMail, string subject, string content, string filePath)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient(_configuration.GetSection("MailSettingServer:Server").Value);
            mail.From = new MailAddress(_configuration.GetSection("MailSettingServer:FromEmail").Value, _configuration.GetSection("MailSettingServer:FromName").Value);
            mail.To.Add(toMail);
            mail.Subject = subject;
            mail.Body = content;

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(filePath);
            mail.Attachments.Add(attachment);

            smtpServer.Port = Convert.ToInt32(_configuration.GetSection("MailSettingServer:Port").Value);
            smtpServer.Credentials = new NetworkCredential(_configuration.GetSection("MailSettingServer:UserName").Value, _configuration.GetSection("MailSettingServer:Password").Value);
            smtpServer.EnableSsl = Convert.ToBoolean(_configuration.GetSection("MailSettingServer:EnableSsl").Value);

            try
            {
                smtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendMailAsync(string toMail, string subject, string content, string filePath)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient(_configuration.GetSection("MailSettingServer:Server").Value);
            mail.From = new MailAddress(_configuration.GetSection("MailSettingServer:FromEmail").Value, _configuration.GetSection("MailSettingServer:FromName").Value);
            mail.To.Add(toMail);
            mail.Subject = subject;
            mail.Body = content;

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(filePath);
            mail.Attachments.Add(attachment);

            smtpServer.Port = Convert.ToInt32(_configuration.GetSection("MailSettingServer:Port").Value);
            smtpServer.Credentials = new NetworkCredential(_configuration.GetSection("MailSettingServer:UserName").Value, _configuration.GetSection("MailSettingServer:Password").Value);
            smtpServer.EnableSsl = Convert.ToBoolean(_configuration.GetSection("MailSettingServer:EnableSsl").Value);

            try
            {
                await smtpServer.SendMailAsync(mail);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendListMailAsync(List<string> toMail, string subject, string content, string filePath)
        {
            MailMessage mail = new MailMessage();
            var test = _configuration.GetSection("MailSettingServer:Server").Value;
            SmtpClient smtpServer = new SmtpClient(_configuration.GetSection("MailSettingServer:Server").Value);
            mail.From = new MailAddress(_configuration.GetSection("MailSettingServer:FromEmail").Value, _configuration.GetSection("MailSettingServer:FromName").Value);

            foreach (var item in toMail)
            {
                //if(item.Trim() != "thibac688@126.com" && item.Trim() != "thibacvnltd@163.com")
                mail.To.Add(item.Trim().Replace("\n", string.Empty));
            }
            mail.Subject = subject;
            mail.Body = content;

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(filePath);
            mail.Attachments.Add(attachment);

            smtpServer.Port = Convert.ToInt32(_configuration.GetSection("MailSettingServer:Port").Value);
            smtpServer.Credentials = new NetworkCredential(_configuration.GetSection("MailSettingServer:UserName").Value, _configuration.GetSection("MailSettingServer:Password").Value);
            smtpServer.EnableSsl = Convert.ToBoolean(_configuration.GetSection("MailSettingServer:EnableSsl").Value);

            try
            {
                await smtpServer.SendMailAsync(mail);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}