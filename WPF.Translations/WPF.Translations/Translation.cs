using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace WPF.Translations
{
    /// <summary>
    /// A translation class that is capable of providing bindable property language strings dynamically generated from a resource dictionary. 
    /// This class cannot be inherited.
    /// </summary>
    public sealed class Translation : DynamicObject, INotifyPropertyChanged
    {
        #region Fields

        private object resources;
        private Dictionary<string, string> translations = new Dictionary<string, string>();

        #endregion

        #region Properties

        /// <summary>Gets the number of translations.</summary>
        public int Count => translations.Count;

        /// <summary>Gets the collection of keys associated with this trnalsation object.</summary>
        public Dictionary<string, string>.KeyCollection Keys => translations.Keys;

        /// <summary>Gets the translations data provider.</summary>
        public ITranslationDataProvider TranslationDataProvider { get; }

        #endregion

        #region Events

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Translation"/> class.</summary>
        /// <param name="translations">The resource dictionary that contains our translations.</param>
        /// <param name="dataProvider">The translations data provider (where it pulls translation strings from).</param>
        /// <exception cref="ArgumentNullException">Occurs if either parameter is null.</exception>
        /// <exception cref="TypeInitializationException">Occurs if an exceptions is thrown attempting to read the resource dictionary.</exception>
        public Translation(object translations, ITranslationDataProvider dataProvider)
        {
            if (translations == null)
                throw new ArgumentNullException(nameof(translations));

            if (dataProvider == null)
                throw new ArgumentNullException(nameof(dataProvider));

            resources = translations;
            TranslationDataProvider = dataProvider;

            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                throw new TypeInitializationException("WPF.Translations.Translator", ex);
            }
        }

        #endregion

        #region Methods
        /// <summary>Attempts to get a property by key.</summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value associated to the key.</returns>
        /// <exception cref="KeyNotFoundException">Occurs if the key is not found.</exception>
        public string GetTranslation(string key)
        {
            if (!translations.ContainsKey(key))
                throw new KeyNotFoundException(key);

            return translations[key];
        }

        private void Initialize()
        {
            IReadOnlyList<string> keys = TranslationDataProvider.GetKeys(resources);
            IReadOnlyDictionary<string, string> resourceTranslations = TranslationDataProvider.ReadTranslationData(resources);

            foreach (string key in keys)
            {
                translations[key] = resourceTranslations[key].ToString();
            }
        }

        /// <summary>Attempts to get a property.</summary>
        /// <param name="binder">The binder with the info for property retrieval.</param>
        /// <param name="result">The result of the retrieval.</param>
        /// <returns>True if successfully retrieved otherwise false.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            bool success = translations.TryGetValue(binder.Name, out string temp);

            if (success)
            {
                result = temp;

                return true;
            }
            else
            {
                result = null;

                return false;
            }
        }

        /// <summary>Attempts to get a property by key.</summary>
        /// <param name="key">The key to look for.</param>
        /// <param name="value">The value being returned.</param>
        /// <returns>True if found otherwise false.</returns>
        public bool TryGetValue(string key, out string value)
        {
            if (!translations.ContainsKey(key))
            {
                value = string.Empty;

                return false;
            }

            value = translations[key];

            return true;
        }

        /// <summary>Attempts to set the value of a property.</summary>
        /// <param name="binder">The binder with the info for property retrieval.</param>
        /// <param name="value">The value to set for the property.</param>
        /// <returns>True if the value was set otherwise false.</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // we are not adding properties this way, our properties will be generated from Initialize

            if (!translations.ContainsKey(binder.Name))
            {
                return false;
            }

            // we will not put nulls in the dictionary
            translations[binder.Name] = value?.ToString() ?? string.Empty;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(binder.Name));

            return true;
        }

        #endregion
    }
}
