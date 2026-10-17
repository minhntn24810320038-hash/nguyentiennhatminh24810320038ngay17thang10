using System;
using System.Drawing;
using System.Windows.Forms;

namespace BaitapWinForms.Forms
{
    public class FormMenu : Form
    {
        public FormMenu()
        {
            this.Text = "Menu Quản Lý Bài Tập WinForms";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Button btnBai1 = new Button { Text = "Bài 1: Clinic Appointment Manager", Location = new Point(50, 40), Size = new Size(280, 40) };
            Button btnBai2 = new Button { Text = "Bài 2: Lab Equipment Checkout", Location = new Point(50, 100), Size = new Size(280, 40) };
            Button btnBai3 = new Button { Text = "Bài 3: Repair Service Tracker", Location = new Point(50, 160), Size = new Size(280, 40) };

            btnBai1.Click += (s, e) => new FormBai1().ShowDialog();
            btnBai2.Click += (s, e) => new FormBai2().ShowDialog();
            btnBai3.Click += (s, e) => new FormBai3().ShowDialog();

            this.Controls.AddRange(new Control[] { btnBai1, btnBai2, btnBai3 });
        }
    }
}