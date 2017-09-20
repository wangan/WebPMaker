using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WebPMaker {
    public partial class Mask_Progrs : UserControl {
        public int MaxNum { get; private set; }

        Mask_Progrs() {
            InitializeComponent();
        }

        public Mask_Progrs(int maxNum) : this() {
            MaxNum = maxNum;
            progrs_Bar.Minimum = 0;
            progrs_Bar.Maximum = 100;
        }

        public void ShowInfo(int num) {
            var infoText = $"{num} / {MaxNum}";
            Debug.WriteLine(infoText);

            progrs_Bar.Value = (int)(num * 1.0 / MaxNum * 100);
            progrs_Text.Text = infoText;
            progrs_Bar.PerformStep();
            progrs_Text.Refresh();
        }
    }
}
