namespace AuthService.Application.Exceptions
{
    /// <summary>
    /// Thrown when a user tries to register with an existing username or email.
    /// </summary>
    public class DuplicateAccountException : Exception
    {
        public DuplicateAccountException()
            : base("Account already exists.") { }

        public DuplicateAccountException(string message)
            : base(message) { }
    }
}
