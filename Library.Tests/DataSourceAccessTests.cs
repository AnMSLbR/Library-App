using System;
using DataSource;
using System.Data.OleDb;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginDataSourceAccess;
using Moq;
using LibraryCommon;

namespace Library.Tests
{
    [TestClass]
    public class DataSourceAccessTests
    {
        private List<List<string>> _listOfBooks;
        private string _connectString;
        private DataSourceAccess _plugin;
        IDataBase _db = null;
        private List<List<string>> _dbData;

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
            SaveDatabaseData();
            CheckResultWriteBooks();
            CheckResultReadBooks();
            DeleteRecordFromDB();
            RestoreDatabaseData();
            CheckResultThrowingExceptionWriteBooksComplete();
            CheckResultThrowingExceptionReadBooksComplete();
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

        private void FillIncorrectListOfBooks()
        {
            _listOfBooks.Clear();
            _listOfBooks = new List<List<string>>();
            List<string> book1 = new List<string>() { "Мартин Р.", "Чистая архитектура", "7-325-1342-93" };
            List<string> book2 = new List<string>() { "Экберг Ф.", "C# Smorgasbord", "4-82-43654-573", "654", "21.04.2007" };
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

        private void CheckResultThrowingExceptionWriteBooksComplete()
        {
            FillIncorrectListOfBooks();
            string message = "Невозможно сохранить книги в базе данных - прерывание по исключению:" + "\n" + "Индекс за пределами диапазона. Индекс должен быть положительным числом, а его размер не должен превышать размер коллекции." + "\r\n" + "Имя параметра: index";
            EventArgsString mess = null;
            _plugin.OnError += delegate (object sender, EventArgsString e)
            {
                mess = e;
            };
            _plugin.WriteBooks(_listOfBooks);
            Assert.IsNotNull(mess, "Событие не вызвано");
            Assert.AreEqual(message, mess.Message, $"Ожидается сообщение: \"{message}\";" + "\n" + $"Вызвано сообщение: \"{mess.Message}\"");
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

        private void CheckResultThrowingExceptionReadBooksComplete()
        {
            FillIncorrectListOfBooks();
            _plugin = new DataSourceAccess(_db);
            string message = "Невозможно загрузить книги из базы данных - прерывание по исключению:" + "\n" + "Ссылка на объект не указывает на экземпляр объекта.";
            EventArgsString mess = null;
            _plugin.OnError += delegate (object sender, EventArgsString e)
            {
                mess = e;
            };
            _plugin.ReadBooks();
            Assert.IsNotNull(mess, "Событие не вызвано");
            Assert.AreEqual(message, mess.Message, $"Ожидается сообщение: \"{message}\";" + "\n" + $"Вызвано сообщение: \"{mess.Message}\"");
        }

        private void SaveDatabaseData()
        {
            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                dbConnection.Open();
                _dbData = new List<List<string>>();
                List<string> book = new List<string>();
                OleDbCommand selectCommand = new OleDbCommand("SELECT * FROM Books", dbConnection);
                OleDbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    book = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        book.Add(reader[i].ToString());
                    }
                    _dbData.Add(book);
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
                    string deleteQuery = "DELETE * FROM Books WHERE ID = (SELECT MAX(ID) FROM Books)";
                    OleDbCommand deleteCommand = new OleDbCommand(deleteQuery, dbConnection);
                    deleteCommand.ExecuteNonQuery();
                }

            }
        }

        private void RestoreDatabaseData()
        {
            using (OleDbConnection dbConnection = new OleDbConnection(_connectString))
            {
                dbConnection.Open();
                int i = 1;
                foreach (List<string> Book in _dbData)
                {
                    string insertQuery = $"INSERT INTO Books (Id, Author, Title, ISDN, Price) VALUES ({i},'{Book[1]}','{Book[2]}','{Book[3]}',{Convert.ToDecimal(Book[4])})";
                    OleDbCommand insertCommand = new OleDbCommand(insertQuery, dbConnection);
                    insertCommand.ExecuteNonQuery();
                    i++;
                }
            }
        }

        [TestMethod]
        public void CheckPluginAccessFake()
        {
            CheckResultWriteBooksFake();
            CheckResultThrowingExceptionWriteBooksFake();
            CheckResultReadBooksFake();
            CheckResultThrowingExceptionReadBooksFake();
        }

