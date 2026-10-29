using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;
using Vettingo.AuthService.Application.Validations;

namespace Vettingo.AuthService.UnitTests;

public sealed class RegistrationValidatorTests
{
    [Theory]
    [InlineData(99999, false)]
    [InlineData(100000, true)]
    [InlineData(999999, true)]
    [InlineData(1000000, false)]
    [InlineData(0, false)]
    [InlineData(-123456, false)]
    public void Register_And_TwoFactor_Codes_Should_Have_Exactly_Six_Digits(int code, bool valid)
    {
        new RegisterCommandRequestValidator().Validate(new RegisterCommandRequest { Email = "user@example.com", Code = code })
            .IsValid.Should().Be(valid);
        new TwoFactorLoginCommandRequestValidator().Validate(new TwoFactorLoginCommandRequest { Email = "user@example.com", Code = code })
            .IsValid.Should().Be(valid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData(null)]
    public void All_Registration_And_TwoFactor_Requests_Should_Validate_Email(string? email)
    {
        new RegisterCommandRequestValidator().Validate(new RegisterCommandRequest { Email = email!, Code = 123456 })
            .IsValid.Should().BeFalse();
        new TwoFactorLoginCommandRequestValidator().Validate(new TwoFactorLoginCommandRequest { Email = email!, Code = 123456 })
            .IsValid.Should().BeFalse();
        new TempRegisterCommandRequestValidator(Options.Create(new IdentityOptions())).Validate(
            new TempRegisterCommandRequest { Email = email!, Password = "Strong1!", Role = "Candidate" }).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("Aa1!a", false)]
    [InlineData("Aa1!aa", true)]
    [InlineData("strong1!", false)]
    [InlineData("STRONG1!", false)]
    [InlineData("Strong!!", false)]
    [InlineData("Strong12", false)]
    [InlineData("Strong١!", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void TempRegister_Should_Enforce_Identity_Password_Requirements(string? password, bool valid)
    {
        var validator = new TempRegisterCommandRequestValidator(Options.Create(new IdentityOptions()));
        validator.Validate(new TempRegisterCommandRequest { Email = "user@example.com", Password = password!, Role = "Candidate" })
            .IsValid.Should().Be(valid);
    }
}
