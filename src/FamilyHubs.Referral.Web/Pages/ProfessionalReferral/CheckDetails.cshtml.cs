using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral
{
    public class CheckDetailsModel : ProfessionalReferralSessionModel
    {
        public ConnectionRequestModel? ConnectionRequestModel { get; set; }

        public CheckDetailsModel(IConnectionRequestDistributedCache connectionRequestCache)
            : base(ConnectJourneyPage.CheckDetails, connectionRequestCache)
        {
        }

        protected override void OnGetWithModel(ConnectionRequestModel model)
        {
            ConnectionRequestModel = model;
        }

        protected override string? OnPostWithModel(ConnectionRequestModel model)
        {
            return "Confirmation";
        }
    }
}
