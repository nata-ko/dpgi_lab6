using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab6
{
    /// <summary>
    /// Логика взаимодействия для ConvertPage.xaml
    /// </summary>
    public partial class ConvertPage : Page
    {
        int prevFromIndex;
        int prevToIndex;
        private SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CurrenciesDB"].ConnectionString);
        CurrenciesDBEntities context;
        public ConvertPage()
        {
            InitializeComponent();
            context = new CurrenciesDBEntities();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            convertFromComboBox.ItemsSource = context.Currencies.Select(s => s.Currency1).ToList();
            convertFromComboBox.SelectedIndex = 0;
            prevFromIndex = 0;
            convertToComboBox.ItemsSource = context.Currencies.Select(s => s.Currency1).ToList();
            convertToComboBox.SelectedIndex = 1;
            prevToIndex = 1;
           

          //  currenciesForFilter.ItemsSource = currencies;
        }

        private void convertBtn_Click(object sender, RoutedEventArgs e)
        {
            swapCurrencies();
        }

        private void convertFromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedFromIndex = convertFromComboBox.SelectedIndex;
            int selectedToIndex = convertToComboBox.SelectedIndex;
            currentFromLabel.Content = convertFromComboBox.SelectedItem.ToString();
            if (selectedToIndex == selectedFromIndex)
            {
                prevToIndex = convertToComboBox.SelectedIndex;
                convertToComboBox.SelectedIndex = prevFromIndex;
                prevFromIndex = convertFromComboBox.SelectedIndex;
            }

            convertNumber.Text = "";
        }
        private void swapCurrencies()
        {
            int index1 = convertFromComboBox.SelectedIndex;
            convertFromComboBox.SelectedIndex = convertToComboBox.SelectedIndex;
            convertToComboBox.SelectedIndex = index1;
        }

        private void convertToComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedFromIndex = convertFromComboBox.SelectedIndex;
            int selectedToIndex = convertToComboBox.SelectedIndex;
            currentToLabel.Content = convertToComboBox.SelectedItem.ToString();
            if (selectedToIndex == selectedFromIndex)
            {
                prevFromIndex = convertFromComboBox.SelectedIndex;
                convertFromComboBox.SelectedIndex = prevToIndex;
                prevToIndex = convertToComboBox.SelectedIndex;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {


            if (inputNumber.Text.Trim().Length > 0)
            {
                decimal inputedV = 0;
                if (Decimal.TryParse(inputNumber.Text, out _))
                {
                    inputedV = Decimal.Parse(inputNumber.Text);
                }
                else
                {
                    MessageBox.Show("Please, Input number");
                    return;
                }


                string fromCurrencies = convertFromComboBox.SelectedItem.ToString();

                string toCurrencies = convertToComboBox.SelectedItem.ToString();

                // get value from db
                // for from currency
                decimal initialCurrencyFromCurrencyToUSD = context.Currencies.Where(c => c.Currency1 == fromCurrencies).ToList().First().FromCurrencyToUSD;
                decimal initialCurrencyFromUSDToCurrency = context.Currencies.Where(c => c.Currency1 == fromCurrencies).ToList().First().FromUSDToCurrency;

                decimal resultedCurrencyFromCurrencyToUSD = context.Currencies.Where(c => c.Currency1 == toCurrencies).ToList().First().FromCurrencyToUSD;
                decimal resultedCurrencyFromUSDToCurrency = context.Currencies.Where(c => c.Currency1 == toCurrencies).ToList().First().FromUSDToCurrency;

                convertNumber.Text = ((inputedV * initialCurrencyFromCurrencyToUSD) * resultedCurrencyFromUSDToCurrency).ToString();


                // add info to history
                History history = new History
                {
                    Date = DateTime.Now.ToShortDateString(),
                    FromCurrency = fromCurrencies,
                    ToCurrency = toCurrencies,
                    InputValue = inputedV.ToString(),
                    ConvertedValue = convertNumber.Text.Substring(0, 10)

                };
                context.Histories.Add(history);
                try
                {

                    context.SaveChanges();
                    connection.Open();
                    SqlCommand command = new SqlCommand("Insert into [History](Date, FromCurrency,ToCurrency,InputValue,ConvertedValue) VALUES(@Date, @FromCurrency,@ToCurrency,@InputValue,@ConvertedValue)", connection);
                    command.Parameters.AddWithValue("Date", DateTime.Now.ToShortDateString());
                    command.Parameters.AddWithValue("FromCurrency", fromCurrencies);
                    command.Parameters.AddWithValue("ToCurrency", toCurrencies);
                    command.Parameters.AddWithValue("InputValue", inputedV.ToString());
                    command.Parameters.AddWithValue("ConvertedValue", convertNumber.Text.Substring(0, 10));
                    command.ExecuteNonQuery();
                    connection.Close();

                }
                catch (DbEntityValidationException ex)
                {
                    MessageBox.Show($"The data to db were not recorded because of {ex}");

                }


            }
            else
            {
                MessageBox.Show("Please, input something");
            }

        }
    }
}
