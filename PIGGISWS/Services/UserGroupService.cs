using Microsoft.Graph;
using PIGGISWS.Interfaces;

namespace PIGGISWS.Services;

public class UserGroupService : IUserGroupService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserGroupService> _logger;

    public UserGroupService(GraphServiceClient graphServiceClient, IConfiguration configuration, ILogger<UserGroupService> logger)
    {
        _graphServiceClient = graphServiceClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<string>> GetUserGroupsAsync(string userId)
    {
        var groupNames = new List<string>();

        try
        {
            // TRAE LOS GRUPOS A LOS QUE PERTENECE EL USUARIO
            var memberOf = await _graphServiceClient.Users[userId].MemberOf.Request().GetAsync();

            if (memberOf != null)
            {
                foreach (var item in memberOf)
                {
                    if (item is Microsoft.Graph.Group group)
                    {
                        groupNames.Add(group.DisplayName ?? string.Empty);
                    }
                }
            }
        }
        catch (ServiceException ex)
        {
            _logger.LogError($"Error obteniendo grupos del usuario {userId}: {ex.Message}");
        }

        return groupNames;
    }

    public async Task<string> GetUserRoleAsync(string userId)
    {
        try
        {
            var groups = await GetUserGroupsAsync(userId);
            var groupMapping = _configuration.GetSection("GroupMapping");

            if (groups.Contains("AGENTES_COMERCIALES"))
                return "AGENTES_COMERCIALES";

            if (groups.Contains("AGENTES_DESPACHOS"))
                return "AGENTES_DESPACHOS";

            return "SIN_ASIGNAR";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error obteniendo rol del usuario {userId}: {ex.Message}");
            return "ERROR";
        }
    }
}
