using CourseRegistration_API.Enums;
using Microsoft.AspNetCore.Authorization;
using CourseRegistration_API.Utils;
namespace CourseRegistration_API.Validators
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
