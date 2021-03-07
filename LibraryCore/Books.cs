using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace LibraryCore
{
    /// <summary>
    /// Класс <c>Books</c>.
    /// Содержит список объектов типа <c>Book</c>.
    /// </summary>
    [Serializable]
    public class Books
    {
        /// <summary>
        /// Создание списка объектов типа <c>Book</c>.
        /// </summary>
        public List<Book> BooksList { get; set; }

        /// <summary>
        /// Конструктор без аргументов
        /// </summary>
        public Books()
        {
            BooksList = new List<Book>();
        }
        /// <summary>
        /// Конструктор со списком книг в аргументе.
        /// </summary>
        /// <param name="listBooks"></param>
        public Books(List<Book> listBooks)
        {
             BooksList = listBooks;
        }
        /// <summary>
        /// Преобразует список элементов <c>Book</c> и значения их полей в объекте класса <c>Books</c> в строки.
        /// </summary>
        /// <param name="books">Экземпляр класса <c>Books</c>.</param>
        /// <returns>Список книг типа <c>List<List<string>></c>.</returns>
        public static List<List<string>> BooksToList(Books books)
        {
            List<List<string>> listOfBooks = new List<List<string>>();

            foreach (Book book in books.BooksList)
            {
                List<string> list = new List<string>() { book.Id, book.Author, book.Title, book.ISDN, book.Price.ToString() };
                listOfBooks.Add(list);
            }
            return listOfBooks;
        }
        /// <summary>
        /// Преобразует список строк в список элементов <c>Book</c> и значения их полей в объекте класса <c>Books</c>.
        /// </summary>
        /// <param name="list">Список книг типа <c>List<List<string>></c>.</param>
        /// <returns>Экземляр класса <c>Books</c>.</returns>
        public Books ListToBooks(List<List<string>> list)
        {
            Books books = new Books();
            foreach (List<string> item in list)
            {
                Book book = new Book();
                book.Id = item[0];
                book.Author = item[1];
                book.Title = item[2];
                book.ISDN = item[3];
                book.Price = item[4];
                books.BooksList.Add(book);
            }
            return books;
        }

    }
    /// <summary>
    /// Класс <c>Book</c>.
    /// Содержит свойства для значений имени автора, названия, ISDN, цены книги.
    /// </summary>
    [Serializable]
    public class Book
    {
        bool _change;
        /// <value> Идентификатор книги. /// </value>
        public string Id { get; set; }
        /// <value> Содержит значение имени автора. /// </value>
        public string Author { get; set; }
        /// <value> Содержит значение названия книги. /// </value>
        public string Title { get; set; }
        /// <value> Содержит значение ISDN книги. /// </value>
        public string ISDN { get; set; }
        /// <value> Содержит значение цены книги. /// </value>
        public string Price { get; set; }

        /// <summary>
        /// Признак изменения
        /// </summary>
        public bool IsChange { get { return _change; } }
        /// <summary>
        /// Стандартный конструктор без параметров, т.к. класс подлежит сериализации.
        /// </summary>
        public Book() { }
        /// <summary>
        /// Конструктор, принимающий в качестве параметров значения для имени автора, названия, ISDN, цены книги.
        /// </summary>
        /// <param name="Author">Строка с именем автора типа string.</param>
        /// <param name="Title">Строка с названием типа string.</param>
        /// <param name="ISDN">Строка с ISDN типа string.</param>
        /// <param name="Price">Число с плавающей запятой для цены типа decimal.</param>
        public Book(string Author, string Title, string ISDN, string Price)
        {
            this.Author = Author;
            this.Title = Title;
            this.ISDN = ISDN;
            this.Price = Price;
        }

        /// <summary>
        /// Проверка на изменения
        /// </summary>
        /// <param name="author"></param>
        /// <param name="title"></param>
        /// <param name="isdn"></param>
        /// <param name="price"></param>
        public void CheckChange(string author, string title, string isdn, string price)
        {
            if (Author != author || Title != title || ISDN != isdn || Price != price)
                _change = true;
            else
                _change = false;
        }
    }



}
