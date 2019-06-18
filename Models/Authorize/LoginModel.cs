namespace AccessControl.Models.Authorize
{
    public class LoginModel
    {
        public string Account { get; set; }

        public string Password { get; set; }

        public bool Remember { get; set; }

        public string ReturnUrl { get; set; }
    }
}
