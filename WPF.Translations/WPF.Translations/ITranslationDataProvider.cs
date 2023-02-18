using System.Collections.Generic;

namespace WPF.Translations
{
    /// <summary>Describes an object that is capable of building a key value pair collection to provide for translations.</summary>
    public interface ITranslationDataProvider
    {
        /// <summary>Gets the collection of keys in the resource.</summary>
        /// <param name="resource">The resource to read data from.</param>
        /// <returns>A read-only list of keys in the resource.</returns>
        IReadOnlyList<string> GetKeys(object resource);

        /// <summary>Reads the resource and builds a dictionary of translation key value pairs.</summary>
        /// <param name="resource">The resource to read data from.</param>
        /// <returns>A dictionary containing the translation data (key value pairs).</returns>
        IReadOnlyDictionary<string, string> ReadTranslationData(object resource);
    }
}
