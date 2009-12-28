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

namespace InstallQueuer
{
    public abstract class BasePackageInstaller : IPackageInstaller 
    {
        public BasePackageInstaller(string FilePath) { _FilePath = FilePath; }

        private readonly string _FilePath;
        public string FilePath { get { return _FilePath; } }

        public abstract PackageInstallerSupportedFeatures SupportedFeatures {get;}
    }

    [Export(typeof(IPackageInstallerFactory))]
    public class MsiPackageInstallerFactory : IPackageInstallerFactory
    {
        public int AffinityForPackage(string FilePath)
        {
            return (FilePath.ToLowerInvariant().EndsWith(".msi") ? 1 : 0);
        }

        public IPackageInstaller CreateInstallerForPackage(string FilePath)
        {
            return new MsiPackageInstaller(FilePath);
        }
    }

    public class MsiPackageInstaller : BasePackageInstaller
    {
        public MsiPackageInstaller(string FilePath) : base(FilePath) {}

        public override PackageInstallerSupportedFeatures SupportedFeatures {
            get {
                return PackageInstallerSupportedFeatures.SupportsUnattendInstall;
            }
        }
    }
}

// vim: ts=4:sts=4:sw=4:et
