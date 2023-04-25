using FamilyHubs.Referral.Core.Models;

namespace FamilyHubs.Referral.Core.DistributedCache;

public interface IConnectionRequestDistributedCache
{
    Task<ConnectionRequestModel?> GetAsync();
    Task SetAsync(ConnectionRequestModel model);
    Task RemoveAsync();
}