using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnterpriseOrderSystem.Application.Common.Exceptions;
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
            //  1. validate if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

            if (existingUser != null)
                throw new BadRequestException("Email already exists");

            //  2. hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            //  3. ambil role
            var role = await _context.Roles
                .FirstAsync(x => x.Name == "User", cancellationToken);

            //  4. create user
            var user = new User(request.Email, passwordHash, role.Id);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            //  5. generate token
            var token = _jwtService.GenerateToken(user.Id, user.Email, role.Name);

            //  6. generate refresh token
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken(
                user.Id,
                refreshToken,
                DateTime.UtcNow.AddDays(7)
            );

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            //  7. return response
            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = role.Name //  better daripada user.Role.Name
            };
        }
    }
}
