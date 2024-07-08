namespace Tel.Egram.Services.Persistence.Resources;

public class PhoneCode
{
    public required string Code { get; set; }
    public required string CountryCode { get; set; }
    public required string CountryName { get; set; }
    public required string Mask { get; set; }
}