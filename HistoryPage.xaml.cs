using System;
using System.Collections.Generic;
using System.Configuration;
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
      /// Логика взаимодействия для HistoryPage.xaml
      /// </summary>
    public partial class HistoryPage : Page
    {
        private bool isDirty = true;
        private SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CurrenciesDB"].ConnectionString);
        CurrenciesDBEntities context;
        public HistoryPage()
        {
            InitializeComponent();
            context = new CurrenciesDBEntities();
            FullHistory_Load();
            currenciesForFilter.ItemsSource = context.Currencies.Select(s => s.Currency1).ToList();
            currenciesForFilter.SelectedIndex = 0;

        }

        private void FullHistory_Load()
        {
            historyTable.ItemsSource = context.Histories.ToList();
        }

        private void FilterByDate()
        {
            string date = datePicker.Text;
            var list = context.Histories.Where(h => h.Date == date).ToList();
            if (list.Count > 0)
            {
                historyTable.ItemsSource = list;
            }
            else
            {
                MessageBox.Show("There is no recors with this date");
            }

        }
      

        private void FilterByCurrency()
        {
            string filteringCurrency = currenciesForFilter.SelectedItem.ToString();
            var list = context.Histories.Where(h => h.FromCurrency == filteringCurrency).ToList();
            if (list.Count > 0)
            {
                historyTable.ItemsSource = list;
            }
            else
            {
                MessageBox.Show("There were no convertation with this initial curreny");
            }
        }


        public void LoadFullHistoryCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        public void FilterHistoryByDateCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        public void FilterHistoryByCurrencyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void LoadFullHistoryCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FullHistory_Load();
            isDirty = true;
        }


        private void FilterHistoryByDateCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterByDate();
            isDirty = true;
        }

        private void FilterHistoryByCurrencyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterByCurrency();
            isDirty = true;
        }

    }
}
