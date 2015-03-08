using System;
using System.Net.Mail;
using System.Web;
using System.Web.Security;

namespace User_Login.Manager
{ 
    public class EmailManager
    {
        private const string EmailFrom = "computech_capstone@yahoo.com";

        public static void SendConfirmationEmail(string firstName, string email)
        {
           // var user = Membership.GetUser();

            //var confirmationGuid = user.ProviderUserKey.ToString();
            var verifyUrl = HttpContext.Current.Request.Url.GetLeftPart
                (UriPartial.Authority) + "/User/Verify/"; // +confirmationGuid;

            using (var client = new SmtpClient())
            {
                using (var message = new MailMessage(EmailFrom, email))
                {
                    message.Subject = "Please Verify your Account";
                    message.Body = "<html><head><meta content=\"text/html; charset=utf-8\" /></head><body><p>Dear " + firstName +
                        ", </p><p>To verify your account, please click the following link:</p>"
                        + "<p><a href=\"" + verifyUrl + "\" target=\"_blank\">" + verifyUrl
                        + "</a></p><div>Best regards,</div><div>Someone</div><p>Do not forward "
                        + "this email. The verify link is private.</p></body></html>";

                    message.IsBodyHtml = true;

                    client.EnableSsl = true;
                    client.Send(message);
                };
            };
        }
} 
}