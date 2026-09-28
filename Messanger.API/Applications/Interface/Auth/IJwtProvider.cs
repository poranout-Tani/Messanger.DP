using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Interface.Auth
{
    public interface IJwtProvider
    {
        string JWTGenerate(Users user);
    }
}
