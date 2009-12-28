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

namespace InstallQueuer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application 
    {
    }

    class AppViewModel : INotifyPropertyChanged
    {    
        public AppViewModel(IInputElement view) { currentWindow = view; }

        [ImportMany]
        public IEnumerable<IPackageInstallerFactory> PackageInstallers { get; set; }

        readonly ObservableCollection<InstallableItem> InstallQueue = new ObservableCollection<InstallableItem>();

        readonly IInputElement currentWindow;

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

        public string FullPath {
            get { return PackageInstaller.FilePath; }
        }

        public string FileName {
            get { return Path.GetFileName(FullPath); }
        }

        public readonly IPackageInstaller PackageInstaller;

        public BackgroundWorker DoInstall()
        {
            if (State != InstallableItemState.Queued)
                throw new Exception("Only queued items can be started");
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
