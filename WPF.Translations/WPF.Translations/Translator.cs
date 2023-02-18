using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace WPF.Translations
{
    /// <summary>A class that provides translations to a WPF application without the need to restart the application. This class cannot be inherited.</summary>
    public sealed class Translator : ITranslationProvider
    {
        #region Fields

        private List<Tuple<string, ResourceDictionary, Translation>> translations = new List<Tuple<string, ResourceDictionary, Translation>>();

        #endregion

        #region Properties

        /// <summary>Gets or sets the translations currently in use.</summary>
        public dynamic CurrentTranslations { get; set; }

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
        public ResourceDictionary KeyContract { get; set; }

        /// <summary>Gets the collection of translations.</summary>
        public IReadOnlyDictionary<string, Translation> Translations { get; private set; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Translator"/> class.</summary>
        public Translator()
        {
            Translations = new Dictionary<string, Translation>();
        }

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
        public bool AddResourceDictionaryForTranslation(string culture, ResourceDictionary resourceDictionary)
        {
            if (KeyContract == null)
                throw new InvalidOperationException("The KeyContract must be set before adding any resource dictionaries.");

            if (Translations.ContainsKey(culture))
                throw new ArgumentException($"Duplicate key. Culture: {culture}.");

            if (resourceDictionary == null)
                throw new ArgumentNullException(nameof(resourceDictionary));

            if (resourceDictionary.Count != KeyContract.Count)
                throw new ArgumentException($"ResourceDictionary does not match KeyContract: key count mistmatch. Culture: {culture}.");

            // look for strings that are that are not there that should be
            List<string> missingKeys = KeyContract.Keys.Cast<string>().Where(x => resourceDictionary.Keys.Cast<string>().All(y => x != y)).ToList();

            if (missingKeys.Count > 0)
                throw new ArgumentException($"ResourceDictionary does not match KeyContract: missing keys. Culture: {culture}.");

            // look for strings that are that are not there that should be
            List<string> extraKeys = resourceDictionary.Keys.Cast<string>().Where(x => KeyContract.Keys.Cast<string>().All(y => x != y)).ToList();

            if (extraKeys.Count > 0)
                throw new ArgumentException($"ResourceDictionary does not match KeyContract: extra keys. Culture: {culture}.");

            try
            {
                Translation translation = new Translation(resourceDictionary);

                Dictionary<string, Translation> dictionary = Translations.ToDictionary(x => x.Key, x => x.Value);
                dictionary.Add(culture, translation);

                Translations = dictionary;
                
                translations.Add(new Tuple<string, ResourceDictionary, Translation>(culture, resourceDictionary, translation));

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
        public void AddResourceDictionariesForTranslation(IEnumerable<Tuple<string, ResourceDictionary>> translations)
        {
            foreach (Tuple<string, ResourceDictionary> translation in translations)
            {
                AddResourceDictionaryForTranslation(translation.Item1, translation.Item2);
            }
        }

        /// <summary>Clears the current collection of translations.</summary>
        public void ClearTranslations()
        {
            CurrentTranslations = null;
            Translations = new Dictionary<string, Translation>();
            translations.Clear();
        }

        #endregion
    }
}
