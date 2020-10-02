using Microsoft.Extensions.Localization;

namespace Base64Convertor.Localization.Json
{
    public class JsonLocalizationOptions : LocalizationOptions
    {
        public ResourcesType ResourcesType { get; set; } = ResourcesType.TypeBased;
    }
}