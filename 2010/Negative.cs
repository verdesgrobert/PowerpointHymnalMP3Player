using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.Office.Tools.Ribbon;

namespace PowerpointHymnalMP3Player
{
    public partial class Negative
    {
        private void Negative_Load(object sender, RibbonUIEventArgs e)
        {
            chkEnabled.Click += chkEnabled_Click;

            chkEnabled.Checked = Properties.Settings.Default.Activated;
            dropDown1.SelectedItemIndex = Properties.Settings.Default.SlideNumber - 1;
            Properties.Settings.Default.Save();
            cboAudioType.SelectedItemIndex = Properties.Settings.Default.Mode == AudioTypeEnum.VLC ? 0 : 1;
        }

        void chkEnabled_Click(object sender, RibbonControlEventArgs e)
        {
            Properties.Settings.Default.Activated = chkEnabled.Checked; 
            Properties.Settings.Default.Save();
        }

        private void Negative_Close(object sender, EventArgs e)
        {
            Properties.Settings.Default.SlideNumber = dropDown1.SelectedItemIndex + 1;
            Properties.Settings.Default.Activated = chkEnabled.Checked; Properties.Settings.Default.Save();
        }

        private void dropDown1_SelectionChanged(object sender, RibbonControlEventArgs e)
        {
            Properties.Settings.Default.SlideNumber = dropDown1.SelectedItemIndex + 1;
            Properties.Settings.Default.Save();
        }

        private void cboAudioType_SelectionChanged(object sender, RibbonControlEventArgs e)
        {
            Properties.Settings.Default.Mode =
                (AudioTypeEnum)(Enum.Parse(typeof(AudioTypeEnum), cboAudioType.SelectedItem.Label));
            Properties.Settings.Default.Save();
        }
    }
}
