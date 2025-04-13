namespace CDQTSystem_API.Constants
{
	public class MessageConstant
	{
		public static class LoginMessage
		{
			public const string InvalidUsernameOrPassword = "Tên đăng nhập hoặc mật khẩu không chính xác";
			public const string DeactivatedAccount = "Tài khoản đang bị vô hiệu hoá";
		}

		public static class RegisterMessage
		{
			public const string RoleNotFound = "Role không tồn tại";
			public const string EmailExisted = "Email đã tồn tại";
			public const string RegisterFailed = "Đăng ký thất bại";
			public const string RegisterSuccess = "Đăng ký thành công";
		}
		public static class RefreshTokenMessage
		{
			public const string RefreshTokenFailed = "Lấy lại token thất bại";
		}
	}
}
