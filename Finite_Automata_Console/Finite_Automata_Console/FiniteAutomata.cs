using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite_Automata_Console
{
    abstract class FiniteAutomata
    {
        // 필드
        // 오토마타를 위한 변수들
        protected HashSet<string> States { get; set; }
        protected HashSet<string> Inputs { get; set; }
        protected Dictionary<Tuple<string, string>, string> TransitionFunctions { get; set; }
        protected string StartState { get; set; }
        protected HashSet<string> FinalStates { get; set; }

        // 오토마타 실행을 위한 변수들
        protected string CurrentState { get; set; }
        protected string LeftReadingInput { get; set; }

        // 쓰기 쉽도록 미리 만들어 둘 것들
        public static HashSet<string> DefaultInputs { get; private set; }
        // set static 
        static FiniteAutomata()
        {
            DefaultInputs = new HashSet<string>();
            for (int i = 'a'; i <= 'a' + 25; i++)
            {
                DefaultInputs.Add(((char)i).ToString());
            }
            for (int i = 'A'; i <= 'A' + 25; i++)
            {
                DefaultInputs.Add(((char)i).ToString());
            }
            for (int i = '0'; i <= '9'; i++)
            {
                DefaultInputs.Add(((char)i).ToString());
            }
            DefaultInputs.Add("Epsilon");
        }

    }
}
