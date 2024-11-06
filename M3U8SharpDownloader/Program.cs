using LForms.Enums;
using LForms.Extensions;
using M3U8SharpDownloader.Forms;
using M3U8SharpDownloader.Properties;
using System;
using System.Windows.Forms;

namespace M3U8SharpDownloader;

internal static class Program
{
    [STAThread]
    internal static void Main()
    {
        MainForm? mainForm = null;

        try
        {
            ApplicationConfiguration.Initialize();
            mainForm = new MainForm();
            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            _ = ex.HandleException(ErrorType.Critical);
        }
        finally
        {
            mainForm?.Dispose();
        }
    }
}