using System;
using System.Collections.Generic;

namespace StateMachine.Runtime {
    [Serializable]
    public class State 
    {
        public string StateName;
        public List<int> NextNodeIndices = new();
    }
}