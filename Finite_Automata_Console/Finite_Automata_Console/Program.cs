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
            var ad = new EpsilonNFA();
            ad.SubsetConstruction();
        }
    }
}
