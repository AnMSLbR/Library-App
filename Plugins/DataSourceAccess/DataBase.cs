using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace PluginDataSourceAccess
{
    class DataBase : IDataBase
    {
        private OleDbConnection _dbConnection;
        public void OpenConnection(string connectString)
        {
            _dbConnection = new OleDbConnection(connectString);
            _dbConnection.Open();
        }

        public int Modify(string insertQuery)
        {
            OleDbCommand insertCommand = new OleDbCommand(insertQuery, _dbConnection);
            return insertCommand.ExecuteNonQuery();
        }

        public List<string> Retrieve(string selectQuery)
        {
            List<string> listOfStrings = new List<string>();
            OleDbCommand selectCommand = new OleDbCommand(selectQuery, _dbConnection);
            OleDbDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    listOfStrings.Add(reader[i].ToString());
                }
            }
            return listOfStrings;
        }

        public void CloseConnection()
        {
            _dbConnection.Close();
        }
    }
}
