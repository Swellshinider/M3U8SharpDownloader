using M3U8SharpDownloader.Forms;
using System;
using System.Windows.Forms;

namespace M3U8SharpDownloader;

internal static class Program
{
    [STAThread]
    internal static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(Configurator.ProgramName));
    }
}