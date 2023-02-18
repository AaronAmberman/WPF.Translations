using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WPF.Translations
{
    /// <summary>A translation provider that is capable of reading through resource dictionaries for translation key value pairs.</summary>
    public class ResourceDictionaryTranslationDataProvider : ITranslationDataProvider
    {
        /// <summary>Gets the collection of keys in the resource.</summary>
        /// <param name="resource">The resource to read data from.</param>
        /// <returns>A read-only list of keys in the resource.</returns>
        public IReadOnlyList<string> GetKeys(object resource)
        {
            ResourceDictionary res = resource as ResourceDictionary;

            if (res == null) return new List<string>();

            return res.Keys.Cast<string>().ToList();
        }

        /// <summary>Reads the resource of type T and builds a dictionary of translation key value pairs.</summary>
        /// <param name="resource">The resource to read data from.</param>
        /// <returns>A dictionary containing the translation data (key value pairs).</returns>
        public IReadOnlyDictionary<string, string> ReadTranslationData(object resource)
        {
            ResourceDictionary res = resource as ResourceDictionary;

            if (res == null) return new Dictionary<string, string>();

            Dictionary<string, string> translations = new Dictionary<string, string>();

            foreach (string key in res.Keys)
            {
                translations[key] = res[key].ToString();
            }

            return translations;
        }
    }
}
