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
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;

namespace InstallQueuer
{
    public abstract class BasePackageInstaller : IPackageInstaller 
    {
        public BasePackageInstaller(string FilePath) { _FilePath = FilePath; }

        private readonly string _FilePath;
        public string FilePath { get { return _FilePath; } }

        public abstract PackageInstallerSupportedFeatures SupportedFeatures {get;}
        public abstract void InstallPackage(Dictionary<PackageInstallerSupportedFeatures, object> options);

        protected static int launchProgramAndWait(string ExeName, string Params)
        {
            var proc = Process.Start(ExeName, Params);
            proc.WaitForExit();
            return proc.ExitCode;
        }
    }

    [Export(typeof(IPackageInstallerFactory))]
    public class MsiPackageInstallerFactory : IPackageInstallerFactory
    {
        public int AffinityForPackage(string FilePath)
        {
            return (FilePath.ToLowerInvariant().EndsWith(".msi") ? 5 : 0);
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
                return PackageInstallerSupportedFeatures.UnattendInstall;
            }
        }

        public override void InstallPackage(Dictionary<PackageInstallerSupportedFeatures, object> Options)
        {
            string quiet_option = (Options.ContainsKey(PackageInstallerSupportedFeatures.UnattendInstall) ?
                " /qn " : "");

            string msiexec_params = String.Format(CultureInfo.InvariantCulture, "/I \"{0}\" {1}",
                FilePath, quiet_option);

            int retcode = launchProgramAndWait(@"%SystemRoot%\System32\MsiExec.exe", msiexec_params);

            // MsiExec returns Win32 error codes
            if (retcode != 0)
                throw new Win32Exception(retcode);
        }
    }

    [Export(typeof(IPackageInstallerFactory))]
    public class ExePackageInstallerFactory : IPackageInstallerFactory
    {
        public int AffinityForPackage(string FilePath)
        {
            return (FilePath.ToLowerInvariant().EndsWith(".exe") ? 1 : 0);
        }

        public IPackageInstaller CreateInstallerForPackage(string FilePath)
        {
            return new ExePackageInstaller(FilePath);
        }
    }

    public class DummyExePackageInstaller : BasePackageInstaller
    {
        public DummyExePackageInstaller(string FilePath) : base(FilePath) {}

        public override PackageInstallerSupportedFeatures SupportedFeatures {
            get {
                return 0;
            }
        }

        public override void InstallPackage(Dictionary<PackageInstallerSupportedFeatures, object> Options)
        {
            int retcode = launchProgramAndWait(FilePath);

            // Who knows what this means...
            if (retcode != 0)
                throw new Exception("Failed install");
        }
    }
}

// vim: ts=4:sts=4:sw=4:et
