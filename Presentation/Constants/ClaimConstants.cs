namespace Data.Constants;

//with chat gpt help to feed the RoleClaimsIitializer
public class ClaimConstants
{
        //general claim
        public const string Permission = "Permission";
        public const string Role = "Role";
        public const string UserId = "UserId";
        public const string ClientId = "ClientId";
        public const string ProjectId = "ProjectId";


        //Users
        public const string CreateUser = "Permission.CreateUser";
        public const string EditUser = "Permission.EditUser";
        public const string DeleteUser = "Permission.DeleteUser";
        public const string ViewUser = "Permission.ViewUser";
        public const string ViewAllUsers = "Permission.ViewAllUsers";
        public const string ViewOwnProfile = "Permission.ViewOwnProfile";
        public const string EditOwnProfile = "Permission.EditOwnProfile";

        //Clients
        public const string CreateClient = "Permission.CreateClient";
        public const string EditClient = "Permission.EditClient";
        public const string DeleteClient = "Permission.DeleteClient";
        public const string ViewClient = "Permission.ViewClient";
        public const string ViewAllClients = "Permission.ViewAllClients";


        //Projects
        public const string CreateProject = "Permission.CreateProject";
        public const string EditProject = "Permission.EditProject";
        public const string DeleteProject = "Permission.DeleteProject";
        public const string ViewProject = "Permission.ViewProject";
        public const string ViewAllProjects = "Permission.ViewAllProjects";
        public const string AssignUserToProject = "Permission.AssignUserToProject";
        public const string RemoveUserFromProject = "Permission.RemoveUserFromProject";
        public const string AssignClientToProject = "Permission.AssignClientToProject";
        public const string RemoveClientFromProject = "Permission.RemoveClientFromProject";


}
