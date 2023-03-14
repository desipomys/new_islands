using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/**
 * Stack-based Finite State Machine.
 * Push and pop states to the FSM.
 * 
 * States should push other states onto the stack 
 * and pop themselves off.
 */
using System;


public class GoapFSM {

	public Stack<GoapFSMState> stateStack = new Stack<GoapFSMState> ();

	public delegate void GoapFSMState (GoapFSM fsm, GameObject gameObject);
	

	public void Update (GameObject gameObject) {
		if (stateStack.Peek() != null)
			stateStack.Peek()(this, gameObject);
	}

	public void pushState(GoapFSMState state) {
		stateStack.Push (state);
	}

	public void popState() {
		stateStack.Pop ();
	}
	public void ClearState()
	{
		stateStack.Clear();
	}
}
/*
public interface GoapFSMState 
{
	
	void Update (GoapFSM fsm, GameObject gameObject);
}
*/