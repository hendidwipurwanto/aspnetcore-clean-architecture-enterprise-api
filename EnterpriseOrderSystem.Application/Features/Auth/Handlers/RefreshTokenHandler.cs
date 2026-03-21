using EnterpriseOrderSystem.Application.Common.Exceptions;
using EnterpriseOrderSystem.Application.Features.Auth.Commands;
using EnterpriseOrderSystem.Application.Features.Auth.DTOs;
using EnterpriseOrderSystem.Application.Interfaces;
using EnterpriseOrderSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOrderSystem.Application.Features.Auth.Handlers
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        // ✅ FIX: pakai interface, bukan ApplicationDbContext
        public RefreshTokenHandler(IApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Get refresh token from DB
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

            // 2. Token not found
            if (storedToken == null)
                throw new BadRequestException("Invalid refresh token");

            // 3. REUSE DETECTION
            if (storedToken.IsUsed || storedToken.IsRevoked)
            {
                var userTokens = await _context.RefreshTokens
                    .Where(x => x.UserId == storedToken.UserId && !x.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var token in userTokens)
                {
                    token.Revoke();
                }

                await _context.SaveChangesAsync(cancellationToken);

                throw new BadRequestException("Refresh token reuse detected. Please login again.");
            }

            // 4. Expired check
            if (storedToken.ExpiryDate < DateTime.UtcNow)
                throw new BadRequestException("Refresh token expired");

            // 5. Get user
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == storedToken.UserId, cancellationToken);

            if (user == null)
                throw new BadRequestException("User not found");

            // 6. Generate new JWT
            var newJwt = _jwtService.GenerateToken(user.Id, user.Email, user.Role.Name);

            // 7. ROTATION
            storedToken.MarkAsUsed();
            storedToken.Revoke();

            // 8. Generate new refresh token
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken(
                user.Id,
                newRefreshToken,
                DateTime.UtcNow.AddDays(7)
            );

            _context.RefreshTokens.Add(newRefreshTokenEntity);

            await _context.SaveChangesAsync(cancellationToken);

            // 9. Return response
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