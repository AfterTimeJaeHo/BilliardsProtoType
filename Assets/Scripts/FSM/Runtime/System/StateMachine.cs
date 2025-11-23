using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SRPG;
using UnityEngine;

namespace StateMachine.Runtime
{
    public class StateMachine
    {
        public Entity OwnerEntity { get; private set; }
        public IState CurState { get; private set; }
        public IState PreState { get; private set; }
        public IState GlobalState { get; private set; }
        private List<StateTransition> transitions;

        public virtual void Init(Entity entity, IState state)
        {
            OwnerEntity = entity;
            CurState = state;
            PreState = null;
            GlobalState = null;
            transitions = new List<StateTransition>();
            CurState.Enter();
        }

        // 유니티 Update에서 매 프레임 호출
        public virtual void Execute()
        {
            StateTransition transition = transitions.SingleOrDefault(transiton => transiton.condition?.Invoke() == true);
            if (transition != null)
            {
                ChangeState(transition.nextState);
                return;
            }

            if (GlobalState != null)
            {
                GlobalState.Execute();
            }

            if (CurState != null)
            {
                CurState.Execute();
            }
        }

        public virtual void ChangeState(IState newState)
        {
            if (CurState != null)
            {
                PreState = CurState;
                CurState.Exit();
            }

            CurState = newState;
            CurState.Enter();
        }

        public virtual void SetGlobalState(IState newState)
        {
            GlobalState = newState;
            GlobalState.Enter();
        }

        public virtual void BackToPreState()
        {
            ChangeState(PreState);
        }

        public virtual void AddTransition(StateTransition transition)
        {
            transitions.Add(transition);
        }

        public virtual void RemoveTransition(StateTransition transition)
        {
            if (transitions.Contains(transition))
                transitions.Remove(transition);
        }
    }   
}