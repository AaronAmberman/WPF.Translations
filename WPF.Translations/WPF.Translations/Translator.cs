using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WPF.Translations
{
    /// <summary>A class that provides translations to a WPF application without the need to restart the application. This class cannot be inherited.</summary>
    public sealed class Translator<T> : ITranslationProvider<T>
    {
        #region Fields

        private List<Tuple<string, T, Translation>> translations = new List<Tuple<string, T, Translation>>();

        #endregion

        #region Properties

        /// <summary>Gets or sets the translations currently in use.</summary>
        public Translation CurrentTranslations { get; set; }

        /// <summary>Gets or sets the translation to use as the key enforcement contract.</summary>
        /// <remarks>
        /// <para>
        /// Key enforcement contract means that all incoming resource dictionaries must have the exact 
        /// same number of keys and all the keys must match or an error will be thrown.
        /// </para>
        /// <para>
        /// Before any resource dictionaries can be added to the translations collection this property must be set
        /// or an error will be thrown.
        /// </para>
        /// </remarks>
        public Translation KeyContract { get; set; }

        /// <summary>
        /// Gets or sets the translation data provider (the component that will read the resource and construct a dictionary of translations).
        /// </summary>
        public ITranslationDataProvider TranslationDataProvider { get; set; }

        /// <summary>Gets the collection of translations.</summary>
        public IReadOnlyDictionary<string, Translation> Translations { get; private set; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Translator"/> class.</summary>
        /// <remarks>
        /// 
        /// </remarks>
        public Translator()
        {
            Translations = new Dictionary<string, Translation>();
        }

        #endregion

        #region Methods

        /// <summary>Adds a resource dictionary to the list of translations.</summary>
        /// <param name="culture">The culture string to use as a key for the translation.</param>
        /// <param name="resource">The resource dictionary to add that contains our translated strings.</param>
        /// <exception cref="InvalidOperationException">The KeyContract must be set before adding any resource dictionaries.</exception>
        /// <exception cref="ArgumentException">Duplicate key.</exception>
        /// <exception cref="ArgumentNullException">resourceDictionary is null..</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: key count mistmatch.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: missing keys.</exception>
        public bool AddResourceForTranslation(string culture, T resource)
        {
            if (KeyContract == null)
                throw new InvalidOperationException("The KeyContract must be set before adding any resource dictionaries.");

            if (Translations.ContainsKey(culture))
                throw new ArgumentException($"Duplicate key. Culture: {culture}.");

            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            try
            {
                Translation translation = new Translation(resource, TranslationDataProvider);

                if (translation.Count != KeyContract.Count)
                    throw new ArgumentException($"ResourceDictionary does not match KeyContract: key count mistmatch. Culture: {culture}.");

                // look for strings that are that are not there that should be
                IReadOnlyList<string> keys = TranslationDataProvider.GetKeys(resource);

                List<string> missingKeys = KeyContract.Keys.Cast<string>().Where(x => keys.Cast<string>().All(y => x != y)).ToList();

                if (missingKeys.Count > 0)
                    throw new ArgumentException($"ResourceDictionary does not match KeyContract: missing keys. Culture: {culture}.");

                Dictionary<string, Translation> dictionary = Translations.ToDictionary(x => x.Key, x => x.Value);
                dictionary.Add(culture, translation);

                Translations = dictionary;
                
                translations.Add(new Tuple<string, T, Translation>(culture, resource, translation));

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred attempting to add the resource dictionary to the translator.{Environment.NewLine}{ex}");

                return false;
            }
        }

        /// <summary>Adds a collection of resource dictionaries to the list of translations.</summary>
        /// <param name="translations">The collections of translations to add.</param>
        /// <exception cref="InvalidOperationException">The KeyContract must be set before adding any resource dictionaries.</exception>
        /// <exception cref="ArgumentException">Duplicate key.</exception>
        /// <exception cref="ArgumentNullException">resourceDictionary is null..</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: key count mistmatch.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: missing keys.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: extra keys.</exception>
        public void AddResourcesForTranslation(IEnumerable<Tuple<string, T>> translations)
        {
            foreach (Tuple<string, T> translation in translations)
            {
                AddResourceForTranslation(translation.Item1, translation.Item2);
            }
        }

        /// <summary>Clears the current collection of translations.</summary>
        public void ClearTranslations()
        {
            CurrentTranslations = null;
            Translations = new Dictionary<string, Translation>();
            translations.Clear();
        }

        /// <summary>Releases resources used by the Translation objects.</summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, Translation> kvp in Translations)
            {
                kvp.Value.Dispose();
            }

            Translations = null;
            
            translations.Clear();
            translations = null;

            TranslationDataProvider = null;
        }

        #endregion
    }
}
