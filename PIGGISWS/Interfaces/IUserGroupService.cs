namespace PIGGISWS.Interfaces;

public interface IUserGroupService
{
    Task<List<string>> GetUserGroupsAsync(string userId);
    Task<string> GetUserRoleAsync(string userId);
}
