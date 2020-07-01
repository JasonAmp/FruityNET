namespace FruityNET.Exceptions
{
    public class ErrorMessages
    {
        public const string UserDoesNotExist = "User Does Not Exist.";
        public const string RequiredValuesNotProvided = "Required fields not provided";
        public const string InvalidInput = "Required fields have invalid input";
        public const string UserNotProvided = "Please provide a Username";
        public const string AccountSuspended = "Account is currently suspended.";
        public const string AccountInactive = "Account needs to be activated.";

        public const string PendingRequest = "There is already a pending invite.";
        public const string InvalidResetCredentials = "Email does not match account which is needed to reset password.";
        public const string passwordConfirmationFail = "Password confirmation failed.";

        public const string PostDoesNotExist = "Post does not exist.";



        public const string AdminExists = "User is an existing Admin.";




        #region Forbidden
        public const string ForbiddenAccess = "User does not have permission.";

        #endregion

        #region Login
        public const string InvalidLogin = "Invalid Login Attempt";
        public const string NotSignedIn = "No User is signed in.";

        #endregion


        #region Group
        public const string GroupDoesNotExist = "Group Does Not Exist.";
        public const string GroupUserExists = "User is already part of Group.";
        public const string RequestDoesNotExist = "Request Does Not Exist.";


        #endregion
    }
}