using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace helpers
{
    public class email
    {
        private bool isDevelopment = false;
        private readonly IConfiguration _configs;

        public email(IConfiguration configs)
        {
            isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development ||
                            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local";
            _configs = configs;
        }
        public async Task<bool> sendMailAsync(string To,
                                              string Body,
                                              string Subject,
                                              [Optional] string CC,
                                              [Optional] string BCC,
                                              [Optional] Dictionary<string, byte[]> multplie_attachments)
        {
            if (isDevelopment)
            {
                To = "test.npsmt@gmail.com";
            }

            bool emailResponse = false;

            //Calling smtpin.falconide.com for primary email sender
            emailResponse = await sendMailByFalconide(To, Body, Subject, CC, BCC, multplie_attachments);
            return emailResponse;
        }

        //Sending mail from smtpin.falconide.com
        private async Task<bool> sendMailByFalconide(string To,
                                                     string Body,
                                                     string Subject,
                                                     string CC,
                                                     string BCC,
                                                     Dictionary<string, byte[]> attachments)
        {
            try
            {

                MailMessage mail = new MailMessage();

                if (!string.IsNullOrEmpty(To))
                {
                    foreach (string to_mail in To.Split('|'))
                    {
                        mail.To.Add(to_mail);
                    }
                }

                if (!string.IsNullOrEmpty(CC))
                {
                    foreach (string cc_mail in CC.Split('|'))
                    {
                        mail.CC.Add(cc_mail);
                    }
                }

                if (!string.IsNullOrEmpty(BCC))
                {
                    foreach (string bcc_mail in BCC.Split('|'))
                    {
                        mail.Bcc.Add(bcc_mail);
                    }
                }

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachement in attachments)
                    {
                        Attachment fileAttachment = new Attachment(new MemoryStream(attachement.Value), attachement.Key);
                        mail.Attachments.Add(fileAttachment);
                    }
                }

                string EmailSend = _configs.GetSection("emailCredential:EmailSend").Value.ToString();
                string EmailUser = _configs.GetSection("emailCredential:EmailUser").Value.ToString();
                string SMTP = _configs.GetSection("emailCredential:Emailhost").Value.ToString();
                string EmailPass = _configs.GetSection("emailCredential:EmailPass").Value.ToString();
                string EmailPort = _configs.GetSection("emailCredential:EmailPort").Value.ToString();

                SmtpClient SmtpServer = new SmtpClient(SMTP);

                mail.From = new MailAddress(EmailSend, "mastertrust");
                mail.Subject = Subject;
                mail.IsBodyHtml = true;
                mail.Body = Body;
                SmtpServer.EnableSsl = true;
                SmtpServer.Port = Convert.ToInt32(EmailPort);
                SmtpServer.Credentials = new System.Net.NetworkCredential(EmailUser, EmailPass);
                await SmtpServer.SendMailAsync(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
