using EnterpriseOrderSystem.Application.Common.Exceptions;
using EnterpriseOrderSystem.Application.Features.Auth.Commands;
using EnterpriseOrderSystem.Application.Features.Auth.DTOs;
using EnterpriseOrderSystem.Application.Interfaces;
using EnterpriseOrderSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Application.Features.Auth.Handlers
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public RefreshTokenHandler(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Ambil refresh token dari DB
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

            // 2. Validasi token
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
                throw new BadRequestException("Invalid refresh token");

            // 3. Ambil user
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == storedToken.UserId, cancellationToken);

            if (user == null)
                throw new BadRequestException("User not found");

            // 4. Generate JWT baru
            var newJwt = _jwtService.GenerateToken(user.Id, user.Email, user.Role.Name);

            // (Optional tapi recommended) ROTATE refresh token
            storedToken.Revoke();

            var newRefreshToken = _jwtService.GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken(
                user.Id,
                newRefreshToken,
                DateTime.UtcNow.AddDays(7)
            );

            _context.RefreshTokens.Add(newRefreshTokenEntity);

            await _context.SaveChangesAsync(cancellationToken);

            // 5. Return token baru
            return new AuthResponseDto
            {
                Token = newJwt,
                RefreshToken = newRefreshToken,
                Email = user.Email,
                Role = user.Role.Name
            };
        }
    }
}
