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
            CheckResultWriteBooksFake01();
            CheckResultWriteBooksFake02();
            CheckResultThrowingExceptionWriteBooksFake();
            CheckResultReadBooksFake();
            CheckResultThrowingExceptionReadBooksFake();
        }
        // case: count of books in list (= 2) <= count of records in db (= 4)
        private void CheckResultWriteBooksFake01()
        {
            var mock = new Mock<IDataBase>();
            mock.Setup(x => x.Retrieve("SELECT COUNT(Id) FROM Books"))
                .Returns(new List<string>() { "4" });

            mock.Setup(x => x.Retrieve($"SELECT * FROM Books WHERE [Id] = {1} AND [Author] = '{_listOfBooks[0][0]}' " +
                 $"AND [Title] = '{_listOfBooks[0][1]}' AND [ISDN] = '{_listOfBooks[0][2]}' AND [Price] = {Convert.ToDecimal(_listOfBooks[0][3])}"))
                 .Returns(_listOfBooks[0]);

            mock.Setup(x => x.Retrieve($"SELECT * FROM Books WHERE [Id] = {2} AND [Author] = '{_listOfBooks[1][0]}' " +
                 $"AND [Title] = '{_listOfBooks[1][1]}' AND [ISDN] = '{_listOfBooks[1][2]}' AND [Price] = {Convert.ToDecimal(_listOfBooks[1][3])}"))
                 .Returns(new List<string>());
            mock.Setup(x => x.Modify($"UPDATE Books SET [Author] = '{_listOfBooks[1][0]}', [Title] = '{_listOfBooks[1][1]}', [ISDN] = '{_listOfBooks[1][2]}'," +
                 $" [Price] = {Convert.ToDecimal(_listOfBooks[1][3])} WHERE [Id] = {2}"));

            mock.Setup(x => x.Modify($"DELETE * FROM Books WHERE Id = (SELECT MAX(Id) FROM Books)"));

            _plugin = new DataSourceAccess(mock.Object);

            _plugin.WriteBooks(_listOfBooks);

            mock.VerifyAll();
            mock.Verify(x => x.Modify(It.IsAny<string>()), Times.Exactly(3));
            mock.Verify(x => x.Retrieve(It.IsAny<string>()), Times.Exactly(3));
        }

        // case: count of books in list (= 3) > count of records in db (= 2)
        private void CheckResultWriteBooksFake02()
        {
            _listOfBooks.Clear();
            FillListOfBooks();
            _listOfBooks.Add(new List<string>() { "Мюллер Д.П.", "C# для чайников", "4-657-4982-77", "324" });
            var mock = new Mock<IDataBase>();
            mock.Setup(x => x.Retrieve("SELECT COUNT(Id) FROM Books"))
                .Returns(new List<string>() { "2" });

            mock.Setup(x => x.Retrieve($"SELECT * FROM Books WHERE [Id] = {1} AND [Author] = '{_listOfBooks[0][0]}' " +
                 $"AND [Title] = '{_listOfBooks[0][1]}' AND [ISDN] = '{_listOfBooks[0][2]}' AND [Price] = {Convert.ToDecimal(_listOfBooks[0][3])}"))
                 .Returns(new List<string>());
            mock.Setup(x => x.Modify($"UPDATE Books SET [Author] = '{_listOfBooks[0][0]}', [Title] = '{_listOfBooks[0][1]}'," +
                 $" [ISDN] = '{_listOfBooks[0][2]}', [Price] = {Convert.ToDecimal(_listOfBooks[0][3])} WHERE [Id] = {1}"));

            mock.Setup(x => x.Retrieve($"SELECT * FROM Books WHERE [Id] = {2} AND [Author] = '{_listOfBooks[1][0]}' " +
                 $"AND [Title] = '{_listOfBooks[1][1]}' AND [ISDN] = '{_listOfBooks[1][2]}' AND [Price] = {Convert.ToDecimal(_listOfBooks[1][3])}"))
                 .Returns(_listOfBooks[1]);

            mock.Setup(x => x.Retrieve($"SELECT * FROM Books WHERE [Id] = {3} AND [Author] = '{_listOfBooks[2][0]}' " +
                 $"AND [Title] = '{_listOfBooks[2][1]}' AND [ISDN] = '{_listOfBooks[2][2]}' AND [Price] = {Convert.ToDecimal(_listOfBooks[2][3])}"))
                 .Returns(new List<string>());
            mock.Setup(x => x.Modify($"INSERT INTO Books (Id, Author, Title, ISDN, Price)" +
                 $" VALUES ({3}, '{_listOfBooks[2][0]}', '{_listOfBooks[2][1]}', '{_listOfBooks[2][2]}', {Convert.ToDecimal(_listOfBooks[2][3])})"));

            _plugin = new DataSourceAccess(mock.Object);

            _plugin.WriteBooks(_listOfBooks);

            mock.VerifyAll();
            mock.Verify(x => x.Modify(It.IsAny<string>()), Times.Exactly(2));
            mock.Verify(x => x.Retrieve(It.IsAny<string>()), Times.Exactly(4));
        }

        private void CheckResultThrowingExceptionWriteBooksFake()
        {
            NullReferenceException ex = new NullReferenceException();
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
            _listOfBooks.Clear();
            FillListOfBooks();
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