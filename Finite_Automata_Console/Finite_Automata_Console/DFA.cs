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

        }
        
        // 생성자
        public DFA(HashSet<string> _States, HashSet<string> _Inputs, Microsoft.Collections.Extensions.MultiValueDictionary<Tuple<string, string>, string> _TransitionFunctions, HashSet<string> _StartState, List<string> _FinalStates)
        {
            States = _States;
            Inputs = _Inputs;
            TransitionFunctions = _TransitionFunctions;
            StartState = _StartState;
            FinalStates = _FinalStates;
        }

    }
}
