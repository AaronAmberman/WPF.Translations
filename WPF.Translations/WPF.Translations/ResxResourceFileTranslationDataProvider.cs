using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace WPF.Translations
{
    /// <summary>A translation provider that is capable of reading through RESX resource files for translation key value pairs.</summary>
    public class ResxResourceFileTranslationDataProvider : ITranslationDataProvider
    {
        /// <summary>Gets the collection of keys in the resource.</summary>
        /// <param name="resource">The resource to read data from.</param>
        /// <returns>A read-only list of keys in the resource.</returns>
        public IReadOnlyList<string> GetKeys(object resource)
        {
            ResourceReader rr = resource as ResourceReader;

            if (rr == null) return new List<string>();

            IDictionaryEnumerator dict = rr.GetEnumerator();

            List<string> keys = new List<string>();

            while (dict.MoveNext())
            {
                if (dict.Value.GetType() == typeof(string))
                {
                    string key = dict.Key?.ToString() ?? string.Empty;

                    keys.Add(key);
                }
            }

            return keys;
        }

        /// <summary>Reads the resource of type T and builds a dictionary of translation key value pairs.</summary>
        /// <param name="resource">The resource to read data from.</param>
        /// <returns>A dictionary containing the translation data (key value pairs).</returns>
        public IReadOnlyDictionary<string, string> ReadTranslationData(object resource)
        {
            ResourceReader rr = resource as ResourceReader;

            if (rr == null) return new Dictionary<string, string>();

            IDictionaryEnumerator dict = rr.GetEnumerator();

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            while (dict.MoveNext())
            {
                if (dict.Value?.GetType() == typeof(string))
                {
                    string key = dict.Key?.ToString() ?? string.Empty;
                    string value = dict.Value?.ToString() ?? string.Empty;
                    
                    keyValuePairs.Add(key, value);
                }
            }

            return keyValuePairs;
        }
    }
}
