namespace CDQTSystem_API.Helpers
{
	public class MSSVGeneration
	{
		public static string GenerateStudentId()
		{
			// Simple example - you might want a more sophisticated approach
			return "STU" + DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Substring(0, 4);
		}
	}
}
