using System;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace SmallTaskList.App
{
    public partial class Form1 : Form
    {
        private readonly string _startUrl;
        private ChromiumWebBrowser _browser;

        public Form1(string startUrl)
        {
            _startUrl = startUrl ?? "about:blank";
            InitializeComponent();
            InitializeBrowser();
        }

        /// <summary>
        /// Parameterless constructor for the designer only; browser will show about:blank.
        /// </summary>
        public Form1() : this("about:blank")
        {
        }

        private void InitializeBrowser()
        {
            if (DesignMode)
                return;
            _browser = new ChromiumWebBrowser(_startUrl)
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(_browser);
        }
    }
}
