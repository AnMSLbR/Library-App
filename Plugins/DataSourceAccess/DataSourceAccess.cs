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
        IDataBase _db = new DataBase();
        private string _connectString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\..\DataBases\Books.mdb;";
        public DataSourceAccess() { }


        public DataSourceAccess(IDataBase db)
        {
            this._db = db;
        }

        private OleDbConnection OpenConnectDB()
        {

            OleDbConnection con = new OleDbConnection(_connectString);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new EventArgsString("При открытии соединения к БД Access вызвано исключение\n" + ex.Message));
                return null;
            }
            return con;
        }

        #region Реализация интерфейса IDataSource
        /// <summary>
        /// Содержит название плагина.
        /// </summary>
        public string NamePlugin { get { return "PluginDataSourceAccess"; } }

        /// <summary>
        /// Содержит описание плагина.
        /// </summary>
        public string DescriptionPlugin { get { return "Плагин для записи и чтения из MS Access"; } }

        /// <summary>
        /// Запись (добавление) списка книг
        /// </summary>
        /// <param name="listOfBooks"></param>
        /// <returns></returns>
        public bool WriteBooks(List<List<string>> listOfBooks)
        {
            using (OleDbConnection con = OpenConnectDB())
            {
                try
                {
                    OleDbCommand com = con.CreateCommand();
                    foreach (List<string> item in listOfBooks)
                    {
                        com.CommandText = "INSERT INTO Books (Author, Title, ISDN, Price) VALUES (@Author, @Title, @ISDN, @Price)";

                        com.Parameters.AddWithValue("@Author", item[0]);
                        com.Parameters.AddWithValue("@Title", item[1]);
                        com.Parameters.AddWithValue("@ISDN", item[2]);
                        com.Parameters.AddWithValue("@Price", decimal.Parse(item[3]));

                        com.ExecuteNonQuery();
                        com.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new EventArgsString("Невозможно записать книги в базе данных - прерывание по исключению:" + "\n" + ex.Message));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Обновление списка книг.
        /// </summary>
        /// <param name="listOfBooks"></param>
        public bool UpdateBooks(List<List<string>> listOfBooks)
        {
            using (OleDbConnection con = OpenConnectDB())
            {
                try
                {
                    OleDbCommand com = con.CreateCommand();
                    foreach (List<string> item in listOfBooks)
                    {
                        com.CommandText = "UPDATE Books SET Author=?, Title=?, ISDN=?, Price=?" +
                                          " WHERE Id=?";

                        com.Parameters.AddWithValue("Author", item[1]);
                        com.Parameters.AddWithValue("Title", item[2]);
                        com.Parameters.AddWithValue("ISDN", item[3]);
                        com.Parameters.AddWithValue("Price", decimal.Parse(item[4]));
                        com.Parameters.AddWithValue("Id", int.Parse(item[0]));

                        com.ExecuteNonQuery();
                        com.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new EventArgsString("Невозможно обновить книги в базе данных - прерывание по исключению:" + "\n" + ex.Message));
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Чтение из базы данных MS Access.
        /// </summary>
        /// <returns>
        /// Список книг типа <c>List<List<string>></c>.
        /// </returns>
        public List<List<string>> ReadBooks()
        {
            using (OleDbConnection con = OpenConnectDB())
            {
                List<List<string>> listOfBooks = new List<List<string>>();
                try
                {

                    OleDbCommand com = con.CreateCommand();
                    com.CommandText = "SELECT * FROM Books";

                    OleDbDataReader rd = com.ExecuteReader();
                    while (rd.Read())
                    {
                        string id = Convert.ToString(rd[0]);
                        string author = Convert.ToString(rd[1]);
                        string title = Convert.ToString(rd[2]);
                        string isdn = Convert.ToString(rd[3]);
                        string price = Convert.ToString(rd[4]);

                        List<string> book = new List<string>() { id, author, title, isdn, price };
                        listOfBooks.Add(book);
                    }
                    rd.Close();

                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new EventArgsString("Невозможно загрузить книги из базы данных - прерывание по исключению:" + "\n" + ex.Message));
                }
                return listOfBooks;
            }
        }

        /// <summary>
        /// Удаление книги
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool DeleteBook(string Id)
        {
            using (OleDbConnection con = OpenConnectDB())
            {
                try
                {

                    OleDbCommand com = con.CreateCommand();
                    com.CommandText = "DELETE FROM Books WHERE Id = @Id";
                    com.Parameters.AddWithValue("@Id", Id);

                    com.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new EventArgsString("Не удалось удалить книгу с Id = "+ Id +" - прерывание по исключению:" + "\n" + ex.Message));
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Событие - ошибка с передачей строки.
        /// </summary>
        public event EventHandler<EventArgsString> OnError;
        #endregion
    }
}
