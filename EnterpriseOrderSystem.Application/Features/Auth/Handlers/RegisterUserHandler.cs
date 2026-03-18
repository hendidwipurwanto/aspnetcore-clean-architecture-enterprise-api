using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnterpriseOrderSystem.Application.Features.Auth.Commands;
using EnterpriseOrderSystem.Application.Features.Auth.DTOs;
using EnterpriseOrderSystem.Application.Interfaces;
using EnterpriseOrderSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOrderSystem.Application.Features.Auth.Handlers
{ 


    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly  ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public RegisterUserHandler(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // simple hash (nanti bisa upgrade)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var role = await _context.Roles.FirstAsync(x => x.Name == "User", cancellationToken);

            var user = new User(request.Email, passwordHash, role.Id);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtService.GenerateToken(user.Id, user.Email, role.Name);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = role.Name
            };
        }
    }
}
