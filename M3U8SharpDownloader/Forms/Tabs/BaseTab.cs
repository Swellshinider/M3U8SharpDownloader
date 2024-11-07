using LForms.Controls.Panels;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms.Tabs;

internal abstract class BaseTab : LealPanel
{
    internal BaseTab() : base(true)
    {
        Dock = DockStyle.Fill;
    }
}