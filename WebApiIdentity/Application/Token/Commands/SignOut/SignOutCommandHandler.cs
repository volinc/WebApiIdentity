using MediatR;

namespace WebApiIdentity.Application.Token.Commands.SignOut;

public class SignOutCommandHandler : IRequestHandler<SignOutCommand, Unit>
{
    public Task<Unit> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Value);
    }
}