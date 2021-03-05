using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSource;
using System.Data.OleDb;
using System.Windows.Forms;
using LibraryCommon;
using LibraryCore;

namespace PluginDataSourceAccess
{
    /// <summary>
    /// Содержит методы для чтения и записи из базы данных MS Access.
    /// </summary>
    public class DataSourceAccess : IDataSource
    {
        EventHandler<EventArgsString> _onError;
        IDataBase _db = new DataBase();
        private string _connectString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\..\DataBases\Books.mdb;";
        /// <summary>
        /// Содержит название плагина.
        /// </summary>
        public string NamePlugin { get { return "PluginDataSourceAccess"; } }
        /// <summary>
        /// Содержит описание плагина.
        /// </summary>
        public string DescriptionPlugin { get { return "Плагин для записи и чтения из MS Access"; } }

        public DataSourceAccess() { }
        public DataSourceAccess(IDataBase db)
        {
            this._db = db;
        }
        /// <summary>
        /// Запись в базу данных MS Access.
        /// </summary>
        /// <param name="listOfBooks">Список книг типа <c>List<List<string>></c>.</param>
        public void WriteBooks(List<List<string>> listOfBooks)
        {
            ClearData();
            try
            {
                _db.OpenConnection(_connectString);
                int i = 1;
                foreach (List<string> Book in listOfBooks)
                {
                    _db.Modify($"INSERT INTO Books (Id, Author, Title, ISDN, Price) VALUES ({i},'{Book[0]}','{Book[1]}','{Book[2]}',{Convert.ToDecimal(Book[3])})");
                    i++;
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно сохранить книги в базе данных - прерывание по исключению:" + "\n" + ex.Message));
            }
            _db?.CloseConnection();
        }
        /// <summary>
        /// Чтение из базы данных MS Access.
        /// </summary>
        /// <returns>
        /// Список книг типа <c>List<List<string>></c>.
        /// </returns>
        public List<List<string>> ReadBooks()
        {
            List <List<string>> listOfBooks = new List<List<string>>();

            try
            {
                _db.OpenConnection(_connectString);
                List<string> bookAttributes = _db.Retrieve("SELECT * FROM Books");
                List<string> book = new List<string>();
                int k = 0;
                for (int i = 1; i <= bookAttributes.Count; i++)
                {
                    if (i % 5 == 0)
                        continue;
                    book.Add(bookAttributes[i]);
                    k++;
                    if (k == 4)
                    {
                        k = 0;
                        listOfBooks.Add(book);
                        book = new List<string>();
                    }
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно загрузить книги из базы данных - прерывание по исключению:" + "\n" + ex.Message));
            }
            _db?.CloseConnection();
            return listOfBooks; 
        }

        private void ClearData()
        {

            try
            {
                _db.OpenConnection(_connectString);
                _db.Modify("DELETE FROM Books");
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно очистить базу данных - прерывание по исключению:" + "\n" + ex.Message));
            }
            _db?.CloseConnection();
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
