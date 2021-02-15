using System;
using DataSource;
using System.Data.OleDb;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginDataSourceAccess;

namespace Library.Tests
{
    [TestClass]
    public class DataSourceAccessTests
    {
        private List<List<string>> _listOfBooks;
        private string _connectString;
        private DataSourceAccess _plugin;

        [TestInitialize]
        public void TestInitialize()
        {
            _connectString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\..\DataBases\Books.mdb;";
            _plugin = new DataSourceAccess();

        }

        [TestMethod]
        public void CheckPluginAccess()
        {
            FillListOfBooks();
            CheckResultWriteBooks();
            CheckResultReadBooks();
            DeleteRecordFromDB();
        }

        private void FillListOfBooks()
        {
            _listOfBooks = new List<List<string>>();
            List<string> book1 = new List<string>() { "Бек К.", "Экстремальное программирование. Разработка через тестирование", "8-5566-498-55", "355" };
            List<string> book2 = new List<string>() { "Мартин Р.", "Чистый код", "7-325-4632-643", "459" };
            _listOfBooks.Add(book1);
            _listOfBooks.Add(book2);
        }

        private void CheckResultWriteBooks()
        {
            _plugin.WriteBooks(_listOfBooks);

            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                dbConnection.Open();
                string selectQuery = $"SELECT TOP {_listOfBooks.Count} * FROM Books ORDER BY ID DESC";
                OleDbCommand selectCommand = new OleDbCommand(selectQuery, dbConnection);
                OleDbDataReader reader = selectCommand.ExecuteReader();
                int i = _listOfBooks.Count - 1;
                while (reader.Read())
                {
                    if (reader["Author"].ToString() != _listOfBooks[i][0])
                        Assert.Fail($"Ошибка записи: Поле 'Автор' {0}-й книги сохранено некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    if (reader["Title"].ToString() != _listOfBooks[i][1])
                        Assert.Fail($"Ошибка записи: Поле 'Название' {0}-й книги сохранено некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    if (reader["ISDN"].ToString() != _listOfBooks[i][2])
                        Assert.Fail($"Ошибка записи: Поле 'ISDN' {0}-й книги сохранено некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    if (reader["Price"].ToString() != _listOfBooks[i][3])
                        Assert.Fail($"Ошибка записи: Поле 'Цена' {0}-й книги сохранено некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    i--;
                }
            }
        }

        private void CheckResultReadBooks()
        {
            _listOfBooks.Clear();
            _listOfBooks = _plugin.ReadBooks();

            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                dbConnection.Open();
                string selectQuery = $"SELECT TOP {_listOfBooks.Count} * FROM Books ORDER BY ID DESC";
                OleDbCommand selectCommand = new OleDbCommand(selectQuery, dbConnection);
                OleDbDataReader reader = selectCommand.ExecuteReader();
                int i = _listOfBooks.Count - 1;
                while (reader.Read())
                {
                    if (reader["Author"].ToString() != _listOfBooks[i][0])
                        Assert.Fail($"Ошибка чтения: Поле 'Автор' {0}-й книги прочитано некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    if (reader["Title"].ToString() != _listOfBooks[i][1])
                        Assert.Fail($"Ошибка чтения: Поле 'Название' {0}-й книги прочитано некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    if (reader["ISDN"].ToString() != _listOfBooks[i][2])
                        Assert.Fail($"Ошибка чтения: Поле 'ISDN' {0}-й книги прочитано некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    if (reader["Price"].ToString() != _listOfBooks[i][3])
                        Assert.Fail($"Ошибка чтение: Поле 'Цена' {0}-й книги прочитано некорректно", i + 1);
                    else
                        Assert.IsTrue(true);
                    i--;
                }
            }
        }

        private void DeleteRecordFromDB()
        {
            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                dbConnection.Open();
                for (int i = 0; i < _listOfBooks.Count; i++)
                {
                    string deleteQuery = "DELETE FROM Books WHERE ID = (SELECT MAX(ID) FROM Books)";
                    OleDbCommand deleteCommand = new OleDbCommand(deleteQuery, dbConnection);
                    deleteCommand.ExecuteNonQuery();
                }

            }
        }
    }
}