using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseNameGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += PrintHelpOnUnhandledException;
            //var tt = "-o Lizards -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d -e d";
            //args = tt.Split(' ');
            if (args.Contains("-h"))
            {
                PrintHelp();
                Environment.Exit(0);
                //Print help and exit
                // -e envio and optional cat seperated by space
                // can be multiple times - anything more then two arguments is ignored(or exception thrown?)
            }

            var envirementsAndCategories = new List<EnvironmentOptions>();
            var defaultCategory = "";

            for (int i = 0; i < args.Length; i++)
            {

                if (args[i].StartsWith("-"))
                {
                    switch (args[i])
                    {
                        case "-e":
                            if (args[i + 1].StartsWith("-"))
                            {
                                throw new ArgumentException("Must provide a value for the -e flag");
                            }
                            var enviroment = args[++i];
                            var category = i + 1 >= args.Length || args[i + 1].StartsWith("-") ? defaultCategory : args[++i];
                            envirementsAndCategories.Add(new EnvironmentOptions(enviroment, category));
                            break;
                        case "-o":
                            if (!string.IsNullOrWhiteSpace(defaultCategory))
                            {
                                throw new ArgumentException("Can not specify -o more then once");
                            }
                            defaultCategory = args[++i];
                            break;
                        default:
                            throw new ArgumentException($"Unknown switch: {args[i]}");
                    }
                }
            }

            if (envirementsAndCategories.Any(t => string.IsNullOrWhiteSpace(t.Category)))
            {
                foreach (var kev in envirementsAndCategories.Where(t => string.IsNullOrWhiteSpace(t.Category)))
                {
                    kev.Category = defaultCategory;
                }
            }

            Generator.InitDirectories();
            var g = new Generator();

            if (!envirementsAndCategories.Any())
            {
                Console.WriteLine($"Generated {g.Generate(defaultCategory)}");
            }
            else
            {
                foreach (var enviroment in envirementsAndCategories)
                {
                    Console.WriteLine($"Generated {g.Generate(enviroment.Category)} for {enviroment.Enviroment}");
                }
            }
        }

        private static void PrintHelpOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(((Exception)e.ExceptionObject).Message);
            PrintHelp();
            Environment.Exit(1);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("\t-o\tSet the default category");
            Console.WriteLine("\t-e\tEnviroment name [catgeory]");
            Console.WriteLine("\tExample:");
            Console.WriteLine("\t\t-e Release Lizrads -e Staging Birds");
        }
    }
}
