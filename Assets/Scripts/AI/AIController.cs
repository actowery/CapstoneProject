﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour {

    /***************************************************CLASSES AND ENUMS *************************************/
    enum Team {Enemy, Ally};


   

    /********************************************** FIELDS *****************************************/

    Unit unit;
    ModuleType moduleType;

    List<Command> commands;

    public static AIController instance;
    [SerializeField] Map map;

    //List<TileData> annotatedRange;

    /********************************************* UNITY METHODS ***************************************/

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }





    /********************************************************** COMMAND FUNCTIONS ******************************/

    public List<Command> GetAICommands(Unit unit)
    {
        this.unit = unit;
        commands = new List<Command>();
        GetAIUnitType();
        switch (moduleType)
        {
            case ModuleType.shortRange:
                commands = GetMeleeCommands();
                break;
            case ModuleType.heal:
                commands = GetHealCommands();
                break;
        }
        return commands;
    }

    private List<Command> GetMeleeCommands()
    {
        Action meleeAction = GetAction(ActionType.ShortAttack);
        List<Command> commands = new List<Command>();
        
        //Determine if highest threat in melee range is adjacent
        List<Unit> adjEnemies = GetAdjacentUnits(unit.CurrentTile, Team.Enemy);

        Unit highestThreatUnit = GetHighestThreat(map.GetEnemyUnitsInMeleeRange(unit));

        //If the highest threat unit is in range attack it
        if (highestThreatUnit != null && adjEnemies.Count > 0 && adjEnemies.Contains(highestThreatUnit))
        {
            commands.Add(new Command(highestThreatUnit.CurrentTile, Command.CommandType.Action, meleeAction));
        }
        else
        {
            //If there is not unit in melee range find the closest enemy and move to engage
            if (highestThreatUnit == null)
            {
                highestThreatUnit = GetClosestUnit(unit.CurrentTile, Team.Enemy);
            }
            //Move to the enemy with the highest threat
            Tile closestTileToEnemy = GetClosestTile(map.GetMovementRange(unit), highestThreatUnit.CurrentTile);
            commands.Add(new Command(closestTileToEnemy, Command.CommandType.Move, null));
        }
        /*TODO Implement
         * Check if the commands include a movement. If it doesn't unit has attacked without moving
         * If this is the case move away to a lower threat location
         */
        if (!ContainsMove(commands))
        {
            //Move to a lower threat area
        }
        /* The unit has moved without attacking. Attack if possible*/
        if (!ContainsAction(commands) && ContainsMove(commands))
        {
            //Get Enemies Adjacent to move
            adjEnemies = GetAdjacentUnits(GetFirstMoveTile(commands), Team.Enemy);
            Unit target = GetHighestThreat(adjEnemies);
            if (target != null)
                commands.Add(new Command(target.CurrentTile, Command.CommandType.Action, meleeAction));
        }

        return commands;
    }


    private List<Command> GetHealCommands()
    {
        Action action = GetAction(ActionType.Heal);
        //List<TileData> tileData = UpdateTileData(action);


        List<Command> commands = new List<Command>();

        //Find Highest Value Damaged Unit (HVDU) in potential range
        List<Unit> allies = map.GetAllAllies(unit)
            .Where(unit => unit.PlayerNumber == unit.PlayerNumber && unit.GetDamage() > 0)
            .OrderBy(o=>o.GetThreat())
            .OrderByDescending(o=>o.GetHP())
            .ToList();

        Unit HVDU = allies.Count > 0 ? allies[0] : null;


        //If unit is in current range heal and move to lower threat area &&|| towards next best HVDU
        List<Tile> tilesInHealRange = map.GetMovementRangeExtended(unit, 2);
        

        //Else move to lowest threat tile in range of HVDU and heal HVDU

        //TEST
        commands.Add(new Command(unit.CurrentTile, Command.CommandType.Action, action));
        map.ColorTiles(tilesInHealRange, Tile.TileColor.ally);
        //map.ColorTiles(map.GetMovementRange(aiUnit), Tile.TileColor.move);
        return commands;
    }

    /************************************************* INFORMATION FUNCTIONS *************************************/

    //private List<TileData> UpdateTileData(Action action)
    //{
    //    List<Tile> rangeList = map.GetMovementRange(unit);
    //    List<TileData> annotatedRange = new List<TileData>();
    //    foreach(var t in rangeList)
    //    {
    //        TileData tD = new TileData(t);
    //        List<Unit> unitsInRange = map.GetUnitsInRange(t, action.Range); 
    //        foreach(var u in unitsInRange)
    //        {
    //            if (u.PlayerNumber == unit.PlayerNumber)
    //                tD.AlliesInUnitRange++;
    //            else
    //                tD.EnemiesInUnitRange++;
    //        }

    //    }
    //    return annotatedRange;
    //}

    /************************************************ UTILITY FUNCTIONS *****************************************/

    //Returns the unit with highest threat from a list of units
    private Unit GetHighestThreat(List<Unit> units)
    {
        Unit highestThreat = null;
        //TODO Convert this to find highest threat unit
        //highestThreat = units.Count > 0 ? units[0] : null;
        foreach(var u in units)
        {
            if(highestThreat == null || highestThreat.GetThreat() < u.GetThreat())
            {
                highestThreat = u;
            }
        }
        return highestThreat;
    }

    private Action GetAction(ActionType type)
    {
        List<Action> actions = unit.GetActions();
        Action bestAction = null;
        foreach(var a in actions)
        {
            if(a.Type == type)
            {
                //There should be only one melee action but this will get the best if there is more then one
                if(bestAction == null || bestAction.Power < a.Power)
                    bestAction = a;
            }
        }
        return bestAction;
    }

    private bool ContainsMove(List<Command> commands)
    {
        foreach (var c in commands)
            if (c.commandType == Command.CommandType.Move)
                return true;
        return false;
    }

    private bool ContainsAction(List<Command> commands)
    {
        foreach (var c in commands)
            if (c.commandType == Command.CommandType.Action)
                return true;
        return false;
    }

    private Tile GetFirstMoveTile(List<Command> commands)
    {
        foreach (var c in commands)
            if (c.commandType == Command.CommandType.Move)
                return c.target;
        return null;
    }

    //A ai unit without modules with break this which is fine because that unit wouldn't make sense in the current design
    private void GetAIUnitType()
    {
        moduleType = unit.GetModuleTypes()[0];
    }

    //Gets and returns the Unit that is closest to the specified Tile
    //If the enemy flag is set it will return the closest enemy
    //If the flag is not set it will return the closest ally
    //It will never return the current unit
    private Unit GetClosestUnit(Tile tile, Team team)
    {
        int aiPlayerNumber = unit.PlayerNumber;
        List<Unit> units = map.GetAllUnits();
        float closestDist = float.MaxValue;
        Unit closestUnit = null;
        foreach (var u in units)
        {
            if (u != unit && ((team == Team.Enemy && u.PlayerNumber != aiPlayerNumber) || (team == Team.Ally && u.PlayerNumber == aiPlayerNumber)))
            {
                float distToE = Vector2Int.Distance(u.CurrentTile.GetCoords(), unit.CurrentTile.GetCoords());
                if (distToE < closestDist)
                {
                    closestDist = distToE;
                    closestUnit = u;
                }
            }
        }
        return closestUnit;
    }

    List<Unit> GetAdjacentUnits(Tile tile, Team team)
    {
        List<Unit> units = map.GetAdjacentUnits(tile);
        List<Unit> adjacentUnits = new List<Unit>();
        foreach(var u in units)
            if ((team == Team.Enemy && u.PlayerNumber != unit.PlayerNumber) || (team == Team.Ally && u.PlayerNumber == unit.PlayerNumber))
                adjacentUnits.Add(u);
        return adjacentUnits;
    }

    private Tile GetClosestTile(List<Tile>avaliableTiles, Tile target)
    {
        Tile closestTile = null;
        float closestDist = float.MaxValue; //Reset distance
        foreach (var t in avaliableTiles)
        {
            float distToTile = Vector2Int.Distance(target.GetCoords(), t.GetCoords());
            if (distToTile < closestDist)
            {
                closestDist = distToTile;
                closestTile = t;
            }
        }
        return closestTile;
    }

    /*This function will look at all units in melee range
     *If the enemy flag is set the enemy with the highest threat will be returned
     * If the flag is not set the ally with the highest threat will be retruned
     * If no units are in range that meet the selected critera null will be returned
     */
    private Unit GetHighestThreatInMeleeRange(Team team)
    {
        return null;
    }


}
