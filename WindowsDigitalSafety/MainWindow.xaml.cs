using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WindowsDigitalSafety.Content_Filter;

namespace WindowsDigitalSafety
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Website> websiteList;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Website> WebsiteList
        {
            get { return websiteList; }
            set
            {
                websiteList = value;
                OnPropertyChanged(nameof(WebsiteList));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            websiteList = new ObservableCollection<Website>();
        //    websiteList = new ObservableCollection<Website>
        //{
        //    new Website { Name = "Item 1", IsBlocked = false },
        //    new Website { Name = "Item 2", IsBlocked = true },
        //    new Website { Name = "Item 3", IsBlocked = false }
        //};
            DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string newWebsite = InputTextBox.Text;
            if (!string.IsNullOrEmpty(newWebsite))
            {
                FirewallManager.ManageWebsite(newWebsite, Action.BLOCK);
                
                websiteList.Add(new Website { Name = newWebsite, IsBlocked = true });
                InputTextBox.Text = string.Empty; 
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button removeButton = (Button)sender;
            Website website = removeButton.DataContext as Website;
            if (website != null)
            {
                FirewallManager.ManageWebsite(website.Name, Action.UNBLOCK);

                WebsiteList.Remove(website);
            }
        }
    }

    public class Website
    {
        public string Name { get; set; }
        public bool IsBlocked { get; set; }
    }
}
