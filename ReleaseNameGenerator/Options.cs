using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseNameGenerator
{
    public class EnvironmentOptions
    {
        public string Enviroment { get; set; }
        public string Category { get; set; }

        public EnvironmentOptions():this("", "")
        {

        }

        public EnvironmentOptions(string enviroment): this(enviroment, "")
        {

        }

        public EnvironmentOptions(string enviroment, string category)
        {
            Enviroment = enviroment;
            Category = category;
        }

        public override string ToString() => $"{Enviroment}: {Category}";
    }
}
