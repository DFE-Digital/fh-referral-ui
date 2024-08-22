using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.FullPages.Checkboxes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactDetailsModel : ProfessionalReferralCacheModel, ICheckboxesPageModel
{
    public string? FullName { get; set; }

    private bool[] SelectedContactMethodMapping { get; init; } = new bool[(int)ConnectContactDetailsJourneyPage.LastContactMethod + 1];

    public static readonly Checkbox[] StaticCheckboxes = new Checkbox[]
    {
        new("Email", "Email"),
        new("Telephone", "Telephone"),
        new("Text message", "Textphone"),
        new("Letter", "Letter")
    };

    public IEnumerable<ICheckbox> Checkboxes => StaticCheckboxes;

    [BindProperty]
    public IEnumerable<string> SelectedValues { get; set; } = Enumerable.Empty<string>();

    public string? DescriptionPartial => null;

    public string? Legend { get; private set; }

    public string? Hint => "Select all that apply.";

    public ContactDetailsModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.ContactDetails, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        FullName = model.FamilyContactFullName;

        bool[] contactMethods;

        if (HasErrors)
        {
            contactMethods = SelectedContactMethodMapping;
        }
        else
        {
            contactMethods = model.ContactMethodsSelected;

            List<string> selectedValues = new();

            for (int i = 0; i < contactMethods.Length; i++)
            {
                if (contactMethods[i])
                {
                    selectedValues.Add(StaticCheckboxes[i].Value);
                }
            }

            SelectedValues = selectedValues;
        }

        Legend = $"How can this service contact {FullName}?";

        //todo: move this and code from CheckDetails into one place

        // handle this edge case:
        // user reaches check details page, then clicks to change Contact methods, then selects a new contact method,
        // then clicks continue, then back to Contact methods page, then back to Check details page.
        // without this, they will have a contact method selected, but without the appropriate contact details.
        // with this, they won't have a back button and will be forced to re-enter contact details.
        if (Flow == JourneyFlow.ChangingContactMethods
            && ((contactMethods[(int)ConnectContactDetailsJourneyPage.Telephone] && model.TelephoneNumber == null)
            || (contactMethods[(int)ConnectContactDetailsJourneyPage.Textphone] && model.TextphoneNumber == null)
            || (contactMethods[(int)ConnectContactDetailsJourneyPage.Email] && model.EmailAddress == null)
            || (contactMethods[(int)ConnectContactDetailsJourneyPage.Letter] &&
                (model.AddressLine1 == null || model.TownOrCity == null || model.Postcode == null))))
        {
            BackUrl = null;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
#pragma warning disable S6605
        if (!ModelState.IsValid || SelectedValues.IsNullOrEmpty())
#pragma warning restore S6605
        {
            return RedirectToSelf(null, ErrorId.ContactDetails_NoContactMethodsSelected);
        }

        model.ContactMethodsSelected = SelectedContactMethodMapping;

        for (int i = 0; i < StaticCheckboxes.Length; i++)
        {
            model.ContactMethodsSelected[i] = SelectedValues.Any(selectedValue => selectedValue.Equals(StaticCheckboxes[i].Value));
        }

        return FirstContactMethodPage(model.ContactMethodsSelected);
    }
}
