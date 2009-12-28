using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace InstallQueuer.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [ImportMany]
        public IEnumerable<IPackageInstallerFactory> PackageInstallers { get; set; }

        readonly ObservableCollection<InstallableItem> InstallQueue = new ObservableCollection<InstallableItem>();

        public MainWindow()
        {
            InitializeComponent();
        }
    }

    class InstallableItem : INotifyPropertyChanged
    {
        //
        // INotifyPropertyChanged stuff
        //

        private void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

// vim: ts=4:sts=4:sw=4:et
