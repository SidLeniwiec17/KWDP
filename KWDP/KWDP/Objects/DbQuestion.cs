using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDP.Objects
{
    public class DbQuestion
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }

        public DbQuestion()
        {

        }

        public DbQuestion(string cont, string desc, int type)
        {            
            Content = cont;
            Description = desc;
            Type = type;
        }

        internal string ToSqlString()
        {
            string values = "";
            values += "'" + Content + "', ";
            values += "'" + Description + "', ";
            values += Type;
            return values;
        }
    }
}
