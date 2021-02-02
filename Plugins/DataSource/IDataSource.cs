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


        void WriteBooks(List<List<string>> listOfBooks);
        List<List<string>> ReadBooks();
        event EventHandler<EventArgsString> OnError;
        
        
    }
}
