using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DataSource;
using LibraryCommon;
using LibraryCore;


namespace Library
{
    /// <summary>
    /// Класс пользовательского элемента управления <c>UCLibrary</c>.
    /// </summary>
    public partial class UCLibrary : UserControl
    {
        Books _books = new Books();
        bool _checkChanges = false;
        IDataSource _plugin;
        
        /// <summary>
        /// Инициализирует компоненты пользовательского элемента управления <c>UCLibrary</c>.
        /// </summary>
        public UCLibrary()
        {
            InitializeComponent();
            ClearInformation();
            LoadPluginSavedInConfig();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            CreateBook();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditBook();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteBook();
        }

        private void labelLoad_Click(object sender, EventArgs e)
        {
            UpdateListView();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                UpdateInformation(_books.BooksList[listView.SelectedIndices[0]]);
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void loadPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFormLoadPlugin();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void labelLoad_MouseEnter(object sender, EventArgs e)
        {
            labelLoad.ForeColor = Color.DimGray;
        }

        private void labelLoad_MouseLeave(object sender, EventArgs e)
        {
            labelLoad.ForeColor = Color.Gainsboro;
        }
        
        private void helpToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            helpToolStripMenuItem.ForeColor = Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
        }

        private void helpToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            helpToolStripMenuItem.ForeColor = Color.Gainsboro;
        }

        private void Exit()
        {
            if (_checkChanges == true)
            {               
                DialogResult result = MessageBox.Show("Сохранить изменения?", "Сохранить", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (_plugin == null)
                    {
                        MessageBox.Show("Сохранение невозможно, подключите плагин", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        OpenFormLoadPlugin();
                    }
                    else
                    {
                        SaveChanges();
                        Application.Exit();
                    }
                }
                else if (result == DialogResult.No)
                    Application.Exit();
            }
            else
                Application.Exit();
        }

        private void OpenFormLoadPlugin()
        {
            FormLoadPlugin loadPlugin = new FormLoadPlugin(_plugin);
            loadPlugin.OnError += new EventHandler<EventArgsString>(catchError);
            DialogResult result = loadPlugin.ShowDialog();
            _plugin = loadPlugin.Plugin;
            if (_plugin != null)
            {
                labelLoad.Enabled = true;
                LoadFromFile();
                UpdateListView();
            }
        }

        private void SaveChanges()
        {
             _plugin.WriteBooks(Books.BooksToList(_books));        
        }

        private void CreateBook()
        {
            listView.SelectedItems.Clear();
            Book book = new Book();
            FormEditBook editForm = new FormEditBook(book);
            editForm.OnError += new EventHandler<EventArgsString>(catchError);
            DialogResult result = editForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                _books.BooksList.Add(book);
                AddBookToListView(book, listView);
                UpdateInformation(_books.BooksList[_books.BooksList.Count - 1]);
                _checkChanges = true;
            }
        }

        private void EditBook()
        {
            FormEditBook editForm = new FormEditBook(_books.BooksList[listView.SelectedIndices[0]]);
            editForm.OnError += new EventHandler<EventArgsString>(catchError);
            DialogResult result = editForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                ListViewItem _listViewItem = new ListViewItem(_books.BooksList[listView.SelectedIndices[0]].Author, _books.BooksList[listView.SelectedIndices[0]].Title);
                listView.Items[listView.SelectedIndices[0]].Text = _listViewItem.ToString();
                UpdateInformation(_books.BooksList[listView.SelectedIndices[0]]);
                _checkChanges = true;
            }
        }

        private void DeleteBook()
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите удалить книгу?", "Удалить", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _books.BooksList.Remove(_books.BooksList[listView.SelectedIndices[0]]);
                listView.SelectedItems[0].Remove();
                ClearInformation();
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                _checkChanges = true;
            }
        }

        private void LoadPluginSavedInConfig()
        {
            if (ConfigurationManager.AppSettings["pluginSelected"] != " ")
            {
                FormLoadPlugin loadPlugin = new FormLoadPlugin(_plugin);
                loadPlugin.OnError += new EventHandler<EventArgsString>(catchError);
                _plugin = loadPlugin.Plugin;
                labelLoad.Enabled = true;
                LoadFromFile();
                UpdateListView();

            }
        }
        
        private void UpdateListView()
        {
            listView.Clear();
            ClearInformation();
            _checkChanges = false;
            _books.BooksList.Clear();
            _books = LoadFromFile();
            foreach (Book book in _books.BooksList)
            {
                AddBookToListView(book, listView);
            }
        }

        private void AddBookToListView(Book book, ListView list)
        {
            ListViewItem listViewItem = new ListViewItem(book.Author, book.Title);
            list.Items.Add(listViewItem.ToString());
        }

        private Books LoadFromFile()
        {
            return _books.ListToBooks(_plugin.ReadBooks()); 
        }

        private void ClearInformation()
        {
            textAuthor.Text = string.Empty;
            textTitle.Text = string.Empty;
            textISDN.Text = string.Empty;
            textPrice.Text = string.Empty;
        }

        private void UpdateInformation(Book book)
        {
            textAuthor.Text = book.Author;
            textTitle.Text = book.Title;
            textISDN.Text = book.ISDN;
            textPrice.Text = Convert.ToString(book.Price);
        }

        /// <summary>
        /// Выводит на экран сообщение с информацией об исключении.
        /// </summary>
        /// <param name="sender">Объект типа <c>object</c>.</param>
        /// <param name="e">Объект типа <c>EventArgsString</c>.</param>
        private void catchError(object sender, EventArgsString e)
        {
            MessageBox.Show(e.Message);
        }

    }
}
