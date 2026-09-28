using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;

        public int ExpiretesHours { get; set; } = 12;    
    }
}
