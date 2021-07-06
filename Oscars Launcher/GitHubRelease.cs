using System.Collections.Generic;

namespace Oscar_s_Launcher
{
    class GitHubRelease
    {
        public List<GitHubAssets> assets { get; set; }
    }

    class GitHubAssets
    {
        public string browser_download_url { get; set; }
    }
}
