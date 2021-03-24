using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stiky
{
    public partial class Stiky : Form
    {
        //Finally, make the form draggable with our panel. Add those at the class level:
        //https://stackoverflow.com/questions/29024910/how-to-design-a-custom-close-minimize-and-maximize-button-in-windows-form-appli
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int YELLOW = 0;
        public const int PINK = 1;
        public const int BLUE = 2;
        public const int GREEN = 3;

        private Color[] stikyBGColors = new Color[]
        {
            Color.FromArgb(225, 225, 128),
            Color.FromArgb(225, 192, 255),
            Color.FromArgb(100, 149, 237),
            Color.FromArgb(173, 225, 47)
        };

        private Button exitButton;
        private Button minimizeButton;
        private Button settingsButton;
        private Button newPageButton;
        private Panel titleBarPanel;
        private Panel settingsPanel;
        private Panel bottomBarPanel;
        private Label settingsLabel;
        private bool settingsPanelOpen = false;
        private const int TITLE_BAR_HEIGHT = 24;
        private const int TITLE_BAR_OFFSET = 16;
        private const int SETTINGS_FORM_WIDTH = 100;


        public Stiky()
        {
            InitScreen();
        }

        private void InitScreen()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Stiky";
            this.Text = "Stiky";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Stiky_MouseDown);

            /* Set previous window position and size to Stiky window */
            this.StartPosition = FormStartPosition.Manual;
            this.Size = Properties.Settings.Default.WindowSize;
            this.Location = Properties.Settings.Default.WindowPosition;

            CheckIfFormOutsideOfTheScreen();

            /* Clear background and update buttons */
            this.FormBorderStyle = FormBorderStyle.None;
            this.MinimumSize = Properties.Settings.Default.MinWindowSize;
            this.BackColor = Properties.Settings.Default.BackgroundColor;

            /* Creating title bar */
            titleBarPanel = new Panel();
            titleBarPanel.Width = this.Width - TITLE_BAR_OFFSET;
            titleBarPanel.Height = TITLE_BAR_HEIGHT;
            titleBarPanel.Location = new Point(0, 0);
            titleBarPanel.BackColor = Color.Transparent;
            titleBarPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            titleBarPanel.AutoSize = false;
            titleBarPanel.Dock = DockStyle.None;
            titleBarPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            Controls.Add(titleBarPanel);

            /* Creating settings bar */
            settingsPanel = new Panel();
            settingsPanel.Width = 0;
            settingsPanel.Height = this.Height;
            settingsPanel.Location = new Point(0, 0);
            settingsPanel.BackColor = Color.Transparent;
            settingsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            settingsPanel.AutoSize = false;
            settingsPanel.Dock = DockStyle.None;
            settingsPanel.Visible = false;
            Controls.Add(settingsPanel);

            /* Creating bottom bar */
            bottomBarPanel = new Panel();
            bottomBarPanel.Width = 0;
            bottomBarPanel.Height = this.Height;
            bottomBarPanel.Location = new Point(0, 0);
            bottomBarPanel.BackColor = Color.LightPink;
            bottomBarPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            bottomBarPanel.AutoSize = false;
            bottomBarPanel.Dock = DockStyle.None;
            bottomBarPanel.Visible = false;
            Controls.Add(bottomBarPanel);

            /* -Add controls to panel */
            /* --Label */
            settingsLabel = new Label();
            settingsLabel.Width = SETTINGS_FORM_WIDTH;
            settingsLabel.Height = TITLE_BAR_HEIGHT;
            settingsLabel.Location = new Point(0, TITLE_BAR_HEIGHT + 1);
            settingsLabel.AutoSize = false;
            settingsLabel.TextAlign = ContentAlignment.MiddleCenter;
            settingsLabel.BackColor = Color.LightGray;
            settingsLabel.Text = Properties.Settings.Default.SettingsTitle;
            settingsLabel.Name = "SettingsLabel";
            settingsLabel.Font = new Font("Constantia", 14, FontStyle.Bold);
            settingsPanel.Controls.Add(settingsLabel);

            /*  -Creating minmize Button */
            minimizeButton = new Button();
            minimizeButton.Dock = DockStyle.Right;
            minimizeButton.Width = TITLE_BAR_HEIGHT;
            minimizeButton.FlatStyle = FlatStyle.Flat;
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.BackColor = Color.Transparent;
            minimizeButton.Name = "MinimizeButton";
            minimizeButton.BackgroundImage = Properties.Resources.minimize.ToBitmap();
            minimizeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            minimizeButton.Click += new EventHandler(minimizeButton_Click);
            minimizeButton.FlatAppearance.MouseOverBackColor = ControlPaint.LightLight(this.BackColor);
            minimizeButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(this.BackColor);
            titleBarPanel.Controls.Add(minimizeButton);

            /*  -Creating Exit Button */
            exitButton = new Button();
            exitButton.Dock = DockStyle.Right;
            exitButton.Width = TITLE_BAR_HEIGHT;
            exitButton.FlatStyle = FlatStyle.Flat;
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.BackColor = Color.Transparent;
            exitButton.Name = "ExitButton";
            exitButton.BackgroundImage = Properties.Resources.exit.ToBitmap();
            exitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            exitButton.Click += new EventHandler(exitButton_Click);
            exitButton.FlatAppearance.MouseOverBackColor = ControlPaint.LightLight(this.BackColor);
            exitButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(this.BackColor);
            titleBarPanel.Controls.Add(exitButton);

            /* -Create New Button */
            newPageButton = new Button();
            newPageButton.Dock = DockStyle.Right;
            newPageButton.Width = TITLE_BAR_HEIGHT;
            newPageButton.Height = TITLE_BAR_HEIGHT;
            newPageButton.FlatStyle = FlatStyle.Flat;
            newPageButton.FlatAppearance.BorderSize = 0;
            newPageButton.BackColor = Color.Transparent;
            newPageButton.Name = "NewButton";
            newPageButton.BackgroundImage = Properties.Resources.newPage.ToBitmap();
            newPageButton.BackgroundImageLayout = ImageLayout.Stretch;
            newPageButton.Click += new EventHandler(newPageButton_Click);
            newPageButton.FlatAppearance.MouseOverBackColor = ControlPaint.LightLight(this.BackColor);
            newPageButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(this.BackColor);
            bottomBarPanel.Controls.Add(newPageButton);

            /* -Creating Settings Button */
            settingsButton = new Button();
            settingsButton.Dock = DockStyle.Left;
            settingsButton.Width = TITLE_BAR_HEIGHT;
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.FlatAppearance.BorderSize = 0;
            settingsButton.BackColor = Color.Transparent;
            settingsButton.Name = "SettingsButton";
            settingsButton.BackgroundImage = Properties.Resources.settings.ToBitmap();
            settingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            settingsButton.Click += new EventHandler(settingsButton_Click);
            settingsButton.FlatAppearance.MouseOverBackColor = ControlPaint.LightLight(this.BackColor);
            settingsButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(this.BackColor);
            titleBarPanel.Controls.Add(settingsButton);

            /* Add event handler for save settings at the closing */
            Application.ApplicationExit += new EventHandler(SaveSettings);

            this.ResumeLayout(false);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            System.Timers.Timer settingsAnimTimer = new System.Timers.Timer();
            settingsAnimTimer.Interval = 20;
            settingsAnimTimer.AutoReset = true;
            settingsAnimTimer.SynchronizingObject = settingsPanel;
            
            settingsAnimTimer.Elapsed += (sender, args) =>
            {
                if(settingsPanelOpen)
                {
                    if(settingsPanel.Width >= 10)
                    {
                        settingsPanel.Width -= 10;
                    }
                    else
                    {
                        settingsPanel.Width = 0;
                        settingsPanel.Visible = false;
                        settingsAnimTimer.Stop();
                        settingsPanelOpen = false;
                    }
                }
                else
                {
                    settingsPanel.Visible = true;
                    settingsPanel.Width += 10;
                    if (settingsPanel.Width >= SETTINGS_FORM_WIDTH)
                    {
                        settingsPanel.Width = SETTINGS_FORM_WIDTH;
                        settingsAnimTimer.Stop();
                        settingsPanelOpen = true;
                    }
                }
            };
            settingsAnimTimer.Start();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void newPageButton_Click(object sender, EventArgs e)
        {

        }

        private void SaveSettings(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.WindowPosition = this.Location;
            Properties.Settings.Default.Save();
        }

        private void CheckIfFormOutsideOfTheScreen()
        {
            int _offset = 50;
            if ((Screen.PrimaryScreen.Bounds.Width - _offset <= this.Location.X) ||
                (Screen.PrimaryScreen.Bounds.Height - _offset <= this.Location.Y))
            {
                this.Location = new Point(0, 0);
            }
        }

        //and plug them in a MouseDown event of the panel:
        private void Stiky_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const UInt32 WM_NCHITTEST = 0x0084;
            const UInt32 WM_MOUSEMOVE = 0x0200;

            const UInt32 HTLEFT = 10;
            const UInt32 HTRIGHT = 11;
            const UInt32 HTBOTTOMRIGHT = 17;
            const UInt32 HTBOTTOM = 15;
            const UInt32 HTBOTTOMLEFT = 16;
            const UInt32 HTTOP = 12;
            const UInt32 HTTOPLEFT = 13;
            const UInt32 HTTOPRIGHT = 14;

            const int RESIZE_HANDLE_SIZE = 10;
            bool handled = false;
            if (m.Msg == WM_NCHITTEST || m.Msg == WM_MOUSEMOVE)
            {
                Size formSize = this.Size;
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point clientPoint = this.PointToClient(screenPoint);

                Dictionary<UInt32, Rectangle> boxes = new Dictionary<UInt32, Rectangle>() {
            {HTBOTTOMLEFT, new Rectangle(0, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTBOTTOM, new Rectangle(RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTBOTTOMRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE)},
            {HTTOPRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTTOP, new Rectangle(RESIZE_HANDLE_SIZE, 0, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTTOPLEFT, new Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTLEFT, new Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE) }
        };

                foreach (KeyValuePair<UInt32, Rectangle> hitBox in boxes)
                {
                    if (hitBox.Value.Contains(clientPoint))
                    {
                        m.Result = (IntPtr)hitBox.Key;
                        handled = true;
                        break;
                    }
                }
            }

            if (!handled)
                base.WndProc(ref m);
        }
    }
}
