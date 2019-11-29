using System;
using System.Collections.Generic;

namespace Finite_Automata_Console
{
    class EpsilonNFA : FiniteAutomata
    {
        // 실제 오토마타 실행을 위한 변수들
        string CurrentState { get; set; }
        string LeftReadingInput { get; set; }

        // 기본 생성자
        public EpsilonNFA()
        {
            States = new HashSet<string>();
            Inputs = DefaultInputs;
            TransitionFunctions = new Microsoft.Collections.Extensions.MultiValueDictionary<Tuple<string, string>, string>();
            StartState = new HashSet<string>();
            FinalStates = new List<string>();
        }
        
        // 생성자 Input 생략하면 기본 인풋 넣음
        public EpsilonNFA(HashSet<string> _States, Microsoft.Collections.Extensions.MultiValueDictionary<Tuple<string, string>, string> _TransitionFunctions, HashSet<string> _StartState, List<string> _FinalStates)
        {
            States = _States;
            Inputs = DefaultInputs;
            TransitionFunctions = _TransitionFunctions;
            StartState = _StartState;
            FinalStates = _FinalStates;
        }

        // 생성자
        public EpsilonNFA(HashSet<string> _States, HashSet<string> _Inputs, Microsoft.Collections.Extensions.MultiValueDictionary<Tuple<string, string>, string> _TransitionFunctions, HashSet<string> _StartState, List<string> _FinalStates)
        {
            States = _States;
            Inputs = _Inputs;
            TransitionFunctions = _TransitionFunctions;
            StartState = _StartState;
            FinalStates = _FinalStates;
        }

        // Transition을 추가해주는 함수
        public void AddTransition(Tuple<string, string> delta, string chagedState)
        {
            TransitionFunctions.Add(delta, chagedState);
        }

        //e-NFA to DFA
        public static DFA SubsetConstruction(EpsilonNFA epsilonNFA)
        {
            HashSet<string> startState = new HashSet<string>();
            List<HashSet<string>> states = new List<HashSet<string>>();
            Microsoft.Collections.Extensions.MultiValueDictionary<Tuple<string, string>, string> transitionFunctions = new Microsoft.Collections.Extensions.MultiValueDictionary<Tuple<string, string>, string>();
            HashSet<string> reconstructedStates = new HashSet<string>();
            List<string> finalStates = new List<string>();

            foreach (var item in epsilonNFA.StartState)
            {
                startState.UnionWith(epsilonNFA.closure(item));
                break;
            }

            foreach (var input in epsilonNFA.Inputs)
            {
                foreach (var state in startState)
                {
                    foreach (var output in epsilonNFA.TransitionFunctions[new Tuple<string, string>(state, input)])
                    {
                        var newState = epsilonNFA.closure(output);
                        if (states.Contains(newState)) // 여기서 같은것이 반복되면 무시!
                        {
                            continue;
                        }

                        states.Add(newState);
                        
                        string reconstructedStartState = "";
                        string reconstructedFinalState = "";
                        foreach (var item in startState)
                        {
                            reconstructedStartState += item;
                        }
                        foreach (var item in newState)
                        {
                            reconstructedFinalState += item;
                        }
                        if (reconstructedFinalState.Equals(string.Empty))
                        {
                            continue;
                        }

                        transitionFunctions.Add(new Tuple<string, string>(reconstructedStartState,input),reconstructedFinalState);

                    }
                } 
            }

            // 해쉬셋을 재구성해서 스테이트로 쓰기위해 하나의 문자열로 만든다
            foreach (var set in states)
            {
                var newState = "";
                foreach (var str in set)
                {
                    newState += str;
                }
                reconstructedStates.Add(newState);

            }

            // 원래 NFA의 finalstate를 가진 것이 곧 DFA의 finalStte
            foreach (var state in reconstructedStates)
            {
                foreach (var nfaFinal in epsilonNFA.FinalStates)
                {
                    if (state.Contains(nfaFinal.ToString()))
                    {
                        finalStates.Add(state);
                    }
                }
            }
            
            return new DFA(reconstructedStates,epsilonNFA.Inputs,transitionFunctions, startState, finalStates);
        }

        HashSet<string> closure(HashSet<string> _startStates)
        {
            var reachables = new HashSet<string>();
            //자기 state추가 
            foreach (var start in _startStates)
            {
                reachables.Add(start);
            }

            foreach (var start in _startStates)
            {
                foreach (var cl in closure(start))
                {
                    reachables.Add(cl);
                }
            }

            return reachables;
        }

