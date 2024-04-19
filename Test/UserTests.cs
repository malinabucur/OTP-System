using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using EntityFramework.Models;
using EntityFramework;
using BusinessLogic;
using Moq;

namespace Test
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void GetUserById_Returns_CorrectUser()
        {
            // Arrange
            var mockContext = new Mock<OtpsystemContext>();
            var mockTotp = new Mock<TOTP.TOTP>();
            var service = new Service(mockContext.Object, mockTotp.Object);

            var users = new[]
            {
                new User { UserId = 1, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1" },
                new User { UserId = 3, Username = "user3", Email = "user3@example.com", PasswordHash = "hash3" },
                new User { UserId = 2, Username = "user2", Email = "user2@example.com", PasswordHash = "hash2" },
            }.AsQueryable();
            mockContext.Setup(c => c.Users).Returns(MockDbSet(users));

            // Act
            var user = service.GetUserById(2);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(2, user.UserId);
            Assert.AreEqual("user2", user.Username);
            Assert.AreEqual("user2@example.com", user.Email);
        }

        [Test]
        public void GetUserById_WithNonExistentId_Returns_Null()
        {
            // Arrange
            var mockContext = new Mock<OtpsystemContext>();
            var mockTotp = new Mock<TOTP.TOTP>();
            var service = new Service(mockContext.Object, mockTotp.Object);

            var users = new[]
            {
                new User { UserId = 1, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1" },
                new User { UserId = 2, Username = "user2", Email = "user2@example.com", PasswordHash = "hash2" },
                new User { UserId = 3, Username = "user3", Email = "user3@example.com", PasswordHash = "hash3" }
            }.AsQueryable();
            mockContext.Setup(c => c.Users).Returns(MockDbSet(users));
            // Act
            // Assuming 100 is a non-existent user ID
            var user = service.GetUserById(100);

            // Assert
            Assert.IsNull(user);
        }
        public static DbSet<T> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet.Object;
        }
    }
}
