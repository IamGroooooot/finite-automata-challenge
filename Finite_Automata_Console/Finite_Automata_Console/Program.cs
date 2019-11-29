using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite_Automata_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Given Regular Expression: a+bc");
            EpsilonNFA thomsonNFA1 = EpsilonNFA.ThomsonsConstruction("a+bc");
            Console.WriteLine(thomsonNFA1.ToString());
            Console.WriteLine("-----------------------------------------------------------\n");

            Console.WriteLine("Given  Expression: a*");
            EpsilonNFA thomsonNFA2 = EpsilonNFA.ThomsonsConstruction("a*");
            Console.WriteLine(thomsonNFA2.ToString());
            Console.WriteLine("-----------------------------------------------------------\n");

        }
    }
}
