using Microsoft.AspNetCore.Identity;

namespace EfIdentity5793Repro
{
    public class Role : IdentityRole<int>
    {

    }

    public class RoleClaim : IdentityRoleClaim<int>
    {

    }

    public class User : IdentityUser<int>
    {

    }

    public class UserClaim : IdentityUserClaim<int>
    {

    }

    public class UserRole : IdentityUserRole<int>
    {

    }


}
