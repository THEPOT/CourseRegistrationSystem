using CDQTSystem_API.Enums;
using Microsoft.AspNetCore.Authorization;
using CDQTSystem_API.Utils;
namespace CDQTSystem_API.Validators
{
	public class CustomAuthorizeAttribute : AuthorizeAttribute
	{
		public CustomAuthorizeAttribute(params RoleEnum[] roleEnums)
		{
			var allowedRolesAsString = roleEnums.Select(x => x.GetDescriptionFromEnum());
			Roles = string.Join(",", allowedRolesAsString);
		}
	}
}
