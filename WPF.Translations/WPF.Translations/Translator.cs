using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WPF.Translations
{
    /// <summary>A class that provides translations to a WPF application without the need to restart the application. This class cannot be inherited.</summary>
    public sealed class Translator<T> : ITranslationProvider<T>
    {
        #region Fields

        private bool disposedValue;
        private List<Tuple<string, T, Translation>> translationCollection = new List<Tuple<string, T, Translation>>();
        private dynamic currentTranslations;
        private Translation keyContract;
        private ITranslationDataProvider translationDataProvider;
        private IReadOnlyDictionary<string, Translation> translations;

        #endregion

        #region Properties

        /// <summary>Gets or sets the translations currently in use.</summary>
        public dynamic CurrentTranslations
        {
            get
            {
                VerifyDisposed();

                return currentTranslations;
            }
            set
            {
                VerifyDisposed();

                currentTranslations = value;
            }
        }

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
        public Translation KeyContract
        {
            get
            {
                VerifyDisposed();

                return keyContract;
            }
            set
            {
                VerifyDisposed();

                keyContract = value;
            }
        }

        /// <summary>
        /// Gets or sets the translation data provider (the component that will read the resource and construct a dictionary of translations).
        /// </summary>
        public ITranslationDataProvider TranslationDataProvider
        {
            get
            {
                VerifyDisposed();

                return translationDataProvider;
            }
            set
            {
                VerifyDisposed();

                translationDataProvider = value;
            }
        }

        /// <summary>Gets the collection of translations.</summary>
        public IReadOnlyDictionary<string, Translation> Translations
        {
            get
            {
                VerifyDisposed();

                return translations;
            }
            private set
            {
                VerifyDisposed();

                translations = value;
            }
        }

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
            VerifyDisposed();

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

                translationCollection.Add(new Tuple<string, T, Translation>(culture, resource, translation));

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
            VerifyDisposed();

            foreach (Tuple<string, T> translation in translations)
                AddResourceForTranslation(translation.Item1, translation.Item2);
        }

        /// <summary>Adds a collection of resource dictionaries to the list of translations.</summary>
        /// <param name="translations">The dictionary of translations to add.</param>
        /// <exception cref="InvalidOperationException">The KeyContract must be set before adding any resource dictionaries.</exception>
        /// <exception cref="ArgumentException">Duplicate key.</exception>
        /// <exception cref="ArgumentNullException">resourceDictionary is null..</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: key count mistmatch.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: missing keys.</exception>
        /// <exception cref="ArgumentException">ResourceDictionary does not match KeyContract: extra keys.</exception>
        public void AddResourcesForTranslation(IDictionary<string, T> translations)
        {
            VerifyDisposed();

            foreach (KeyValuePair<string, T> translation in translations)
                AddResourceForTranslation(translation.Key, translation.Value);
        }

        /// <summary>Clears the current collection of translations.</summary>
        public void ClearTranslations()
        {
            VerifyDisposed();

            CurrentTranslations = null;
            Translations = new Dictionary<string, Translation>();
            translationCollection.Clear();
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (KeyValuePair<string, Translation> kvp in Translations)
                    {
                        kvp.Value.Dispose();
                    }

                    CurrentTranslations = null;
                    Translations = null;

                    translationCollection.Clear();
                    translationCollection = null;

                    TranslationDataProvider = null;
                }

                disposedValue = true;
            }
        }

        /// <summary>Releases resources used by the Translator object.</summary>
        public void Dispose()
        {
            VerifyDisposed();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void VerifyDisposed([CallerMemberName] string caller = "")
        {
            if (disposedValue)
                throw new ObjectDisposedException("WPF.Translations.Translator", $"{caller} cannot be accessed because the object instance has been disposed.");
        }

        #endregion
    }
}
