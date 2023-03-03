using FamilyHubs.ServiceDirectory.Shared.Dto;

namespace FamilyHubs.ReferralUi.Ui.Models;

public static class ReferralDtoViewModel
{
    public static string GetDateRecieved(this ReferralDto referralDto)
    {
        if (referralDto.DateRecieved != null)
            return referralDto.DateRecieved.Value.ToString("yyyyMMdd");

        return string.Empty;
    }

    public static string GetStatus(this ReferralDto referralDto)
    {
        switch(referralDto.Status.LastOrDefault()?.Status)
        {
            case "Accept Connection":
                return "Accepted";
                               
            case "Reject Connection":
                return "Declined";
                                
            case "Connection Made":
                return "Opened";
                            
            default:
                return "New";
       }

    }
}
