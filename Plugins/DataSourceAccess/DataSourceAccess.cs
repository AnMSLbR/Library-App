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

namespace DataSourceAccess
{
    /// <summary>
    /// Содержит методы для чтения и записи из базы данных MS Access.
    /// </summary>
    public class DataSourceAccess : IDataSource
    {
        EventHandler<EventArgsString> _onError;
        
        private string _connectString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\DataBases\Books.mdb;";
        /// <summary>
        /// Содержит название плагина.
        /// </summary>
        public string NamePlugin { get { return "PluginDataSourceAccess"; } }
        /// <summary>
        /// Содержит описание плагина.
        /// </summary>
        public string DescriptionPlugin { get { return "Плагин для записи и чтения из MS Access"; } }
        /// <summary>
        /// Запись в базу данных MS Access.
        /// </summary>
        /// <param name="listOfBooks">Список книг типа <c>List<List<string>></c>.</param>
        public void WriteBooks(List<List<string>> listOfBooks)
        {
            ClearData();
            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                try
                {
                    dbConnection.Open();
                    int i = 1;
                    foreach (List<string> Book in listOfBooks)
                    {
                        string insertQuery = $"INSERT INTO Books (Id, Author, Title, ISDN, Price) VALUES ({i},'{Book[0]}','{Book[1]}','{Book[2]}',{Convert.ToDecimal(Book[3])})";
                        OleDbCommand insertCommand = new OleDbCommand(insertQuery, dbConnection);
                        insertCommand.ExecuteNonQuery();
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(this, new EventArgsString("Невозможно сохранить книги в базе данных - прерывание по исключению:" + "\n" + ex.Message));
                }
            }
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
            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                try
                {
                    dbConnection.Open();
                    string selectQuery = "SELECT * FROM Books";
                    OleDbCommand selectCommand = new OleDbCommand(selectQuery, dbConnection);
                    OleDbDataReader reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        List<string> list = new List<string>() { reader["Author"].ToString(), reader["Title"].ToString(), reader["ISDN"].ToString(), reader["Price"].ToString() };
                        listOfBooks.Add(list);
                    }
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(this, new EventArgsString("Невозможно загрузить книги из базы данных - прерывание по исключению:" + "\n" + ex.Message));
                }
                return listOfBooks;
            }
        }

        private void ClearData()
        {
            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                try
                {
                    dbConnection.Open();
                    string deleteQuery = "DELETE FROM Books";
                    OleDbCommand deleteCommand = new OleDbCommand(deleteQuery, dbConnection);
                    deleteCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(this, new EventArgsString("Невозможно очистить базу данных - прерывание по исключению:" + "\n" + ex.Message));
                }
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
