using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Base64Convertor.Localization.Json.Internal;

namespace Base64Convertor.Localization.Json
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly string _resourcesRelativePath;
        private readonly ResourcesType _resourcesType = ResourcesType.TypeBased;
        private readonly ILoggerFactory _loggerFactory;

        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> localizationOptions,
            ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(localizationOptions);

            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? string.Empty;
            _resourcesType = localizationOptions.Value.ResourcesType;
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            ArgumentNullException.ThrowIfNull(resourceSource);

            var typeInfo = resourceSource.GetTypeInfo();
            var assembly = typeInfo.Assembly;
            var resourcesPath = Path.Combine(PathHelpers.GetApplicationRoot(), GetResourcePath(assembly));

            return CreateJsonStringLocalizer(resourcesPath, typeInfo.Name);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            ArgumentNullException.ThrowIfNull(baseName);
            ArgumentNullException.ThrowIfNull(location);

            var assemblyName = new AssemblyName(location);
            var assembly = Assembly.Load(assemblyName);
            var resourcesPath = Path.Combine(PathHelpers.GetApplicationRoot(), GetResourcePath(assembly));
            string resourceName = null;

            if (_resourcesType == ResourcesType.TypeBased)
            {
                resourceName = TrimPrefix(baseName, location + ".");
            }

            return CreateJsonStringLocalizer(resourcesPath, resourceName);
        }

        protected virtual JsonStringLocalizer CreateJsonStringLocalizer(
            string resourcesPath,
            string resourcename)
        {
            var logger = _loggerFactory.CreateLogger<JsonStringLocalizer>();

            return _resourcesType == ResourcesType.TypeBased
                ? new JsonStringLocalizer(
                    resourcesPath,
                    resourcename,
                    logger)
                : new JsonStringLocalizer(
                    resourcesPath,
                    logger);
        }

        private string GetResourcePath(Assembly assembly)
        {
            var resourceLocationAttribute = assembly.GetCustomAttribute<ResourceLocationAttribute>();

            return resourceLocationAttribute == null
                ? _resourcesRelativePath
                : resourceLocationAttribute.ResourceLocation;
        }

        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name[prefix.Length..];
            }

            return name;
        }
    }
}