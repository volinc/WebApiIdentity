using FluentValidation;

namespace WebApiIdentity.Application.Token.Commands.SignOut;

public class SignOutCommandValidator : AbstractValidator<SignOutCommand>
{
    public SignOutCommandValidator()
    {
    }
}