using System.Text.Json;
using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

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

    public SupportDetailsModel(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task OnGetAsync(string serviceId, string serviceName)
    {
        //todo:
        //Fixes Session Changing between requests 
        HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

        ServiceId = serviceId;
        ServiceName = serviceName;

        //todo: naming
        var model = await _distributedCache.GetAsync<ConnectWizzardViewModel>(TempStorageConfiguration.KeyConnectWizzardViewModel);
        //if (model == null)
        //{
        //    model = new ConnectWizzardViewModel();
        //}
        //model.ServiceId = serviceId;
        //model.ServiceName = serviceName;
        //_distributedCache.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

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
            if (string.IsNullOrWhiteSpace(TextBoxValue?.Trim()))
                PartialTextBoxViewModel.ValidationValid = false;

            return Page();
        }

        if (TextBoxValue.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(252) ?? string.Empty;
        }

        var model = await _distributedCache.GetAsync<ConnectWizzardViewModel>(TempStorageConfiguration.KeyConnectWizzardViewModel)
                    ?? new ConnectWizzardViewModel
                    {
                        ServiceId = serviceId,
                        ServiceName = serviceName
                    };

        model.FullName = TextBoxValue;
        //todo: options. do we need to set any? use a static
        //todo: safe to use fire and forget?
        await _distributedCache.SetAsync(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        return RedirectToPage("/ProfessionalReferral/WhySupport");
    }
}
