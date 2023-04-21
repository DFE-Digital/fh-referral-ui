using System.Text.Json;
using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ReferralCacheKeys : IReferralCacheKeys
{
    private readonly string _sessionId;

    public ReferralCacheKeys(IHttpContextAccessor httpContextAccessor)
    {
        _sessionId = httpContextAccessor.HttpContext!.Session.Id;
    }

    public string ProfessionalReferral => SessionNamespaced("PR");

    private string SessionNamespaced(string key)
    {
        return $"{_sessionId}{key}";
    }
}

public interface IReferralCacheKeys
{
    string ProfessionalReferral { get; }
}

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetAsync<T>(
        this IDistributedCache cache,
        string key,
        CancellationToken token = default)
    {
        var json = await cache.GetStringAsync(key, token);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public static async Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions? options = null,
        CancellationToken token = default)
    {
        var json = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, json, options ?? new DistributedCacheEntryOptions(), token);
    }
}

public class SupportDetailsModel : PageModel
{
    private readonly IDistributedCache _distributedCache;
    private readonly IReferralCacheKeys _referralCacheKeys;

    public string ServiceId { get; private set; } = default!;
    public string ServiceName { get; private set; } = default!;

    //todo: separate static with changing
    public PartialTextBoxViewModel PartialTextBoxViewModel { get; } = new()
    {
        ErrorId = "error-summary-title",
        HeadingText = "Who should the service contact in the family?",
        HintText = "This must be a person aged 16 or over.",
        TextBoxLabel = "Full name",
        MainErrorText = "Enter a full name",
        TextBoxErrorText = "Enter a full name",
    };

    [BindProperty]
    public string TextBoxValue { get; set; } = string.Empty;

    public SupportDetailsModel(IDistributedCache distributedCache, IReferralCacheKeys referralCacheKeys)
    {
        _distributedCache = distributedCache;
        _referralCacheKeys = referralCacheKeys;
    }

    public async Task OnGetAsync(string serviceId, string serviceName)
    {
        //todo:
        //Fixes Session Changing between requests 
        HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

        ServiceId = serviceId;
        ServiceName = serviceName;

        var model = await _distributedCache.GetAsync<ProfessionalReferralModel>(_referralCacheKeys.ProfessionalReferral);

        if (!string.IsNullOrEmpty(model?.FullName))
        {
            //todo: two?
            PartialTextBoxViewModel.TextBoxValue = model.FullName;
            TextBoxValue = model.FullName;
        }
    }

    public async Task<IActionResult> OnPostAsync(string serviceId, string serviceName)
    {
        if (!ModelState.IsValid)
        {
            PartialTextBoxViewModel.TextBoxValue = TextBoxValue;
            if (string.IsNullOrWhiteSpace(TextBoxValue))
                PartialTextBoxViewModel.ValidationValid = false;

            return Page();
        }

        if (TextBoxValue.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(255) ?? string.Empty;
        }

        var model = await _distributedCache.GetAsync<ProfessionalReferralModel>(_referralCacheKeys.ProfessionalReferral)
                    ?? new ProfessionalReferralModel
                    {
                        ServiceId = serviceId,
                        ServiceName = serviceName
                    };

        model.FullName = TextBoxValue;
        //todo: options. do we need to set any? use a static
        //todo: safe to use fire and forget?
        await _distributedCache.SetAsync(_referralCacheKeys.ProfessionalReferral, model);

        return RedirectToPage("/ProfessionalReferral/WhySupport");
    }
}
