using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace DurableV2Lab.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client = new HttpClient
        {
            //BaseAddress = new Uri("http://localhost:7071"),
            BaseAddress = new Uri("https://durablelabkaota.azurewebsites.net"),
        };
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonIncrement_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                await _client.PostAsync($"api/counter/{textBoxName.Text}", new StringContent(textBoxAmount.Text, Encoding.UTF8, "text/plain"));
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void ButtonIncrement100Times_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                const int count = 100;
                var res = await Task.WhenAll(
                    Enumerable.Range(0, count)
                        .Select(_ => _client.PostAsync(
                            $"api/counter/{textBoxName.Text}",
                            new StringContent(textBoxAmount.Text, Encoding.UTF8, "text/plain")
                        )
                    )
                );

                var successes = res.Where(x => x.IsSuccessStatusCode).Count();
                MessageBox.Show($"{successes}/{count} was succeeded");
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void ButtonGetStatus_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                var res = await _client.GetAsync($"api/counter/{textBoxName.Text}");
                textBlockValue.Text = JsonConvert.DeserializeObject<dynamic>(await res.Content.ReadAsStringAsync()).entityState.value;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
