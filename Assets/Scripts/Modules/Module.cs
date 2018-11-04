﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {
    public int HitPoints { get; protected set; }
    //public int Mass { get; protected set; }
    //public Target ModuleTarget { get; protected set; }
    //public ModuleType ModuleType { get; set; }
    public Action Action { get; protected set; }

    [SerializeField] protected ModuleType type;
    [SerializeField] protected int mass;

    public int Mass
    {
        get
        {
            return mass;
        }

        set
        {
            mass = value;
        }
    }

    public ModuleType Type
    {
        get
        {
            return type;
        }
    }

    //virtual GetAction
    public virtual Action GetAction()
    {
        return null;
    }

    public virtual Buff GetBuff()
    {
        return null;
    }
}

//used to name module
public enum ModuleType
{
    shortRange, longRange, heal, slow, engine, shields
}

//used for choosing target
