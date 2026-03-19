using EnterpriseOrderSystem.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Application.Features.Auth.Commands
{
    public record LoginUserCommand(
      string Email,
      string Password
  ) : IRequest<AuthResponseDto>;
}
