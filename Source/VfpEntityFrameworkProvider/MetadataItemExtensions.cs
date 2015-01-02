using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider {
    internal static class MetadataItemExtensions {
        internal static T TryGetValueForMetadataProperty<T>(this MetadataItem metadataItem, string propertyName) {
            MetadataProperty property;

            if (!metadataItem.MetadataProperties.TryGetValue(propertyName, true, out property)) {
                return default(T);
            }

            return (T)property.Value;
        }
    }
}