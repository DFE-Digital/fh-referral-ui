using System.Security.Principal;

namespace FamilyHubs.ReferralUi.Ui.Models;

public interface IUserContext
{
    string HashedAccountId { get; set; }
    IPrincipal User { get; set; }
}