        // ep - closure
        HashSet<string> closure(string _startState)
        {
            var reachables = new HashSet<string>();
            //자기 state추가 
            reachables.Add(_startState);
            
            //1회 순회
            var outcomes = TransitionFunctions[new Tuple<string, string>(_startState, string.Empty)];
            if (outcomes.Count != 0)
            {
                foreach (var outcome in outcomes)
                {
                    reachables.Add(outcome);
                }
            }

            //n회 순회
            foreach (var item in reachables)
            {
                reachables.UnionWith(closure(item));
            }

            return reachables;
        }

        public static EpsilonNFA ThomsonsConstruction(string RegularExpression)
        {
            // 톰슨이 가능한 경우
            if (canKleene(RegularExpression))
            {
                var newNFA = new EpsilonNFA();

                //do kleene
                if (RegularExpression.StartsWith("("))
                {// (RE)*인 경우
                    newNFA.StartState.Add("q0");
                    newNFA.FinalStates.Add("q3");
                    newNFA.States.Add("q0");
                    newNFA.States.Add("q3");
                    // can skip
                    newNFA.AddTransition(new Tuple<string, string>("q0", string.Empty), "q3");
                    // can loop
                    string innerRE = RegularExpression.Substring(1, RegularExpression.Length - 3);
                    var innerNFA = ThomsonsConstruction(innerRE);
                    foreach (var startState in innerNFA.StartState)
                    {
                        innerNFA.AddTransition(new Tuple<string, string>(innerNFA.FinalStates[0].ToString(), string.Empty),startState);
                        break;// startstate는 하나일 것이므로 바로 break
                    }

                    //두개의 NFA 통합
                    var output = IntegrateEpsilonNFA(newNFA, innerNFA);
                    foreach (var innerStartState in innerNFA.StartState)
                    {
                        output.AddTransition(new Tuple<string, string>("q0", string.Empty), "r"+innerStartState);
                        break;// startstate는 하나일 것이므로 바로 break
                    }
                    output.AddTransition(new Tuple<string, string>("r"+innerNFA.FinalStates[0].ToString(),string.Empty),"q3");

                    return output;
                }
                else
                {// a*인 경우
                    newNFA.StartState.Add("q0");
                    newNFA.FinalStates.Add("q3");
                    newNFA.States.Add("q0");
                    newNFA.States.Add("q1");
                    newNFA.States.Add("q2");
                    newNFA.States.Add("q3");
                    newNFA.AddTransition(new Tuple<string, string>("q0", string.Empty),"q1");
                    newNFA.AddTransition(new Tuple<string, string>("q0", string.Empty),"q3");
                    newNFA.AddTransition(new Tuple<string, string>("q1", RegularExpression),"q2");
                    newNFA.AddTransition(new Tuple<string, string>("q2", string.Empty),"q3");
                    newNFA.AddTransition(new Tuple<string, string>("q2", string.Empty),"q1");

                    return newNFA;
                }
            }
            else if (canConcat(RegularExpression))
            {
                //do concat // ab
                var left = ThomsonsConstruction(RegularExpression.Substring(0,1));
                var right = ThomsonsConstruction(RegularExpression.Substring(1));
                return Concat(left,right);
            }
            else if (canUnion(RegularExpression))
            {

                if (RegularExpression.Length == 3)
                { // a + b인 경우
                    var newNFA = new EpsilonNFA();
                    //do branch
                    newNFA.StartState.Add("q0");
                    newNFA.FinalStates.Add("q5");
                    newNFA.States.Add("q0");
                    newNFA.States.Add("q1");
                    newNFA.States.Add("q2");
                    newNFA.States.Add("q3");
                    newNFA.States.Add("q4");
                    newNFA.States.Add("q5");
                    newNFA.AddTransition(new Tuple<string, string>("q0", string.Empty), "q1");
                    newNFA.AddTransition(new Tuple<string, string>("q0", string.Empty), "q2");
                    newNFA.AddTransition(new Tuple<string, string>("q3", string.Empty), "q5");
                    newNFA.AddTransition(new Tuple<string, string>("q4", string.Empty), "q5");
                    newNFA.AddTransition(new Tuple<string, string>("q1", RegularExpression.Substring(0, 1)), "q3");
                    newNFA.AddTransition(new Tuple<string, string>("q2", RegularExpression.Substring(2)), "q4");

                    return newNFA;
                }
                else
                {
                    // 괄호가 있는 경우 구현 못했음

                }
            }
            else if (canParen(RegularExpression))
            {
                // (RE)인 경우 => RE
                // do paren
                string noParen = RegularExpression.Substring(1, RegularExpression.Length - 2);
                return ThomsonsConstruction(noParen);
            }
            else if (RegularExpression.Length == 1 && DefaultInputs.Contains(RegularExpression))
            {
                // 하나의 인풋인 경우 // a
                var newNFA = new EpsilonNFA();
                newNFA.StartState.Add("q0");
                newNFA.FinalStates.Add("q1");
                newNFA.States.Add("q0");
                newNFA.States.Add("q1");
                newNFA.AddTransition(new Tuple<string, string>("q0",RegularExpression),"q1");
                return newNFA;
            }

            // 더 쪼개야 톰슨이 가능한 경우
            // 제일 먼저 적용돼야되는 부분을 m으로 두고 쪼갠다
            EpsilonNFA l = new EpsilonNFA();
            EpsilonNFA m = new EpsilonNFA();
            EpsilonNFA r = new EpsilonNFA();

            if (RegularExpression.Contains("*"))
            {
                int starIndex = RegularExpression.IndexOf("*");
                if (DefaultInputs.Contains(RegularExpression[starIndex - 1].ToString()))
                {// a*
                    l = ThomsonsConstruction(RegularExpression.Substring(0, starIndex - 1));
                    m = ThomsonsConstruction(RegularExpression.Substring(starIndex - 1,2));
                    r = ThomsonsConstruction(RegularExpression.Substring(starIndex + 1));
                }
                else
                {// (RE)*
                    //괄호가 있는 경우 구현하지 못했습니다  ㅠㅠ

                }
            }
            else if (RegularExpression.Length > 1 && hasConcatableInput(RegularExpression)!=-1 )
            {// abRE
                int concatIndex = hasConcatableInput(RegularExpression);
                l = ThomsonsConstruction((RegularExpression.Substring(0, concatIndex)));
                m = ThomsonsConstruction((RegularExpression.Substring(concatIndex, 2)));
                if (concatIndex + 2 < RegularExpression.Length-1 )
                {
                    // abRE인 경우
                    r = ThomsonsConstruction((RegularExpression.Substring(concatIndex + 2)));
                }
                else
                {
                    //ab인 경우
                    r = null; 
                }

                // 괄호가 있는 경우는 고려하지 못했습니다 ㅠㅠ
            }
            else if (RegularExpression.Contains("+"))
            {
                int unionIndex = RegularExpression.IndexOf("+");

                if (DefaultInputs.Contains(RegularExpression[unionIndex - 1].ToString()) && DefaultInputs.Contains(RegularExpression[unionIndex + 1].ToString()))
                {//a+b
                    l = ThomsonsConstruction(RegularExpression.Substring(0,unionIndex - 1));
                    m = ThomsonsConstruction(RegularExpression.Substring(unionIndex - 1, 3));
                    r = ThomsonsConstruction(RegularExpression.Substring(unionIndex + 2));
                }
                else
                {
                    //괄호가 있는 경우 구현하지 못했습니다  ㅠㅠ

                }
            }
            // 우선 순위대로 쪼갠다
            else if (RegularExpression.Contains("(") && RegularExpression.Contains(")"))
            {
                int firstIndex = RegularExpression.IndexOf("(");
                int lastIndex = RegularExpression.LastIndexOf(")");
                if (firstIndex > 0)
                {
                    l = ThomsonsConstruction(RegularExpression.Substring(0,firstIndex));
                }
                else
                {
                    l = null;
                }

                m = ThomsonsConstruction(RegularExpression.Substring(firstIndex, lastIndex - firstIndex + 1));

                if (lastIndex == (RegularExpression.Length-1))
                {// RE(RE)
                    r = null;
                }
                else
                {
                    r = ThomsonsConstruction(RegularExpression.Substring(lastIndex));
                }
            }

            return Concat(Concat(l,m), r);
        }


