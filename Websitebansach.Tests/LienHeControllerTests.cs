using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Websitebansach.Controllers;

namespace Websitebansach.Tests.Controllers
{
    [TestClass]
    public class ContactControllerTests
    {
        [TestMethod]
        public void SendMessage_ValidInput_ReturnsSuccessMessage()
        {
            // Arrange
            var controller = new ContactController();

            string validHoTen = "Test User";
            string validEmail = "testuser@example.com";
            string validSoDienThoai = "123456789";
            string validNoiDung = "This is a test message.";

            // Act
            var result = controller.SendMessage(validHoTen, validEmail, validSoDienThoai, validNoiDung) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Lời nhắn của bạn đã được gửi thành công!", controller.TempData["SuccessMessage"]);
        }

        [TestMethod]
        public void SendMessage_InvalidEmail_ReturnsErrorMessage()
        {
            // Arrange
            var controller = new ContactController();

            string validHoTen = "Test User";
            string invalidEmail = "invalid-email"; // Invalid email format
            string validSoDienThoai = "123456789";
            string validNoiDung = "This is a test message.";

            // Act
            var result = controller.SendMessage(validHoTen, invalidEmail, validSoDienThoai, validNoiDung) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            StringAssert.Contains(controller.TempData["ErrorMessage"].ToString(), "Gửi lời nhắn thất bại");
        }

        [TestMethod]
        [ExpectedException(typeof(SmtpException))]
        public void SendMessage_SmtpFails_ThrowsException()
        {
            // Arrange
            var mockSmtpClient = new Mock<SmtpClient>("smtp.gmail.com", 587);
            mockSmtpClient.Setup(s => s.Send(It.IsAny<MailMessage>())).Throws(new SmtpException("SMTP error"));

            var controller = new ContactController
            {
                SmtpClient = mockSmtpClient.Object // Dependency injection of mocked SMTP client
            };

            string validHoTen = "Test User";
            string validEmail = "testuser@example.com";
            string validSoDienThoai = "123456789";
            string validNoiDung = "This is a test message.";

            // Act
            controller.SendMessage(validHoTen, validEmail, validSoDienThoai, validNoiDung);
        }
    }
}
