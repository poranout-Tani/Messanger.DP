using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Contracts.Users
{
    public record UsersPasswordUpdate(
        string NewPassword
        );
}
