using CourseRegistration_API.Enums;
using CourseRegistration_Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseRegistration_API.Utils
{
	public class JwtUtil
	{
		private static IConfiguration _configuration;

		public static void Initialize(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public static string GenerateJwtToken(User account, Tuple<string, Guid> guidClaim)
		{
			var secretKey = "YourStrongSecretKeyThatIsAtLeast32CharactersLong";
			var issuer = "CourseRegistrationSystem";
			var audience = "CourseRegistrationClients";

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
	{
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		new Claim(JwtRegisteredClaimNames.Sub, account.FullName),
		new Claim(ClaimTypes.Role, account.Role.RoleName)
	};

			if (guidClaim != null)
			{
				claims.Add(new Claim(guidClaim.Item1, guidClaim.Item2.ToString()));
			}

			var now = DateTime.UtcNow;
			var expires = account.Role.RoleName.Equals(RoleEnum.Staff.GetDescriptionFromEnum(), StringComparison.OrdinalIgnoreCase)
				? now.AddDays(15)
				: now.AddDays(30);

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				notBefore: now,
				expires: expires,
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static string GenerateRefreshToken(User account, Tuple<string, Guid> guidClaim)
		{

			var secret = "YourLongerSecretKeyHereWithAtLeast32Characters";
			var issuer = "UniversitySystem";
			var audience = "UniversitySystemRefreshToken";

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
	{
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		new Claim(JwtRegisteredClaimNames.Sub, account.FullName),
		new Claim(ClaimTypes.Role, account.Role.RoleName)
	};

			if (guidClaim != null)
			{
				claims.Add(new Claim(guidClaim.Item1, guidClaim.Item2.ToString()));
			}

			var now = DateTime.UtcNow;
			var expires = account.Role.RoleName.Equals(RoleEnum.Staff.GetDescriptionFromEnum(), StringComparison.OrdinalIgnoreCase)
				? now.AddDays(15)
				: now.AddDays(30);

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				notBefore: now,
				expires: expires,
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
