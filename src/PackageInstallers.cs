using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.IO;

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
            var psi = new ProcessStartInfo(ExeName, Params);
            psi.EnvironmentVariables["REBOOT"] = "ReallySuppress";
            var proc = Process.Start(psi);
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

            string msiexec_params = String.Format(CultureInfo.InvariantCulture, "/I \"{0}\" {1} /norestart",
                FilePath, quiet_option);

            int retcode = launchProgramAndWait(@"%SystemRoot%\System32\MsiExec.exe", msiexec_params);

            // MsiExec returns Win32 error codes
            if (retcode != 0)
                throw new Win32Exception(retcode);
        }
    }

    [Export(typeof(IPackageInstallerFactory))]
    public class VsPackageInstallerFactory : IPackageInstallerFactory
    {
        public int AffinityForPackage(string FilePath)
        {
            var di = new DirectoryInfo(Path.GetDirectoryName(FilePath));
            bool is_this_vs = di.GetFiles().Select(x => x.FullName.ToLowerInvariant())
                .Any(x => x.EndsWith("vs_setup.msi"));

            return (is_this_vs ? 10 : 0);
        }

        public IPackageInstaller CreateInstallerForPackage(string FilePath)
        {
            // VS's *real* setup program is actually buried under a folder - if they give us the root
            // installer, let's find the real one
            var setup_path = Path.Combine(Path.GetDirectoryName(FilePath),
                "Setup", "Setup.exe");

            return new DummyExePackageInstaller(setup_path, " /full /noreboot ", " /q ");
        }
    }

    [Export(typeof(IPackageInstallerFactory))]
    public class DummyExePackageInstallerFactory : IPackageInstallerFactory
    {
        public int AffinityForPackage(string FilePath)
        {
            return (FilePath.ToLowerInvariant().EndsWith(".exe") ? 1 : 0);
        }

        public IPackageInstaller CreateInstallerForPackage(string FilePath)
        {
            return new DummyExePackageInstaller(FilePath, "", "");
        }
    }

    public class DummyExePackageInstaller : BasePackageInstaller
    {
        readonly string quietSwitch;
        readonly string exeParams;
        public DummyExePackageInstaller(string FilePath, string Params, string QuietSwitch) : base(FilePath) { exeParams = Params;  quietSwitch = QuietSwitch; }

        public override PackageInstallerSupportedFeatures SupportedFeatures {
            get {
                return (String.IsNullOrEmpty(quietSwitch) ? 0 : PackageInstallerSupportedFeatures.UnattendInstall);
            }
        }

        public override void InstallPackage(Dictionary<PackageInstallerSupportedFeatures, object> Options)
        {
            int retcode = launchProgramAndWait(FilePath, exeParams ?? "");

            // Who knows what this means...
            if (retcode != 0)
                throw new Exception("Failed install");
        }
    }
}

// vim: ts=4:sts=4:sw=4:et