        // 연속적으로 input이 나오는 지 확인
        // 나왔다면 그 index를 반환 아니면 -1
        static int hasConcatableInput(string RE)
        {
            int stringLength = RE.Length;
            for (int i =0; i < stringLength-2; i++) {
                if (DefaultInputs.Contains(RE[i].ToString()) && DefaultInputs.Contains(RE[i+1].ToString()))
                {
                    return i;
                }
            }
            return -1;
        }

        // 두 입실론 NFA를 Concat한다
        static EpsilonNFA Concat(EpsilonNFA l,EpsilonNFA r)
        {
            if(null == l || l.FinalStates.Count ==0 || l.StartState.Count == 0)
            {
                return r;
            }
            else if(null == r || r.FinalStates.Count == 0 || r.StartState.Count == 0)
            {
                return l;
            }
            else
            {   // concat Thomson's Construction 적용
                // l에 State 추가 ㅁ -> ㅁ (->) ㅇ -> ㅇ 
                // l의 final state 랑 r의 start state 연결
                // final 재설정
                var target = IntegrateEpsilonNFA(l, r);

                target.FinalStates.RemoveAll(s=>true); // remove all
                foreach (var rFinal in r.FinalStates)
                {
                    target.FinalStates.Add("r" + rFinal);
                }
                if(l.FinalStates.Count == 0)
                {
                    return target;
                }

                foreach (var start in r.StartState)
                {
                    target.AddTransition(new Tuple<string, string>(l.FinalStates[0].ToString(), string.Empty), "r" + start);
                    break;
                }

                return target;
            }
        }

