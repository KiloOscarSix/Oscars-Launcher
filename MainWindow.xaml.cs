using System;
using System.IO;
using System.Windows;

namespace Oscar_s_Launcher
{
    public partial class MainWindow : Window
    {
        public static string rootPath;
        private Launcher TranscriberLauncher;
        private Launcher RenderTableLauncher;

        public MainWindow()
        {
            InitializeComponent();

            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oscar Six");

            Directory.CreateDirectory(rootPath);

            TranscriberLauncher = new Launcher("AutoTranscriber", LaunchTransciberButton, "https://github.com/College-Kings/autoTranscriber/releases/download/Release/version.txt");
            RenderTableLauncher = new Launcher("AutoRenderTable", LaunchTransciberButton, "https://github.com/College-Kings/autoTranscriber/releases/download/Release/version.txt");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TranscriberLauncher.CheckForUpdates();
            RenderTableLauncher.CheckForUpdates();
        }

        private void LaunchTransciberButton_Click(object sender, RoutedEventArgs e)
        {
            TranscriberLauncher.LaunchSoftware();
        }

        private void LaunchRenderTableButton_Click(object sender, RoutedEventArgs e)
        {
            RenderTableLauncher.LaunchSoftware();
        }
    }

    struct Version
    {
        internal static Version zero = new Version(0, 0, 0);

        private short major;
        private short minor;
        private short patch;

        internal Version(short _major, short _minor, short _patch)
        {
            major = _major;
            minor = _minor;
            patch = _patch;
        }

        internal Version(string _version)
        {
            string[] _versionStrings = _version.Split('.');
            if (_versionStrings.Length != 3)
            {
                major = minor = patch = 0;
                return;
            }

            major = short.Parse(_versionStrings[0]);
            minor = short.Parse(_versionStrings[1]);
            patch = short.Parse(_versionStrings[2]);
        }

        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (major != _otherVersion.major || minor != _otherVersion.minor || patch != _otherVersion.patch) { return true; }
            else { return false; }
        }

        public override string ToString()
        {
            return $"{major}.{minor}.{patch}";
        }
    }
}
