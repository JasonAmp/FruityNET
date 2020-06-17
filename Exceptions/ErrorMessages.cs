namespace FruityNET.Exceptions
{
    public class ErrorMessages
    {
        public const string UserDoesNotExist = "User Does Not Exist.";
        public const string RequiredValuesNotProvided = "Required fields not provided";



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

        #endregion
    }
}