using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VoskDesktop.Helper
{
    public static class UIHelper
    {
        public static void addNew(UIComponents.XPPanelGroup xpGroup, DateTime date, string text)
        {
            // Cek dan lempar ke UI thread jika dipanggil dari background thread
            if (xpGroup.InvokeRequired)
            {
                xpGroup.Invoke((Action)(() => addNew(xpGroup, date, text)));
                return;
            }

            // 1. Inisialisasi XPPanel baru
            UIComponents.XPPanel newPanel = new UIComponents.XPPanel(265);
            newPanel.SuspendLayout();

            // 2. Hitung posisi koordinat Y agar tidak bertumpuk
            int spacing = 8;
            int nextY = spacing;

            if (xpGroup.Controls.Count > 0)
            {
                // Cari posisi paling bawah dari panel yang sudah ada
                Control lastControl = xpGroup.Controls[xpGroup.Controls.Count - 1];
                nextY = lastControl.Bottom + spacing;
            }

            newPanel.Location = new Point(8, nextY);
            newPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            newPanel.AnimationRate = 5;
            newPanel.BackColor = Color.Transparent;
            newPanel.Caption = date.ToString("dddd MMM yyyy, HH:mm:ss");
            newPanel.CaptionCornerType = UIComponents.CornerType.TopLeft | UIComponents.CornerType.TopRight;
            newPanel.CaptionGradient.Start = Color.White;
            newPanel.CaptionGradient.End = Color.FromArgb(200, 213, 247);
            newPanel.CaptionGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            newPanel.CaptionUnderline = Color.FromArgb(255, 255, 255);
            newPanel.PanelGradient.Start = Color.FromArgb(214, 223, 247);
            newPanel.PanelGradient.End = Color.FromArgb(214, 223, 247);
            newPanel.PanelGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            newPanel.TextColors.Foreground = Color.FromArgb(33, 93, 198);
            newPanel.TextHighlightColors.Foreground = Color.FromArgb(66, 142, 255);
            newPanel.HorzAlignment = StringAlignment.Near;
            newPanel.VertAlignment = StringAlignment.Center;
            newPanel.XPPanelStyle = UIComponents.XPPanelStyle.WindowsXP;
            newPanel.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold);
            newPanel.ForeColor = SystemColors.WindowText;
            newPanel.Size = new Size(xpGroup.ClientSize.Width - 16, 265);

            // Glyphs expand/collapse
            newPanel.CollapsedGlyphs.Normal = 0;
            newPanel.CollapsedGlyphs.Highlight = 1;
            newPanel.CollapsedGlyphs.Pressed = 1;
            newPanel.ExpandedGlyphs.Normal = 2;
            newPanel.ExpandedGlyphs.Highlight = 3;
            newPanel.ExpandedGlyphs.Pressed = 3;

            // 3. Kontrol XPTextBox
            UIComponents.XPTextBox newTextBox = new UIComponents.XPTextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                BorderColor = Color.Black,
                UnderlineColor = Color.Orange,
                Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold),
                Text = text,
                PasswordChar = '\0'
            };

            // 4. Label Copy & Translate
            Label lblCopy = new Label
            {
                Text = "Copy",
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = Color.Blue,
                Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Location = new Point(201, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(newTextBox.Text))
                {
                    Clipboard.SetText(newTextBox.Text);
                    lblCopy.Text = "Copied!";
                    lblCopy.ForeColor = Color.Green;
                    System.Threading.Tasks.Task.Delay(1000).ContinueWith(_ =>
                    {
                        if (!lblCopy.IsDisposed && lblCopy.IsHandleCreated)
                        {
                            lblCopy.Invoke((Action)(() =>
                            {
                                lblCopy.Text = "Copy";
                                lblCopy.ForeColor = Color.Blue;
                            }));
                        }
                    });
                }
            };

            Label lblTranslate = new Label
            {
                Text = "Translate",
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = Color.Blue,
                Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Location = new Point(237, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblTranslate.Click += (s, e) =>
            {
                MessageBox.Show("Fungsi Translate untuk teks ini.", "Translate", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            newPanel.Controls.Add(newTextBox);
            newPanel.Controls.Add(lblTranslate);
            newPanel.Controls.Add(lblCopy);

            lblCopy.BringToFront();
            lblTranslate.BringToFront();

            newPanel.ResumeLayout(false);
            newPanel.PerformLayout();

            // 5. Tambahkan panel ke grup
            xpGroup.Controls.Add(newPanel);

            // 6. Gulir otomatis ke panel paling bawah
            xpGroup.ScrollControlIntoView(newPanel);
        }

    }
}
