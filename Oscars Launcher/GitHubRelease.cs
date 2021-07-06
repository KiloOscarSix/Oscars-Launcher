using System.Collections.Generic;

namespace Oscars_Launcher
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
