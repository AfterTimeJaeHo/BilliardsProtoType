using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRPG
{
    public delegate void OnEnter();

    public delegate void OnExecute();

    public delegate void OnExit();
    
    public interface IState
    {

        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }

        // 시작할 때 1회 호출
        public void Enter();
    
        // Update에서 매 프레임 호출
        public void Execute();

        // 종료할 때 1회 호출
        public void Exit();
    }
}


