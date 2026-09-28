namespace Core.Contracts.DTO_s
{
    public class UsersUpdatePassword
    {   
            public string OldPassword { get; set; } = string.Empty;

            public string NewPassword { get; set; } = string.Empty;       
    }
}
