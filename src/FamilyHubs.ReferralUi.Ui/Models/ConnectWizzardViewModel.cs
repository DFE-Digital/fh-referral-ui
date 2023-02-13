using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Models;

internal class ConnectWizzardViewModel
{
    public string ServiceId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ReferralId { get; set; } = string.Empty;
    public bool AnyoneInFamilyBeingHarmed { get; set; } = false;
    public bool HaveConcent { get; set; } = false;
    public string FullName { get; set; } = string.Empty; //Name of Family Contact
    public string? EmailAddress { get; set; }
    public string? Telephone { get; set; }
    public string? Textphone { get; set; }
    public string ReasonForSupport { get; set; } = string.Empty;

    public string Encode(byte xorConstant = 0x62)
    {
        var jsonModel = JsonSerializer.Serialize(this);

        byte[] data = Encoding.UTF8.GetBytes(jsonModel);
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(data[i] ^ xorConstant);
        }
        return Convert.ToBase64String(data);
    }

    public static ConnectWizzardViewModel? Decode(string input, byte xorConstant = 0x62)
    {
        byte[] data = Convert.FromBase64String(input);
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(data[i] ^ xorConstant);
        }
        string plainText = Encoding.UTF8.GetString(data);

        ConnectWizzardViewModel? model =
            JsonSerializer.Deserialize<ConnectWizzardViewModel>(plainText);

        return model;
    }
}

