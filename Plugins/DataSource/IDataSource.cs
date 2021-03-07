using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryCore;
using LibraryCommon;

namespace DataSource
{
    /// <summary>
    /// Интерфейс чтения-записи из источника данных.
    /// </summary>
    public interface IDataSource
    {
        string NamePlugin { get; }
        string DescriptionPlugin { get; }


        /// <summary>
        /// Запись (добавление) списка книг
        /// </summary>
        /// <param name="listOfBooks"></param>
        /// <returns></returns>
        bool WriteBooks(List<List<string>> listOfBooks);

        /// <summary>
        /// Обновление списка книг.
        /// </summary>
        /// <param name="listOfBooks"></param>
        bool UpdateBooks(List<List<string>> listOfBooks);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        bool DeleteBook(string Id);

        List<List<string>> ReadBooks();

        event EventHandler<EventArgsString> OnError;

    }
}
