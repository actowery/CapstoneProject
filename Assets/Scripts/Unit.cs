﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour {

    //Needed to Detect Child Collision
    [SerializeField] Collider collider;

    [Header("Transform Properties")]
    [SerializeField] protected Vector3 heightOffset;
    [SerializeField] protected Vector3 moveDestination;
    [SerializeField] protected float moveSpeed = 10.0f;
    [SerializeField] protected float movementTolerance = .25f;

    [Header("Unit Stats")]
    [Tooltip("Which player controls this unit")]
    [SerializeField] protected float playerNumber;
    [SerializeField] protected float hitPoints;
    [SerializeField] protected float damageTaken;
    [SerializeField] protected float attack;
    [SerializeField] protected float speed;
    [SerializeField] protected float mass;
    [SerializeField] protected float shields;
    [SerializeField] protected float hardPoints;

    protected UnitType type;
    protected List<Module> modules;
    protected List<StatusEffects> statuses;

    [SerializeField] private int movementRange = 4;
    private bool isMoving = false;
    protected bool movementFinished = false;
    Tile currentTile;

    //based on unit type provide this will give base stats to unit
    public void DefineUnit(UnitType Type)
    {
        modules = new List<Module>();
        statuses = new List<StatusEffects>();
        if (UnitType.fighter == Type)
        {
            hitPoints = 100;
            mass = 400;
            hardPoints = 2;
            speed = 100;
            damageTaken = 0;
            attack = 20;
            type = Type;
            shields = 0;
        }
    }

    //Adds status to list
    public void AddStatus(StatusEffects status)
    {
        statuses.Add(status);
    }

    //decrease each status by one -- remove if duration reaches 0
    public void DecrementStatuses()
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            statuses[i].DecrementDuration();
            if (statuses[i].duration == 0)
            {
                statuses.RemoveAt(i);
                i -= 1;
            }
        }
    }

    //for testing
    public void DebugStatuses()
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            Debug.Log(statuses[i].statusName + " " + statuses[i].duration);
        }
    }
    //Adds module to unit
    public void AddModule(Module module)
    {
        //can only have modules up to number of hardpoints
        if (!(modules.Count >= hardPoints))
        {
            modules.Add(module);
        }
    }

    //remove specific module from list (if it is in list)
    public void RemoveModule(ModuleName module)
    {
        for(int i = 0; i < modules.Count; i++)
        {
            if(modules[i].ModuleName == module)
            {
                modules.RemoveAt(i);
                break;
            }
        }
    }

    //removes all modules from unit
    public void RemoveAllModules()
    {
        modules.Clear();
    }

    //returns base hp + bonus from modules
    public float GetHP()
    {
        float moddedStat = hitPoints;

        for (int i = 0; i < modules.Count; i++)
        {
            moddedStat += modules[i].HitPoints;
        }

        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i].statusName == statusType.hitPoints)
            {
                moddedStat += statuses[i].amount;
            }
        }
        return moddedStat;
    }

    //returns base mass + bonus from modules
    public float GetMass()
    {
        float moddedStat = mass;

        for (int i = 0; i < modules.Count; i++)
        {
            moddedStat += modules[i].Mass;
        }

        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i].statusName == statusType.mass)
            {
                moddedStat += statuses[i].amount;
            }
        }
        return moddedStat;
    }

    //returns base attack + bonus from modules
    public float GetAttack()
    {
        float moddedStat = attack;

        for (int i = 0; i < modules.Count; i++)
        {
            moddedStat += modules[i].Attack;
        }

        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i].statusName == statusType.attack)
            {
                moddedStat += statuses[i].amount;
            }
        }
        return moddedStat;
    }

    public Tile CurrentTile
    {
        get
        {
            return currentTile;
        }

        set
        {
            currentTile = value;
        }
    }

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }

        set
        {
            isMoving = value;
        }
    }

    void Awake () {
		moveDestination = transform.position;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        if (IsMoving)
            MoveCharacter();
        if (movementFinished)
        {
            movementFinished = false;
            GameManager.instance.FinishedMovement();
        }
    }

    public virtual void TurnUpdate(){

	}

    //returns movement after calculation based on unit mass
    public int GetMovementRange() {
        float moddedMass = GetMass();
        movementRange = (int)((1000 - moddedMass) / 100);

        if (movementRange <= 0)
        {
            movementRange = 1;
        }
        return movementRange;
    }


    //TODO Make unit get a path of tiles and follow that path
    //This will make unit movement follow the actual NWSE movement they can perform
    //And prevent a unit from shortcutting across tiles they can't actually enter
    public void MoveToTile(Tile tile)
    {
        if (tile != null)
        {
            UpdateTile(tile);
            moveDestination = tile.transform.position + heightOffset;
            isMoving = true;
        }
    }

    private void UpdateTile(Tile tile)
    {
        if(currentTile != null)
            currentTile.UnitOnTile = null;
        currentTile = tile;
        currentTile.UnitOnTile = this;
    }

    protected void MoveCharacter()
    {
        if (Vector3.Distance(moveDestination, transform.position) > movementTolerance)
        {
            transform.position += (moveDestination - transform.position).normalized * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(moveDestination, transform.position) <= movementTolerance)
            {
                movementFinished = true;
                isMoving = false;
                transform.position = moveDestination;
            }
        }
    }

    //Places the unit on a tile instantly (No Visual Movment)
    public void PlaceOnTile(Tile tile)
    {
        if(tile != null)
        {
            moveDestination = transform.position = tile.transform.position + heightOffset;
            UpdateTile(tile);
        }
    }

    private void StartHighlightAnimation()
    {

    }

    private void StopHighlightAnimation()
    {

    }
}

//used to show type of unit
public enum UnitType
{
    fighter,
}
