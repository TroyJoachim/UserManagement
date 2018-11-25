# UserManagement
This is an ASP.NET Core Razor Pages Website. This site uses Identity core for user authentication and authorization. Identity has been extended to allow for user management by an administrator.

Claims are used along with policies to allow page level authorization to the user. Users can be assigned roles, and roles can be assigned RoleClaims. Users who are in a roles inherit the RoleClaims for that role. 
