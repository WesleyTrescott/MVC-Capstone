using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Recaptcha;
using Recaptcha.MvcModel;
using System.Web.UI;
using System.Web.Mvc;
using System.IO;
using System.Configuration;

namespace User_Login.Models
{
    public class ContactModel
    {
        [Required]
        public string firstname { get; set; }

        [Required]
        public string lastname { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string message { get; set; }

        public static string GenerateCaptcha(HtmlHelper helper)
        {
            return RecaptchaControlMvc.GenerateCaptcha(helper, "recaptcha", "default");
        }

        public static string GenerateCaptcha(HtmlHelper helper, string id, string theme)
        {
            if (string.IsNullOrEmpty(RecaptchaControlMvc.PublicKey) || string.IsNullOrEmpty(RecaptchaControlMvc.PrivateKey))
                throw new ApplicationException("reCAPTCHA needs to be configured with a public & private key.");

            RecaptchaControl recaptchaControl1 = new RecaptchaControl();
            recaptchaControl1.ID = id;
            recaptchaControl1.Theme = theme;
            recaptchaControl1.PublicKey = RecaptchaControlMvc.PublicKey;
            recaptchaControl1.PrivateKey = RecaptchaControlMvc.PrivateKey;
            RecaptchaControl recaptchaControl2 = recaptchaControl1;
            HtmlTextWriter writer = new HtmlTextWriter((TextWriter)new StringWriter());
            recaptchaControl2.RenderControl(writer);
            return writer.InnerWriter.ToString();
        }
    }
}