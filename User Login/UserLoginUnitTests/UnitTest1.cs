using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using User_Login.Controllers;
using User_Login.Models;
using System.Web.Mvc;

namespace UserLoginUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Register()
        {
            User user = new User();

            bool result = user.RegisterUser("", "", "", "", "1234565635536-436345-12412-4-15");

            Assert.IsTrue(false);
        }

        [TestMethod]
        public void EmailConfirmation()
        {
            User user = new User();

            bool result = user.Confirmed("john.doe@email.com");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Login()
        {
            LoginUser user = new LoginUser();

            user.IsValid("john.doe@email.com", "123456");

            Assert.IsTrue(false);
        }
    }
}
