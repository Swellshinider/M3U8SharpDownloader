using LForms;
using LForms.Controls.Base;
using LForms.Controls.Buttons;
using LForms.Controls.Mischellaneous;
using LForms.Extensions;
using M3U8SharpDownloader.Forms.Modal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms.Tabs;

internal sealed class MainTab : Panel
{
    private LealButton? _buttonStartStop;
    private LealButton? _buttonAddUrl;

    private bool _isConverting = false;

    public MainTab()
    {
        InitializeComponents();
    }

    private int TopGap
    {
        get
        {
            if (Parent == null || Parent is not LealTabManager ltm)
                return Configurator.BaseGap;

            return Configurator.BaseGap + ltm.ButtonsAreaSize;
        }
    }

    private void InitializeComponents()
    {
        Dock = DockStyle.Fill;

        _buttonStartStop = new LealButton()
        {
            Rounded = true,
            Text = "Start Conversion",
        };
        _buttonStartStop.Click += (s, e) => StartStopConversion(_buttonStartStop!);

        _buttonAddUrl = new LealButton()
        {
            Rounded = true,
            Text = "Add url"
        };
        _buttonAddUrl.Click += (s, e) => LoadModal();

        this.Add(_buttonStartStop);
        this.Add(_buttonAddUrl);
        Resize += (s, e) => ReDraw();
    }

    private void StartStopConversion(LealButton button)
    {
        if (!_isConverting)
        {
            _isConverting = true;
            button.Text = "Stop Conversion";
        }
        else
        {
            var dialogResult = MessageBox.Show("This will cancel all your current download/convertion process.\nAre you sure?", 
                "Are you sure?", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button1
                );

            if (dialogResult != DialogResult.Yes)
                return;

            _isConverting = false;
            button.Text = "Start Conversion";
        }
    }

    private void LoadModal()
    {
        var modal = new UrlDataModal("Add UrlData");
        modal.ShowDialog();
    }

    private void ReDraw()
    {
        Trace.Assert(_buttonStartStop != null);
        _buttonStartStop.SetYAbsolute(Configurator.BaseGap);
        _buttonStartStop.SetXFromLeft(this, Configurator.BaseGap);

        Trace.Assert(_buttonAddUrl != null);
        _buttonAddUrl.SetYOffSetNext(_buttonStartStop, Configurator.BaseGap);
        _buttonAddUrl.SetXFromLeft(this, Configurator.BaseGap);
    }
}