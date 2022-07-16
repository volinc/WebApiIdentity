using MediatR;

namespace WebApiIdentity.Application.Token.Commands.SignOut;

public class SignOutCommandHandler : RequestHandler<SignOutCommand, Unit>
{
    protected override Unit Handle(SignOutCommand request)
    {
        return Unit.Value;
    }
}