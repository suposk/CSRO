namespace CSRO.Server.Auth.Api.Dtos
{

    public class UserRoleDto
    {
        public int RoleId { get; set; }

        public RoleDto Role { get; set; }

        public int UserId { get; set; }

        public UserDto User { get; set; }
    }
}
