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

namespace InstallQueuer
{
    [Flags]  
    public enum PackageInstallerSupportedFeatures {
        UnattendInstall = 0x01,
        ValidFlagMask = 0x01,
    }

    public interface IPackageInstallerFactory {
        int AffinityForPackage(string FilePath);
        IPackageInstaller CreateInstallerForPackage(string FilePath);
    }

    public interface IPackageInstaller {        
        string FilePath { get; }
        string UserFriendlyDescription { get; }
        ImageSource Icon { get; }
        PackageInstallerSupportedFeatures SupportedFeatures { get; }

        void InstallPackage(Dictionary<PackageInstallerSupportedFeatures, object> options);
    }
}

// vim: ts=4:sts=4:sw=4:et
