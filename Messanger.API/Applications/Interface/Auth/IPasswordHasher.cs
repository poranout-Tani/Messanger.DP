using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Interface.Auth
{
    public interface IPasswordHasher
    {
        string Generate(string password);

        bool Verify(string password, string hashedPassword);
    }
}
