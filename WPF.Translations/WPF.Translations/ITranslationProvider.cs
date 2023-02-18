using System;
using System.Collections.Generic;

namespace WPF.Translations
{
    /// <summary>Describes an object that will provide and manage translations objects.</summary>
    public interface ITranslationProvider<T>
    {
        #region Properties

        /// <summary>Gets or sets the translations currently in use.</summary>
        Translation CurrentTranslations { get; set; }

        /// <summary>Gets or sets the resource dictionary to use as the key enforcement.</summary>
        /// <remarks>
        /// <para>
        /// Key enforcement means that all incoming resource dictionaries must have the exact same number of keys 
        /// and all the keys must match or an error will be thrown.
        /// </para>
        /// <para>
        /// Before any resource dictionaries can be added to the translations collection this property must be set
        /// or an error will be thrown.
        /// </para>
        /// </remarks>
        Translation KeyContract { get; set; }

        /// <summary>
        /// Gets or sets the translation data provider (the component that will read the resource and construct a dictionary of translations).
        /// </summary>
        ITranslationDataProvider TranslationDataProvider { get; set; }

        /// <summary>Gets the collection of translations.</summary>
        IReadOnlyDictionary<string, Translation> Translations { get; }

        #endregion

        #region Methods

        /// <summary>Adds a resource dictionary to the list of translations.</summary>
        /// <param name="culture">The culture string to use as a key for the translation.</param>
        /// <param name="resourceDictionary">The resource dictionary to add that contains our translated strings.</param>
        /// <exception cref="InvalidOperationException">The KeyContract must be set before adding any resource dictionaries.</exception>
        /// <exception cref="ArgumentException">Duplicate key.</exception>
        /// <exception cref="ArgumentNullException">resourceDictionary is null..</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: key count mistmatch.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: missing keys.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: extra keys.</exception>
        bool AddResourceDictionaryForTranslation(string culture, T resourceDictionary);

        /// <summary>Adds a collection of resource dictionaries to the list of translations.</summary>
        /// <param name="translations">The collections of translations to add.</param>
        /// <exception cref="InvalidOperationException">The KeyContract must be set before adding any resource dictionaries.</exception>
        /// <exception cref="ArgumentException">Duplicate key.</exception>
        /// <exception cref="ArgumentNullException">resourceDictionary is null..</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: key count mistmatch.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: missing keys.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: extra keys.</exception>
        void AddResourceDictionariesForTranslation(IEnumerable<Tuple<string, T>> translations);

        /// <summary>Clears the current collection of translations.</summary>
        void ClearTranslations();

        #endregion
    }
}
