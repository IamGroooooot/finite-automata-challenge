using System;
using System.Collections.Generic;

namespace Finite_Automata_Console
{
    class EpsilonNFA : FiniteAutomata
    {
        // 오토마타 실행을 위한 변수들
        string CurrentState { get; set; }
        string LeftReadingInput { get; set; }

        public EpsilonNFA()
        {

        }

        public EpsilonNFA(HashSet<string> States, HashSet<string> Inputs, Dictionary<Tuple<string, string>, string> TransitionFunctions, HashSet<string> StartState, HashSet<string> FinalStates)
        {
            StartState = new HashSet<string>();
            StartState.Add("q0");
        }

        //e-NFA to DFA
        public static DFA SubsetConstruction(EpsilonNFA epsilonNFA)
        {
            // 여기에 코드

            return new DFA();
        }

        public static EpsilonNFA ThomsonsConstruction()
        {
            // 여기에 코드

            return new EpsilonNFA();
        }

    }
}
