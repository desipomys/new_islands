using UnityEngine;
using System.Collections.Generic;

public abstract class GoapAction : IDataContainer,IEventRegister {


	private HashSet<KeyValuePair<string,object>> preconditions;//不用保存这些，只保存子类需要保存的东西
	private HashSet<KeyValuePair<string,object>> effects;

	private bool inRange = false;

	/* The cost of performing the action. 
	 * Figure out a weight that suits the action. 
	 * Changing it will affect what actions are chosen during planning.*/
	public float cost{get{return GetCost();} private set{SetCost(value);}}
    float _cost=1f;

	/**
	 * An action often has to perform on an object. This is that object. Can be null. */
	public GameObject target;
	protected EventCenter center;
	protected BaseGoapSensor sensor;

	public GoapAction() {
		preconditions = new HashSet<KeyValuePair<string, object>> ();
		effects = new HashSet<KeyValuePair<string, object>> ();
	}
	public virtual void OnEventRegist(EventCenter evc)
    {
        center=evc;
		sensor=center.gameObject.GetComponent<BaseGoapSensor>();
    }
	/// <summary>
	/// 每次plan时执行
	/// </summary>
	public virtual void doReset() {
		inRange = false;
		target = null;
		reset ();
	}

	/**
	 * Reset any variables that need to be reset before planning happens again.
	 */
	public abstract void reset();

	/**
	 * Is the action done?
	 */
	public abstract bool isDone();

	/**
	 * Procedurally check if this action can run. Not all actions
	 * will need this, but some might.
	 */
	public abstract bool checkProceduralPrecondition(GameObject agent);

	/**
	 * Run the action.
	 * Returns True if the action performed successfully or false
	 * if something happened and it can no longer perform. In this case
	 * the action queue should clear out and the goal cannot be reached.
	 */
	public abstract bool perform(GameObject agent);

	/**
	 * Does this action need to be within range of a target game object?
	 * If not then the moveTo state will not need to run for this action.
	 */
	public abstract bool requiresInRange ();
	
    public virtual float GetCost()
    {
        return _cost;
    }
    public virtual void SetCost(float value){_cost=value;}

	/**
	 * Are we in range of the target?
	 * The MoveTo state will set this and it gets reset each time this action is performed.
	 */
	public virtual bool isInRange () {
		return inRange;
	}
	
	public void setInRange(bool inRange) {
		this.inRange = inRange;
	}


	public void addPrecondition(string key, object value) {
		preconditions.Add (new KeyValuePair<string, object>(key, value) );
	}


	public void removePrecondition(string key) {
		KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
		foreach (KeyValuePair<string, object> kvp in preconditions) {
			if (kvp.Key.Equals (key)) 
				remove = kvp;
		}
		if ( !default(KeyValuePair<string,object>).Equals(remove) )
			preconditions.Remove (remove);
	}


	public void addEffect(string key, object value) {
		effects.Add (new KeyValuePair<string, object>(key, value) );
	}


	public void removeEffect(string key) {
		KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
		foreach (KeyValuePair<string, object> kvp in effects) {
			if (kvp.Key.Equals (key)) 
				remove = kvp;
		}
		if ( !default(KeyValuePair<string,object>).Equals(remove) )
			effects.Remove (remove);
	}

	
	public HashSet<KeyValuePair<string, object>> Preconditions {
		get {
			return preconditions;
		}
	}

	public HashSet<KeyValuePair<string, object>> Effects {
		get {
			return effects;
		}
	}

    public virtual int GetDataCollectPrio => 0;

    public object ToObject()
    {
        return null;
    }
    public virtual void FromObject(object data)
    {
        throw new System.NotImplementedException();
    }

    public void AfterEventRegist()
    {
        throw new System.NotImplementedException();
    }
}

/*
https://gitee.com/baiyun_is_lovely/goap/blob/master/Assets/Standard%20Assets/Scripts/AI/GOAP/GoapAction.cs#
*/