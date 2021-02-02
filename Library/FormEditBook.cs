using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataSource;
using LibraryCommon;
using LibraryCore;

namespace Library
{
    /// <summary>
    /// Класс <c>FormEditBook</c> окна редактирования.
    /// </summary>
    public partial class FormEditBook : Form
    {
        EventHandler<EventArgsString> _onError;
        private Book _book;
        /// <summary>
        /// Инизиализирует компоненты формы <c>FormEditBook</c> и задает значения textBox.Text и numericUpDown.Value.
        /// </summary>
        /// <param name="book">Экземпляр класса <c>Book</c>.</param>
        public FormEditBook(Book book)
        {
            InitializeComponent();
            this._book = book;
            textBoxAuthor.Text = _book.Author;
            textBoxTitle.Text = _book.Title;
            textBoxISDN.Text = _book.ISDN;
            numericUpDownPrice.Value = _book.Price;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveInformation();
        }

        private void SaveInformation()
        {
            try
            {
                _book.Author = textBoxAuthor.Text;
                _book.Title = textBoxTitle.Text;
                _book.ISDN = textBoxISDN.Text;
                _book.Price = numericUpDownPrice.Value;
                if (string.IsNullOrWhiteSpace(textBoxAuthor.Text) || string.IsNullOrWhiteSpace(textBoxTitle.Text) || string.IsNullOrWhiteSpace(textBoxISDN.Text) || numericUpDownPrice.Value == 0)
                {
                    MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.Close();
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно сохранить информацию о книге - прерывание по исключению:" + "\n" + ex.Message));
            }
        }
        /// <summary>
        /// Событие - ошибка с передачей строки.
        /// </summary>
        public event EventHandler<EventArgsString> OnError
        {
            add { _onError += value; }
            remove { _onError -= value; }
        } 
    }
}
