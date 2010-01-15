using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using InstallQueuer.Ui;
using System.ComponentModel.Composition.Hosting;

namespace InstallQueuer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application 
    {
        [Import("AppView")]
        MainWindow theWindow { get; set; }

        void Compose()
        {
            var catalog = new AggregateCatalog();
            var thisAssembly = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            catalog.Catalogs.Add(thisAssembly);

            var container = new CompositionContainer(catalog);                        
            container.ComposeParts(this);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Compose();
            this.MainWindow = theWindow;
            theWindow.Show();
        }
    }

    [Export]
    class AppViewModel : INotifyPropertyChanged
    {        
        [ImportMany]
        public IEnumerable<IPackageInstallerFactory> PackageInstallers { get; set; }

        readonly ObservableCollection<InstallableItem> InstallQueue = new ObservableCollection<InstallableItem>();

        [Import("AppView", typeof(MainWindow))]
        IInputElement currentWindow;

        BackgroundWorker _CurrentRunningJob = null;
        public BackgroundWorker CurrentRunningJob {
            get { return _CurrentRunningJob; }
            set {
                if (_CurrentRunningJob == value)
                    return;
                _CurrentRunningJob = value;
                notifyPropertyChanged("CurrentRunningJob");
                notifyPropertyChanged("IsRunning");
            }
        }

        public bool IsRunning {
            get { return (CurrentRunningJob != null); }
        }

        void continueProcessNextItem()
        {
            var current = InstallQueue.FirstOrDefault(x => x.State == InstallableItemState.Queued);
            if (current == null)    // Empty or all items complete
                return;
            if (IsRunning)
                throw new Exception("continueProcessNextItem called when already running");

            CurrentRunningJob = current.DoInstallAsync();
            CurrentRunningJob.RunWorkerCompleted += (o,e) => {
                CurrentRunningJob = null;
                ((FrameworkElement)currentWindow).Dispatcher.BeginInvoke(new Action(continueProcessNextItem));
            };

            CurrentRunningJob.RunWorkerAsync();
        }


        //
        // INotifyPropertyChanged stuff
        //

        void notifyPropertyChanged(String property)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    enum InstallableItemState {
        Initializing,
        Queued,
        Running,
        Succeeded,
        Failed,
    }

    class InstallableItem : INotifyPropertyChanged
    {
        public InstallableItem(IPackageInstaller installer) { PackageInstaller = installer; }

        InstallableItemState _State;
        public InstallableItemState State {
            get { return _State; }
            set {
                if (_State == value)
                    return;
                _State = value;
                notifyPropertyChanged("State");
            }
        }

        public bool SupportsQuietInstall {
            get { return (PackageInstaller.SupportedFeatures & PackageInstallerSupportedFeatures.UnattendInstall) != 0; }
        }

        bool _ShouldQuietInstall;
        public bool ShouldQuietInstall {
            get { return _ShouldQuietInstall; }
            set {
                if (_ShouldQuietInstall == value)
                    return;
                _ShouldQuietInstall = value;
                notifyPropertyChanged("ShouldQuietInstall");
            }
        }

        public string FullPath {
            get { return PackageInstaller.FilePath; }
        }

        public string FileName {
            get { return Path.GetFileName(FullPath); }
        }

        string _ErrorMessage;
        public string ErrorMessage {
            get { return _ErrorMessage; }
            set {
                if (_ErrorMessage == value)
                    return;
                _ErrorMessage = value;
                notifyPropertyChanged("ErrorMessage");
            }
        }

        public readonly IPackageInstaller PackageInstaller;

        public BackgroundWorker DoInstallAsync()
        {
            if (State != InstallableItemState.Queued)
                throw new Exception("Only queued items can be started");

            State = InstallableItemState.Running;

            var ret = new BackgroundWorker();
            ret.DoWork += (o,e) => {
                var opts = new Dictionary<PackageInstallerSupportedFeatures, object>();
                if (SupportsQuietInstall && ShouldQuietInstall)
                    opts[PackageInstallerSupportedFeatures.UnattendInstall] = true;

                try {
                    PackageInstaller.InstallPackage(opts);
                } catch(Exception ex) {
                    e.Result = ex;
                }
            };

            ret.RunWorkerCompleted += (o,e) => {
                var ex = e.Result as Exception;

                if (ex != null)
                    ErrorMessage = ex.Message;

                State = (ex != null ? InstallableItemState.Failed : InstallableItemState.Succeeded);
            };

            return ret;
        }


        //
        // INotifyPropertyChanged stuff
        //

        void notifyPropertyChanged(String property)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

// vim: ts=4:sts=4:sw=4:et
