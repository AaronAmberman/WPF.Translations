using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WPF.Translations;

namespace Testing
{
    public partial class MainWindow : Window
    {
        #region Fields

        private bool isLoading;

        #endregion

        #region Properties

        /*
         * The magic happens because the translations object is dynamic. However, the underlying type is
         * Translation which does not allow for random property addition.
         */

        public dynamic Translations
        {
            get { return (dynamic)GetValue(TranslationsProperty); }
            set { SetValue(TranslationsProperty, value); }
        }

        public static readonly DependencyProperty TranslationsProperty =
            DependencyProperty.Register("Translations", typeof(Translation), typeof(MainWindow), new PropertyMetadata(null));

        #endregion

        #region Constructors

        public MainWindow()
        {
            /*
             * Prior to initializing our component we want to set the translations
             */
            Translations = App.Translator.CurrentTranslations;

            InitializeComponent();
        }

        #endregion

        #region Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoading = true;

            if (Properties.Settings.Default.Culture == "en")
            {
                comboBox.SelectedIndex = 0;
            }
            else if (Properties.Settings.Default.Culture == "fr")
            {
                comboBox.SelectedIndex = 1;
            }
            else if (Properties.Settings.Default.Culture == "it")
            {
                comboBox.SelectedIndex = 2;
            }
            else if (Properties.Settings.Default.Culture == "ru")
            {
                comboBox.SelectedIndex = 3;
            }
            else if (Properties.Settings.Default.Culture == "zh-Hans")
            {
                comboBox.SelectedIndex = 4;
            }

            isLoading = false;

            // This will throw an exception because our type does not allow for dynamic properties to be added.
            // Translations are tied to the resource dictionaries used to generate them (this is enforced).
            // NOTE: keep this in my when using in XAML
            //Translations.MyNewProperty = "Does this work?";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading) return;

            if (comboBox.SelectedIndex == 0)
            {
                Properties.Settings.Default.Culture = "en";
            }
            else if (comboBox.SelectedIndex == 1)
            {
                Properties.Settings.Default.Culture = "fr";
            }
            else if (comboBox.SelectedIndex == 2)
            {
                Properties.Settings.Default.Culture = "it";
            }
            else if (comboBox.SelectedIndex == 3)
            {
                Properties.Settings.Default.Culture = "ru";
            }
            else if (comboBox.SelectedIndex == 4)
            {
                Properties.Settings.Default.Culture = "zh-Hans";
            }

            Properties.Settings.Default.Save();

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Testing.Properties.Settings.Default.Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Testing.Properties.Settings.Default.Culture);

            // set so all future threads are created in the appropriate culture
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Testing.Properties.Settings.Default.Culture);

            App.Translator.CurrentTranslations = App.Translator.Translations[Testing.Properties.Settings.Default.Culture];

            Translations = App.Translator.CurrentTranslations;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Translations.ErrorMessage);
        }

        #endregion
    }
}
