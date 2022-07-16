using Authentication;
using FluentValidation;

namespace WebApiIdentity.Application.Token.Commands.SignIn;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        //RuleFor(command => command.ClientIp)
        //    .Must(stringValue => IPAddress.TryParse(stringValue, out _))
        //    .When(x => x.ClientIp != null);

        RuleFor(command => command.GrantType)
            .NotEmpty()
            .Must(value => value is GrantTypes.Password or GrantTypes.RefreshToken);

        RuleFor(command => command.Username)
            .NotEmpty()
            .When(command => command.GrantType == GrantTypes.Password);

        RuleFor(command => command.Password)
            .NotEmpty()
            .Length(Limits.Password.MinLength, Limits.Password.MaxLength)
            .When(command => command.GrantType == GrantTypes.Password);

        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .When(command => command.GrantType == GrantTypes.RefreshToken);
    }
}