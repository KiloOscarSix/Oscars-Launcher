using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
