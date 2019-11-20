using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite_Automata_Console
{
    class DFA : FiniteAutomata
    {
        // 오토마타 실행을 위한 변수들
        string CurrentState { get; set; }
        string LeftReadingInput { get; set; }

        public DFA()
        {
            StartState = new HashSet<string>();
            StartState.Add("q0");
            
        }
       
    }
}
