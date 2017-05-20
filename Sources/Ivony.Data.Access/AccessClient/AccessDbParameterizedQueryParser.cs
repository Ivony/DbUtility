using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data;
using Ivony.Data.Common;

namespace Ivony.Data.Access.AccessClient
{
    public class AccessDbParameterizedQueryParser:ParameterizedQueryParser<OleDbCommand, OleDbParameter>
    {
        protected override string GetParameterPlaceholder(object value, int index, out OleDbParameter parameter)
        {
            var name = "?";
            parameter = new OleDbParameter(name, value);

            return name;
        }

        protected override OleDbCommand CreateCommand(string commandText, OleDbParameter[] parameters)
        {
            var command = new OleDbCommand(commandText);
            command.Parameters.AddRange(parameters);
            
            return command;
        }
    }
}
