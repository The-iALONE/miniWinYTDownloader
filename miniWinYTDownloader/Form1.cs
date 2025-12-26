using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace miniWinYTDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();

            webView21.CoreWebView2.WebMessageReceived += WebMessageReceived;

            string uiPath = Path.Combine(Application.StartupPath, "ui");
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "app.local",
                uiPath,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow
            );

            webView21.Source = new Uri("https://app.local/index.html");
        }

        private void WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            string url = e.WebMessageAsJson.Trim('"'); // لینک

            Download(url);
        }

        void Download(string url)
        {
            string exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");

            string output = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                "%(title)s.%(ext)s"
            );

            var proc = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = $"-f best -o \"{output}\" {url}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(proc);
        }
    }
}
