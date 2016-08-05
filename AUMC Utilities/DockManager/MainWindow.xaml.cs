using System;
using System.Collections.Generic;
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
using DockAPI_Routines;

namespace DockManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            // Retrieve a list of groups from CCB and display them on the form:
            lstGroups.ItemsSource = DockRequest.GetAllGroups();
        }

        private void LstGroups_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            var group = lstGroups.SelectedItem as Group;
            // Retrieve events for the group:
            DockRequest.GetEventsFromLink(group.CalendarFeed);
            var end = DateTime.Now;
            var start = end.AddDays(-7);
        }
    }
}
