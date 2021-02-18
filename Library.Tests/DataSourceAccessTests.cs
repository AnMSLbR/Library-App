using System;
using DataSource;
using System.Data.OleDb;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginDataSourceAccess;
using Moq;

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
            FillListOfBooks();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            _listOfBooks.Clear();
        }

        [TestMethod]
        public void CheckPluginAccessComplete()
        {
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
                    Assert.IsTrue(reader["Author"].ToString() == _listOfBooks[i][0], ($"Ошибка записи: Поле 'Автор' {i + 1}-й книги сохранено некорректно"));
                    Assert.IsTrue(reader["Title"].ToString() == _listOfBooks[i][1], ($"Ошибка записи: Поле 'Название' {i + 1}-й книги сохранено некорректно"));
                    Assert.IsTrue(reader["ISDN"].ToString() == _listOfBooks[i][2], ($"Ошибка записи: Поле 'ISDN' {i + 1}-й книги сохранено некорректно"));
                    Assert.IsTrue(reader["Price"].ToString() == _listOfBooks[i][3], ($"Ошибка записи: Поле 'Цена' {i + 1}-й книги сохранено некорректно"));
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
                    Assert.IsTrue(reader["Author"].ToString() == _listOfBooks[i][0], ($"Ошибка чтения: Поле 'Автор' {i +1}-й книги прочитано некорректно"));
                    Assert.IsTrue(reader["Title"].ToString() == _listOfBooks[i][1], ($"Ошибка чтения: Поле 'Название' {i + 1}-й книги прочитано некорректно"));
                    Assert.IsTrue(reader["ISDN"].ToString() == _listOfBooks[i][2], ($"Ошибка чтения: Поле 'ISDN' {i + 1}-й книги прочитано некорректно"));
                    Assert.IsTrue(reader["Price"].ToString() == _listOfBooks[i][3], ($"Ошибка чтения: Поле 'Цена' {i + 1}-й книги прочитано некорректно"));
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