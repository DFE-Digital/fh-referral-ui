using FamilyHubs.ReferralUi.Ui.Services;

namespace FamilyHubs.ReferralUi.Ui.Models;

public interface IFooterViewModel : ILinkCollection, ILinkHelper
{
    bool UseLegacyStyles { get; }
}
