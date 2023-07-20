using FamilyHubs.Referral.Core.Models;

namespace FamilyHubs.Referral.Core.DistributedCache;

public interface IConnectionRequestDistributedCache
{
    Task<ConnectionRequestModel?> GetAsync(string professionalsEmail);
    Task SetAsync(string professionalsEmail, ConnectionRequestModel model);
    Task RemoveAsync(string professionalsEmail);
}