        // Or연산을 할 수 있는지 반환
        static bool canUnion(string RE)
        {
            if (RE.Length == 3) // "a+b"
            {
                if ("+".Equals(RE[1].ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        // Concat연산을 수행할 수 있는지 반환
        static bool canConcat(string RE)
        {
            if (RE.Length == 2)
            {
                if(DefaultInputs.Contains(RE[0].ToString()) && DefaultInputs.Contains(RE[1].ToString()))
                {
                    return true;
                }
            }
            else
            {
                // 괄호가 있는 경우
                // RE(RE)

                //(RE)RE

                //(RE)(RE)

                //RE(RE)RE
            }


            return false;
        }

        // Kleene 연산을 수행할 수 있는지 반환
        static bool canKleene(string RE)
        {
            // 길이가 충분하고 *로 끝이 나는가?
            if (RE.Length > 1 && RE.EndsWith("*"))
            {
                if (RE.Contains("(") && RE.Length > 3)
                {   // (RE)*인 경우
                    if (")".Equals(RE[RE.Length - 2].ToString()))
                    {
                        return true;
                    }
                }
                else
                {
                    // a*d인 경우
                    if(RE.Length == 2 && DefaultInputs.Contains(RE[0].ToString()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // ()가 감지 됐는지 반환
        static bool canParen(string RE)
        {
            if (RE.Length>1)
            {
                return RE.StartsWith("(") && RE.EndsWith(")");
            }

            return false;
        }

        // 왼쪽NFA에 오른쪽NFA의 State추가 Transition추가해서 통합시켜 준다
        private static EpsilonNFA IntegrateEpsilonNFA(EpsilonNFA l, EpsilonNFA r)
        {
            // copy l to new instance
            var leftCopy = new EpsilonNFA();
            leftCopy.Inputs = l.Inputs;
            foreach (var start in l.StartState)
            {
                leftCopy.StartState.Add(start);
            }
            foreach (var final in l.FinalStates)
            {
                leftCopy.FinalStates.Add(final);
            }
            foreach (var state in l.States)
            {
                leftCopy.States.Add(state);
            }
            foreach (var trans in l.TransitionFunctions)
            {
                foreach (var val in trans.Value)
                {
                    leftCopy.AddTransition(new Tuple<string, string>(trans.Key.Item1, trans.Key.Item2), val);
                }
            }

            // do integration
            foreach (var item in r.States)
            {
                leftCopy.States.Add("r" + item);
            }
            foreach (var item in r.TransitionFunctions)
            {
                var rCurrentState = item.Key.Item1;
                rCurrentState = "r" + rCurrentState;
                var rInput = item.Key.Item2;

                var rValues = item.Value;
                foreach (var rVal in rValues)
                {
                    leftCopy.TransitionFunctions.Add(new Tuple<string, string>(rCurrentState, rInput), "r" + rVal);
                }
            }

            return leftCopy;
        }

    }

    
}
