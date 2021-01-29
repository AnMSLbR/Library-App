using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCommon
{
    /// <summary>
    /// Строка в параметре события.
    /// </summary>
    public class EventArgsString : EventArgs
    {
        private string _message;
        /// <summary>
        /// Задает значение поля <c>_message</c>.
        /// </summary>
        /// <param name="s">Строка с текстом сообщения.</param>
        public EventArgsString(string s)
        {
            _message = s;
        }
        /// <summary>
        /// Задает значение и возвращает поля <c>_message</c>.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
