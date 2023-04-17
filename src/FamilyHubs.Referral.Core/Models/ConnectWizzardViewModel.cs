using System.Text;
using System.Text.Json;

namespace FamilyHubs.Referral.Core.Models;

public class ConnectWizzardViewModel
{
    public string ServiceId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty; //Name of the contact
    public string ReasonForSupport { get; set; } = default!;

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
