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
            //Console.Write("+".Equals("a+b"[1].ToString()));

            EpsilonNFA myNFA = EpsilonNFA.ThomsonsConstruction("a+bc");

            Console.WriteLine(myNFA.ToString());
        }
    }
}
