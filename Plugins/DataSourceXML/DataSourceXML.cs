using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DataSource;
using System.Xml.Serialization;
using System.IO;
using LibraryCommon;
using LibraryCore;

namespace PluginDataSourceXML
{
    /// <summary>
    /// Содержит методы для записи и чтения из XML файла.
    /// </summary>
    public class DataSourceXML : IDataSource
    {
        EventHandler<EventArgsString> _onError;
        /// <summary>
        /// Содержит название плагина.
        /// </summary>
        public string NamePlugin { get { return "PluginDataSourceXML"; } }
        /// <summary>
        /// Содержит описание плагина.
        /// </summary>
        public string DescriptionPlugin { get { return "Плагин для записи и чтения XML файлов"; } }
        /// <summary>
        /// Запись в XML файл.
        /// </summary>
        /// <param name="listOfBooks">Список книг типа <c>List<List<string>></c>.</param>
        public bool WriteBooks(List<List<string>> listOfBooks)
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<List<string>>));
                using (FileStream fs = new FileStream(@"..\..\..\DataBases\Books.xml", FileMode.Truncate))
                {
                    xml.Serialize(fs, listOfBooks);
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно сохранить книги в XML файл - прерывание по исключению:" + "\n" + ex.Message));
                return false;
            }
            return true;
        }
        /// <summary>
        /// Обновление списка книг.
        /// </summary>
        /// <param name="listOfBooks"></param>
        public bool UpdateBooks(List<List<string>> listOfBooks)
        {
            return false;
        }
        /// <summary>

        /// Удаление книги
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool DeleteBook(string Id)
        {

            return false;
        }
        /// <summary>
        /// Чтение из XML файла.
        /// </summary>
        /// <returns>
        /// Список книг типа <c>List<List<string>></c>.
        /// </returns>
        public List<List<string>> ReadBooks()
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<List<string>>));
                using (FileStream fs = new FileStream(@"..\..\..\DataBases\Books.xml", FileMode.OpenOrCreate))
                {
                    return (List <List<string>>)xml.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно загрузить книги из XML файла - прерывание по исключению:" + "\n" + ex.Message));
                return new List<List<string>>();
            }
        }
        /// <summary>
        /// Событие - ошибка с передачей строки.
        /// </summary>
        public event EventHandler<EventArgsString> OnError
        {
            add { _onError += value; }
            remove { _onError -= value; }
        }
    }
}
