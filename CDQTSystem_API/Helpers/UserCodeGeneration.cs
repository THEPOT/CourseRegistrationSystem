namespace CDQTSystem_API.Helpers
{
	public class UserCodeGeneration
	{
		public static string GenerateStudentId()
		{
			// Simple example - you might want a more sophisticated approach
			return "STU" + DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Substring(0, 4);
		}

		public static string GenerateStaffId()
		{
			// Simple example - you might want a more sophisticated approach
			return "STA" + DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Substring(0, 4);
		}
		public static string GenerateProfessorId()
		{
			// Simple example - you might want a more sophisticated approach
			return "PRO" + DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Substring(0, 4);
		}
		public static string GenerateAdminId()
		{
			// Simple example - you might want a more sophisticated approach
			return "ADM" + DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Substring(0, 4);
		}

	}
}
