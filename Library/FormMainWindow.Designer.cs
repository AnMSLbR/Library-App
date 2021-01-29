namespace Library
{
    partial class FormMainWindow
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainWindow));
            this.ucLibrary1 = new Library.UCLibrary();
            this.SuspendLayout();
            // 
            // ucLibrary1
            // 
            this.ucLibrary1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.ucLibrary1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucLibrary1.Location = new System.Drawing.Point(0, 0);
            this.ucLibrary1.Name = "ucLibrary1";
            this.ucLibrary1.Size = new System.Drawing.Size(800, 600);
            this.ucLibrary1.TabIndex = 0;
            this.ucLibrary1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ucLibrary1_MouseDown);
            this.ucLibrary1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ucLibrary1_MouseMove);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.ucLibrary1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Library";
            this.ResumeLayout(false);

        }

        #endregion

        private UCLibrary ucLibrary1;
    }
}

