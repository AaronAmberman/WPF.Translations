using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using WPF.Translations;

namespace Testing
{
    public partial class App : Application
    {
        /*
         * This Translator should be put anywhere that is globally accessible so that it may be used in code as well 
         * (statically available on the App or on a ServiceLocator). 
         * 
         * The Translator itself, however, should NOT be put on the MainWindow or a view model in a MVVM setup. When 
         * using the API, the MainWindow or the view model should have a Translation not a Translator. This is because 
         * the Translation object provides the actual translations. This is what you will use in code or what you will 
         * bind to in the UI to get translated strings.
         */
        public static Translator Translator { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // setup our language resources
            Translator translator = new Translator
            {
                KeyContract = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Languages/Language.en.xaml")
                }
            };

            try
            {
                translator.AddResourceDictionariesForTranslation(new List<Tuple<string, ResourceDictionary>>
                {
                    new Tuple<string, ResourceDictionary>("en",  new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Languages/Language.en.xaml")
                    }),
                    new Tuple<string, ResourceDictionary>("fr",  new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Languages/Language.fr.xaml")
                    }),
                    new Tuple<string, ResourceDictionary>("it",  new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Languages/Language.it.xaml")
                    }),
                    new Tuple<string, ResourceDictionary>("ru",  new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Languages/Language.ru.xaml")
                    }),
                    new Tuple<string, ResourceDictionary>("zh-Hans",  new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Languages/Language.zh-Hans.xaml")
                    }),
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred.{Environment.NewLine}{ex}");

                MessageBox.Show("The application could not properly load translations. Exiting application.", "Translation Load Error");

                Environment.Exit(-1);
                return;
            }

            // set the culture on startup to what is selected in settings
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Testing.Properties.Settings.Default.Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Testing.Properties.Settings.Default.Culture);

            // set so all future threads are created in the appropriate culture
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Testing.Properties.Settings.Default.Culture);

            translator.CurrentTranslations = translator.Translations[Testing.Properties.Settings.Default.Culture];

            Translator = translator;
        }
    }
}
