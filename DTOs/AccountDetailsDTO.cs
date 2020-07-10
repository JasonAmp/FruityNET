using FruityNET.Entities;

namespace FruityNET.DTOs
{
    public class AccountDetailsDTO
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class VerifyIdentityDTO
    {
        public UserAccount ExistingAccount { get; set; }
        public string Password { get; set; }
    }
}