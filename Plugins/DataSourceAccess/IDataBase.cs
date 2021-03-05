using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDataSourceAccess
{
    public interface IDataBase
    {
        void OpenConnection(string connectString);
        int Modify(string insertQuery);
        List<string> Retrieve(string selectQuery);
        void CloseConnection();
    }
}
