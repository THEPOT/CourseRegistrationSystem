namespace CourseRegistration_API.Payload.Response
{
    public class RefreshTokenResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}