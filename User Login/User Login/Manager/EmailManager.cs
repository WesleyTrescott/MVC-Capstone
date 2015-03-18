using System;
using System.Net.Mail;
using System.Web;
using System.Web.Security;

namespace User_Login.Manager
{ 
    public class EmailManager
    {
        private const string EmailFrom = "computech_capstone@yahoo.com";

        public static void SendConfirmationEmail(string firstName, string email, string guid)
        {
            var user = new Models.User();
            var verifyUrl = HttpContext.Current.Request.Url.GetLeftPart
                (UriPartial.Authority) + "/User/Verify?email=" + email + "&id=" + guid;

            using (var client = new SmtpClient())
            { 
                using (var message = new MailMessage(
                    new MailAddress(EmailFrom, "Computech Corporation"),
                    new MailAddress(email)
                    ))
                {
                    message.Subject = "Please Verify Your Computech Account";
                    message.Body = "<html><head><meta content=\"text/html; charset=utf-8\" /></head><body><p>Dear " + firstName +
                        ", </p><p>To verify your account, please click the following link:</p>"
                        + "<p><a href=\"" + verifyUrl + "\" target=\"_blank\">" + verifyUrl
                        + "</a></p><div>Best regards,</div><div>Computech Corporation Human Resources</div><p>Do not forward "
                        + "this email. The verify link is private.</p></body></html>";

                    message.IsBodyHtml = true;

                    client.EnableSsl = true;
                    client.Send(message);
                };
            };
        }

        public static void SendForgotPasswordEmail(string firstName, string email, string guid)
        {
            var user = new Models.User();
            var forgotPasswordUrl = HttpContext.Current.Request.Url.GetLeftPart
                (UriPartial.Authority) + "/User/ChangePassword?email=" + email + "&id=" + guid;

            using (var client = new SmtpClient())
            {
                using (var message = new MailMessage(
                    new MailAddress(EmailFrom, "Computech Corporation"),
                    new MailAddress(email)
                    ))
                {
                    message.Subject = "Computech Corporation - Forgot Password";
                    message.Body = "<html><head><meta content=\"text/html; charset=utf-8\" /></head><body><p>Dear " + firstName +
                        ", </p><p>To change your password, please click the following link:</p>"
                        + "<p><a href=\"" + forgotPasswordUrl + "\" target=\"_blank\">" + forgotPasswordUrl
                        + "</a></p><div>Best regards,</div><div>Computech Corporation Human Resources</div><p>Do not forward "
                        + "this email. The link is private.</p></body></html>";

                    message.IsBodyHtml = true;

                    client.EnableSsl = true;
                    client.Send(message);
                };
            };
        }
} 
}