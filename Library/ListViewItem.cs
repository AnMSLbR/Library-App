using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    class ListViewItem
    {
        /// <value> Содержит значение имени автора. /// </value>
        public string Author { get; set; }
        /// <value> Содержит значение названия книги. /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Конструктор, задающий значения свойств <c>Author</c> и <c>Title</c>.
        /// </summary>
        /// <param name="Author">Строка с именем автора типа string.</param>
        /// <param name="Title">Стркоа с названием книги типа string</param>
        public ListViewItem(string Author, string Title)
        {
            this.Author = Author;
            this.Title = Title;
        }
        /// <summary>
        /// Возвращает строку, содержащую имя автора и название книги.
        /// </summary>
        /// <returns>Строка с именем и названием типа string.</returns>
        public override string ToString()
        {
            return Author + " – " + Title;
        }
    }
}
