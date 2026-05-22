using AJT.Models;
using AJT.Options;
using AJT.Services;
using FluentAssertions;

namespace AJT.Tests
{
    public class HashingServiceTests
    {
        private readonly HashingService _sut; // SUT = System Under Test

        public HashingServiceTests()
        {
            var options = Microsoft.Extensions.Options.Options.Create(new AJTOptions { Secret = "BardzoTajnyKluczSerwera1234567890" });
            _sut = new HashingService(options);
        }

        [Fact]
        public void VerifyHash_GivenTamperedToken_ShouldReturnFalse()
        {
            var token = new Token { Id = Guid.NewGuid(), UserRoles = "00" };
            var hashedToken = _sut.Hash(token);

            var parts = hashedToken.Split('.');
            var originalSignature = parts[1];

            var jsonPayload = _sut.DecodePayload(hashedToken);

            var tamperedJson = jsonPayload.Replace("\"00\"", "\"01\"");

            var tamperedBytes = System.Text.Encoding.UTF8.GetBytes(tamperedJson);
            var tamperedPayload64 = Convert.ToBase64String(tamperedBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            var tamperedToken = $"{tamperedPayload64}.{originalSignature}";

            var isValid = _sut.VerifiyHash(tamperedToken);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void VerifyHash_SignedWithDifferentKey_ShouldReturnFalse()
        {
            var attackerOptions = Microsoft.Extensions.Options.Options.Create(new AJTOptions { Secret = "ZupelnieInnyKluczAtakujacego0987654321" });
            var attackerSut = new HashingService(attackerOptions);

            var forgedToken = attackerSut.Hash(new Token { Id = Guid.NewGuid() });

            var isValid = _sut.VerifiyHash(forgedToken);

            isValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("TylkoJedenSegmentBezKropki")]
        [InlineData("Segment1.Segment2.Segment3.ZaDuzoKropek")]
        [InlineData(".TylkoPodpis")]
        [InlineData("TylkoLadunek.")]
        [InlineData("")]
        [InlineData("   ")]
        public void VerifyHash_GivenMalformedFormat_ShouldReturnFalse(string malformedInput)
        {
            var isValid = _sut.VerifiyHash(malformedInput);

            isValid.Should().BeFalse();
        }
    }
}