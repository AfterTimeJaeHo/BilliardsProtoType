using System;
using System.Collections;
using System.Collections.Generic;
using SRPG;
using UnityEngine;

public class StateTransition
{
    public IState nextState;
    public Func<bool> condition;
}