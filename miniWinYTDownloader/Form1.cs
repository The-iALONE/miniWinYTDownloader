using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            // webView21.CoreWebView2.OpenDevToolsWindow();

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

            var psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = $"-f best --newline -o \"{output}\" {url}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi)!;

            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (line == null) continue;

                var match = Regex.Match(line, @"(\d{1,3}\.\d)");

                if (match.Success) {
                    int precent = (int)float.Parse(match.Groups[1].Value);

                    webView21.Invoke(new Action(() =>
                    {
                        webView21.CoreWebView2.PostWebMessageAsJson(
                            $"{{\"type\":\"progress\",\"value\":{precent}}}"
                        );
                    }));
                }
            }
            webView21.Invoke(new Action(() =>
            {
                webView21.CoreWebView2.PostWebMessageAsJson(
                    $"{{\"type\":\"progress\",\"value\":100}}"
                );
            }));
        }
    }
}
