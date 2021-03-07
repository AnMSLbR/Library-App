using System;
using DataSource;
using System.Data.OleDb;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginDataSourceAccess;
using Moq;
using LibraryCommon;
using System.Linq;

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
         }

        private void _plugin_OnError(object sender, EventArgsString e)
        {
            Assert.Fail(e.Message);
        }


        [TestMethod]
        public void CheckPluginAccessComplete()
        {
            //создаем книги
            List<List<string>> listOfBooks = new List<List<string>>();
            List<string> book1 = new List<string>() { "Бек К.", "Экстремальное программирование. Разработка через тестирование", "8-5566-498-55", "355" };
            List<string> book2 = new List<string>() { "Мартин Р.", "Чистый код", "7-325-4632-643", "459" };
            listOfBooks.Add(book1);
            listOfBooks.Add(book2);

            _plugin.OnError += _plugin_OnError;
            //записываем
            if (!_plugin.WriteBooks(listOfBooks))
            {
                Assert.Fail("Запись книг прошла с ошибкой");
                return;
            }

            //читаем
            List<List<string>> listRead = _plugin.ReadBooks();


            //сравниваем
            List<string> book1Read = listRead.FirstOrDefault(x => x[1] == "Бек К." && x[2] == "Экстремальное программирование. Разработка через тестирование");
            if (book1Read == null)
            {
                Assert.Fail("Не нашлась кника book1");
            }
            List<string> book2Read = listRead.FirstOrDefault(x => x[1] == "Мартин Р." && x[2] == "Чистый код");
            if (book2Read == null)
            {
                Assert.Fail("Не нашлась кника book2");
            }

            //удаляем
            if (!_plugin.DeleteBook(book1Read[0]))
            {
                Assert.Fail("Удаление книги book1 прошло с ошибкой");
                return;
            }

            if (!_plugin.DeleteBook(book2Read[0]))
            {
                Assert.Fail("Удаление книги book2 прошло с ошибкой");
                return;
            }

            //опять читаем
            listRead = _plugin.ReadBooks();

            //опять сравниваем
            book1Read = listRead.FirstOrDefault(x => x[1] == "Бек К." && x[2] == "Экстремальное программирование. Разработка через тестирование");
            if (book1Read != null)
            {
                Assert.Fail("Не удалилась книга book1");
            }
            book2Read = listRead.FirstOrDefault(x => x[1] == "Мартин Р." && x[2] == "Чистый код");
            if (book2Read != null)
            {
                Assert.Fail("Не удалилась книга book2");
            }

            //отписываемся
            _plugin.OnError -= _plugin_OnError;
        }

        [TestMethod]
        public void CheckPluginAccessFake()
        {
            //создаем фейковую книгу с автором==null
            List<List<string>> listOfBooks = new List<List<string>>();
            List<string> book1 = new List<string>() { null, "Экстремальное программирование. Разработка через тестирование", "8-5566-498-55", "355" };
            listOfBooks.Add(book1);

            //записываем
            if (_plugin.WriteBooks(listOfBooks))
            {
                Assert.Fail("Фейковая книга записалась");
                return;
            }
        }
    }
}