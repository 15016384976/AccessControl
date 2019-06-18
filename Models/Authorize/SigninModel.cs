namespace AccessControl.Models.Authorize
{
    public class SigninModel
    {
        public string Account { get; set; }

        public string Password { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(Account))
            {
                throw new ValidationException("account should not be null");
            }

            if (string.IsNullOrEmpty(Password))
            {
                throw new ValidationException("password should not be null");
            }

            return true;
        }
    }
}