        private void CheckResultWriteBooksFake()
        {
            var mock = new Mock<IDataBase>();
            List<string> listOfInserts = new List<string>();
            for (int i = 0; i < _listOfBooks.Count; i++)
                listOfInserts.Add($"INSERT INTO Books (Id, Author, Title, ISDN, Price) VALUES ({i + 1},'{_listOfBooks[i][0]}','{_listOfBooks[i][1]}'," +
                    $"'{_listOfBooks[i][2]}',{Convert.ToDecimal(_listOfBooks[i][3])})");
            foreach (string insertQuery in listOfInserts)
                mock.Setup(x => x.Modify(insertQuery));

            _plugin = new DataSourceAccess(mock.Object);

            _plugin.WriteBooks(_listOfBooks);

            mock.VerifyAll();
        }

        private void CheckResultThrowingExceptionWriteBooksFake()
        {
            Exception ex = new Exception();
            string message = "Невозможно сохранить книги в базе данных - прерывание по исключению:" + "\n" + $"{ex.Message}";
            var dbMock = new Mock<IDataBase>();
            dbMock.Setup(x => x.Modify(It.IsAny<string>())).Throws(ex);
            var pluginMock = new Mock<DataSourceAccess>(dbMock.Object);
            EventArgsString mess = null;
            pluginMock.Object.OnError += delegate (object sender, EventArgsString e)
            {
                mess = e;
            };

            pluginMock.Object.WriteBooks(_listOfBooks);

            Assert.IsNotNull(mess, "Событие не вызвано");
            Assert.AreEqual(mess.Message, message, $"Ожидается сообщение: \"{message}\";" + "\n" + $"Вызвано сообщение: \"{mess.Message}\"");
        }

            private void CheckResultReadBooksFake()
        {
            var mock = new Mock<IDataBase>();
            mock.Setup(x => x.Retrieve("SELECT * FROM Books"))
                .Returns(ReturnListOfStrings());

            _plugin = new DataSourceAccess(mock.Object);
            var expected = _listOfBooks;

            var actual = _plugin.ReadBooks();

            Assert.IsTrue(actual != null, "Ошибка чтения: Метод возвращает 'null'");
            Assert.AreEqual(expected.Count, actual.Count, "Ошибка чтения: Возвращено неправильное число книг");
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.IsTrue(expected[i][0] == actual[i][0], ($"Ошибка чтения: Поле 'Автор' {i + 1}-й книги прочитано некорректно"));
                Assert.IsTrue(expected[i][1] == actual[i][1], ($"Ошибка чтения: Поле 'Название' {i + 1}-й книги прочитано некорректно"));
                Assert.IsTrue(expected[i][2] == actual[i][2], ($"Ошибка чтения: Поле 'ISDN' {i + 1}-й книги прочитано некорректно"));
                Assert.IsTrue(expected[i][3] == actual[i][3], ($"Ошибка чтения: Поле 'Цена' {i + 1}-й книги прочитано некорректно"));
            }
        }

        private void CheckResultThrowingExceptionReadBooksFake()
        {
            Exception ex = new Exception();
            string message = "Невозможно загрузить книги из базы данных - прерывание по исключению:" + "\n" + $"{ex.Message}";
            var dbMock = new Mock<IDataBase>();
            dbMock.Setup(x => x.Retrieve("SELECT * FROM Books"))
                .Throws(ex);

            var pluginMock = new Mock<DataSourceAccess>(dbMock.Object);
            EventArgsString mess = null;
            pluginMock.Object.OnError += delegate (object sender, EventArgsString e)
            {
                mess = e;
            };

            pluginMock.Object.ReadBooks();

            Assert.IsNotNull(mess, "Событие не вызвано");
            Assert.AreEqual(mess.Message, message, $"Ожидается сообщение: \"{message}\";" + "\n" + $"Вызвано сообщение: \"{mess.Message}\"");
        }

            private List<string> ReturnListOfStrings()
        {
            List<string> listOfStrings = new List<string>();
            int id = 1;
            foreach (List<string> Book in _listOfBooks)
            {
                listOfStrings.Add($"{id}");
                listOfStrings.AddRange(Book);
                id++;
            }
            return listOfStrings;
        }

    }
}