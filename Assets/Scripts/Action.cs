﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Class is a glorifed struct and may be changed to such later.
//Also it may not need to be a MonoBehaviour but I don't think leaving it as
//such hurts anything.
public class Action {

    //TODO possibly expand. This should cover the actions we have decided to include so far
    private ActionType type; //What the action does
    private int power; //How much it does (damage, healing, etc)
    private int range; //How far it reaches
    private Target target;

    public ActionType Type
    {
        get
        {
            return type;
        }
    }

    public int Power
    {
        get
        {
            return power;
        }
    }

    public int Range
    {
        get
        {
            return range;
        }
    }

    public Target Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    public Action(ActionType type, Target target, int power, int range)
    {
        this.type = type;
        this.target = target;
        this.power = power;
        this.range = range;
    }

    //Used to combine two like type actions into a single more powerful action
    public Action Combine(Action action)
    {
        if (action.type != this.type || action.target != this.target) //For not unlike actions can't be combined
            return null;
        //These calculations can be changed to account for synergies
        int power = this.power + action.power;
        //Ranges should match but if not should default to the the shorter range;
        int range = Mathf.Min(this.range, action.range);

        return new Action(type, target, power, range);
    }
}
public enum ActionType { MeleeAttack, LongAttack, Heal, Slow };

public enum Target
{
    Ally,
    Enemy,
    Self,
    Everyone,
}