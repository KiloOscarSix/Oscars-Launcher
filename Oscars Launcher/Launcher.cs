using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Oscars_Launcher
{
    class Launcher
    {
        private readonly string name;
        private readonly string rootPath;
        private readonly string versionFile;
        private readonly string versionFileLink;
        internal string softwareExe;
        private readonly Button LaunchButton;

        public Launcher(string _name, Button _LaunchButton, string _versionFileLink)
        {
            name = _name;
            rootPath = Path.Combine(MainWindow.rootPath, name);
            LaunchButton = _LaunchButton;
            versionFileLink = _versionFileLink;

            versionFile = Path.Combine(rootPath, "version.txt");
            softwareExe = Path.Combine(rootPath, $"{name}.exe");

            Directory.CreateDirectory(rootPath);
        }

        public void CheckForUpdates()
        {
            if (File.Exists(versionFile))
            {
                Version localVersion = new(File.ReadAllText(versionFile));

                try
                {
                    WebClient webClient = new();
                    Version onlineVersion = new(webClient.DownloadString(versionFileLink));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallSoftwareFiles(true, onlineVersion);
                    }
                    else
                    {
                        LaunchButton.Content = "Launch";
                    }
                }
                catch (Exception error)
                {
                    LaunchButton.Content = "Update Failed - Retry";
                    MessageBox.Show($"Error check for software updates: {error}");
                }
            }
            else
            {
                InstallSoftwareFiles(false, Version.zero);
            }
        }

        private void InstallSoftwareFiles(bool _isUpdate, Version onlineVersion)
        {
            try
            {
                WebClient webClient = new();
                if (_isUpdate)
                {
                    LaunchButton.Content = "Downloading Update";
                }
                else
                {
                    LaunchButton.Content = "Downloading Software";
                    onlineVersion = new Version(webClient.DownloadString(versionFileLink));
                }

                string[] downloadLinks = GetDownloadLinks();
                
                foreach (string link in downloadLinks)
                {
                    string[] fileArray = link.Split('/');
                    string fileName = fileArray[^1];

                    using (WebClient webClient2 = new())
                    {
                        webClient2.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadSoftwareCompletedCallback);
                        webClient2.DownloadFileAsync(new Uri(link), Path.Combine(rootPath, fileName), onlineVersion);
                    }
                }
            }
            catch (Exception error)
            {
                LaunchButton.Content = "Installation Failed - Retry";
                MessageBox.Show($"Error installing software files: {error}");
            }
        }

        private static string HTTPRequest_Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers["accept"] = "application/vnd.github.v3+json";
            request.Headers["User-Agent"] = "request";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new(stream))
            { return reader.ReadToEnd(); }
        }

        private string[] GetDownloadLinks()
        {
            string lowerCaseName = name[0].ToString().ToLower() + name[1..];
            string jsonString = HTTPRequest_Get(@$"https://api.github.com/repos/College-Kings/{lowerCaseName}/releases/latest");
            GitHubRelease githubRelease = JsonSerializer.Deserialize<GitHubRelease>(jsonString);

            List<string> rv = new();
            foreach (GitHubAssets asset in githubRelease.assets)
            {
                rv.Add(asset.browser_download_url);
            }

            return rv.ToArray();
        }

        private void DownloadSoftwareCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersionString = ((Version)e.UserState).ToString();
                File.WriteAllText(versionFile, onlineVersionString);
                LaunchButton.Content = "Launch";
            }
            catch (Exception error)
            {
                LaunchButton.Content = "Download Failed - Retry";
                MessageBox.Show($"Error finishing download: {error}");
            }
        }

        internal void LaunchSoftware()
        {
            if (File.Exists(softwareExe) && LaunchButton.Content.ToString() == "Launch")
            {
                ProcessStartInfo startInfo = new(softwareExe)
                {
                    WorkingDirectory = Path.Combine(rootPath)
                };
                Process.Start(startInfo);
            }
            else if (!LaunchButton.Content.ToString().StartsWith("Downloading"))
            {
                CheckForUpdates();
            }
        }
    }
}
