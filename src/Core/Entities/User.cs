using Core.Enums;

namespace Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password_hash { get; set; }
    public UserRoles Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime Created_At { get; set; }
    public DateTime Updated_At { get; set; }
}
