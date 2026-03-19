using EnterpriseOrderSystem.Application.Features.Auth.Commands;
using EnterpriseOrderSystem.Application.Features.Auth.DTOs;
using EnterpriseOrderSystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Application.Features.Auth.Handlers
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public LoginUserHandler(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

            Console.WriteLine(user.PasswordHash); // hashed
            Console.WriteLine(request.Password); // plain

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role.Name);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.Name
            };
        }
    }
}
