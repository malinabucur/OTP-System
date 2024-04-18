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
        // Base32 = GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ
        private const string Rfc6238SecretSha1 = "12345678901234567890";
        // Base32 = GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZA
        private const string Rfc6238SecretSha256 = "12345678901234567890123456789012";
        // Base32 = GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNA
        private const string Rfc6238SecretSha512 = "1234567890123456789012345678901234567890123456789012345678901234";

        [TestCase(Rfc6238SecretSha1, OtpHashMode.Sha1, 59, "94287082")]
        [TestCase(Rfc6238SecretSha256, OtpHashMode.Sha256, 59, "46119246")]
        [TestCase(Rfc6238SecretSha512, OtpHashMode.Sha512, 59, "90693936")]
        [TestCase(Rfc6238SecretSha1, OtpHashMode.Sha1, 1111111109, "07081804")]
        [TestCase(Rfc6238SecretSha256, OtpHashMode.Sha256, 1111111109, "68084774")]
        [TestCase(Rfc6238SecretSha512, OtpHashMode.Sha512, 1111111109, "25091201")]
        [TestCase(Rfc6238SecretSha1, OtpHashMode.Sha1, 1111111111, "14050471")]
        [TestCase(Rfc6238SecretSha256, OtpHashMode.Sha256, 1111111111, "67062674")]
        [TestCase(Rfc6238SecretSha512, OtpHashMode.Sha512, 1111111111, "99943326")]
        [TestCase(Rfc6238SecretSha1, OtpHashMode.Sha1, 1234567890, "89005924")]
        [TestCase(Rfc6238SecretSha256, OtpHashMode.Sha256, 1234567890, "91819424")]
        [TestCase(Rfc6238SecretSha512, OtpHashMode.Sha512, 1234567890, "93441116")]
        [TestCase(Rfc6238SecretSha1, OtpHashMode.Sha1, 2000000000, "69279037")]
        [TestCase(Rfc6238SecretSha256, OtpHashMode.Sha256, 2000000000, "90698825")]
        [TestCase(Rfc6238SecretSha512, OtpHashMode.Sha512, 2000000000, "38618901")]
        [TestCase(Rfc6238SecretSha1, OtpHashMode.Sha1, 20000000000, "65353130")]
        [TestCase(Rfc6238SecretSha256, OtpHashMode.Sha256, 20000000000, "77737706")]
        [TestCase(Rfc6238SecretSha512, OtpHashMode.Sha512, 20000000000, "47863826")]
        [TestCase(Rfc6238SecretSha1, OtpHashMode.Sha1, 20000000000, "353130")]
        [TestCase(Rfc6238SecretSha256, OtpHashMode.Sha256, 20000000000, "737706")]
        [TestCase(Rfc6238SecretSha512, OtpHashMode.Sha512, 20000000000, "863826")]
        public void ComputeTOTPTest(string secret, OtpHashMode hash, long timestamp, string expectedOtp)
        {
            var otpCalc = new Totp(Encoding.UTF8.GetBytes(secret), 30, hash, expectedOtp.Length);
            var time = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
            var otp = otpCalc.ComputeTotp(time);
            Assert.That(otp, Is.EqualTo(expectedOtp));
        }

        [Test]
        public void ContructorWithKeyProviderTest()
        {
            // Mock a key provider which always returns an all-zero HMAC (causing an all-zero OTP)
            var keyMock = new Mock<IKeyProvider>();
            keyMock.Setup(key => key.ComputeHmac(It.Is<OtpHashMode>(m => m == OtpHashMode.Sha1), It.IsAny<byte[]>())).Returns(new byte[20]);

            var otp = new Totp(keyMock.Object, 30, OtpHashMode.Sha1, 6);
            Assert.That(otp.ComputeTotp(), Is.EqualTo("000000"));
        }

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
                new User { UserId = userId, PasswordHash = QuickHash(otpCode) } // Return user with correct UserId and PasswordHash
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
                new User { UserId = userId, PasswordHash = "correctOTP" } // Return user with correct UserId and PasswordHash
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