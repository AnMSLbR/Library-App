using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    /// <summary>
    /// Основной <c>FormMainWindow</c> класс.
    /// </summary>
    public partial class FormMainWindow : Form
    {
        Point _point;
        /// <summary>
        /// Инициализирует компоненты формы <c>FormMainWindow</c>.
        /// </summary>
        public FormMainWindow()
        {
            InitializeComponent();
        }

        private void ucLibrary1_MouseDown(object sender, MouseEventArgs e)
        {
            _point = new Point(e.X, e.Y);
        }

        private void ucLibrary1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - _point.X;
                this.Top += e.Y - _point.Y;
            }
        }
    }
}
