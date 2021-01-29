using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataSource;
using LibraryCommon;
using LibraryCore;
using OfficeOpenXml;


namespace DataSourceExcel
{
    /// <summary>
    /// Содержит методы для записи и чтения данных из MS Excel.
    /// </summary>
    public class DataSourceExcel : IDataSource
    {
        EventHandler<EventArgsString> _onError;
        /// <summary>
        /// Содержит название плагина.
        /// </summary>
        public string NamePlugin { get { return "PluginDataSourceExcel"; } }
        /// <summary>
        /// Содержит описание плагина.
        /// </summary>
        public string DescriptionPlugin { get { return "Плагин для записи и чтения данных из MS Excel"; } }

        /// <summary>
        /// Запись в таблицу Excel.
        /// </summary>
        /// <param name="listOfBooks">Список книг типа <c>List<List<string>></c>.</param>
        public void WriteBooks(List<List<string>> listOfBooks)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                try
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Books");
                    worksheet.Cells[1, 1].Value = "Автор";
                    worksheet.Cells[1, 2].Value = "Название";
                    worksheet.Cells[1, 3].Value = "ISDN";
                    worksheet.Cells[1, 4].Value = "Цена";
                    int i = 2;
                    foreach (List<string> list in listOfBooks)
                    {
                        worksheet.Cells[i, 1].Value = list[0];
                        worksheet.Cells[i, 2].Value = list[1];
                        worksheet.Cells[i, 3].Value = list[2];
                        worksheet.Cells[i, 4].Value = Convert.ToDecimal(list[3]);
                        i++;
                    }
                    excelPackage.SaveAs(new FileInfo(@"DataBases\Books.xlsx"));
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(this, new EventArgsString("Невозможно сохранить книги в таблицу Excel - прерывание по исключению:" + "\n" + ex.Message));
                }
            }
        }
        /// <summary>
        /// Чтение из таблицы Excel.
        /// </summary>
        /// <returns>
        /// Список книг типа <c>List<List<string>></c>.
        /// </returns>
        public List<List<string>> ReadBooks()
        {
            List<List<string>> listOfBooks = new List<List<string>>();
            try
            {
                FileInfo file = new FileInfo(@"DataBases\Books.xlsx");
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.First();
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        List<string> list = new List<string>();
                        list.Add(worksheet.Cells[i, 1].Value.ToString());
                        list.Add(worksheet.Cells[i, 2].Value.ToString());
                        list.Add(worksheet.Cells[i, 3].Value.ToString());
                        list.Add(worksheet.Cells[i, 4].Value.ToString());
                        listOfBooks.Add(list);
                    }
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно загрузить книги из таблицы Excel - прерывание по исключению:" + "\n" + ex.Message));
            }
            return listOfBooks;
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
