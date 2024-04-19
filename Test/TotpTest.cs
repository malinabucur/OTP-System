using EntityFramework.Models;
using EntityFramework;
using Moq;
using OtpNet;
using System.Text;
using BusinessLogic;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Test
{

    [TestFixture]
    public class TotpTest
    {
        [Test]
        public void ValidateOTP_CorrectOTP_ReturnsTrue()
        {
            // Arrange
            var mockContext = new Mock<OtpsystemContext>();
            var mockTotp = new Mock<TOTP.TOTP>();
            var service = new Service(mockContext.Object, mockTotp.Object);

            var userId = 1;
            var otpCode = "123456";

            // Mock GetOtpCode to return correct values
            mockContext.Setup(c => c.Users).Returns(MockDbSet(new[]
            {
                // Return user with correct UserId and PasswordHash
                new User { UserId = userId, PasswordHash = QuickHash(otpCode) } 
            }));

            // Act
            var result = service.ValidateOTP(otpCode, userId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateOTP_IncorrectOTP_ReturnsFalse()
        {
            // Arrange
            var mockContext = new Mock<OtpsystemContext>();
            var mockTotp = new Mock<TOTP.TOTP>();
            var service = new Service(mockContext.Object, mockTotp.Object);

            var userId = 1;
            var otpCode = "wrongOTP";

            // Mock GetUserById to return a user with incorrect PasswordHash
            mockContext.Setup(c => c.Users).Returns(MockDbSet(new[]
           {
                // Return user with correct UserId and PasswordHash
                new User { UserId = userId, PasswordHash = "correctOTP" } 
            }));

            // Act
            var result = service.ValidateOTP(otpCode, userId);

            // Assert
            Assert.IsFalse(result);
        }

        public static DbSet<T> MockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            return mockSet.Object;
        }

        private static string QuickHash(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }
    }
}