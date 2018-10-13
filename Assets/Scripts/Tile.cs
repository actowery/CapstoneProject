﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
public class Tile : MonoBehaviour {

    public enum TileColors { standard, move, attack }

    private const int gridSize = 10;

    private Vector2Int gridPos;

    [Header("Material Colors")]
    [SerializeField] Color baseColor;
    [SerializeField] Color moveRangeColor;
    [SerializeField] Color attackRangeColor;


    //TODO Possibly remove this. Tile may not need to care if unit is there and GameManager can handle that
    private Unit unitOnTile;

    Renderer[] childrenRenderers;

    //TODO Move this to the search itself
    bool visted = false;

    public bool Visted
    {
        get
        {
            return visted;
        }

        set
        {
            visted = value;
        }
    }

    public static int GridSize
    {
        get
        {
            return gridSize;
        }
    }

    //TODO Clean up coordianate systems
    private void Awake()
    {
        gridPos = GetGridPos();
    }

    // Use this for initialization
    void Start () {
        childrenRenderers = GetComponentsInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

    }

    //Returns Position within the grid (0,0) (3,2) etc
    public Vector2Int GetGridPos()
    {
        return new Vector2Int(
            (int)Mathf.Floor(transform.position.x / gridSize),
            (int)Mathf.Floor(transform.position.z / gridSize)
        );
    }

    //TODO Possibly removed this and multiply GetGridPos by 10 any time actual coords are needed
    //Returns Unity Coordidates of the tile (0,0) (30,20) etc
    public Vector2Int GetCoords()
    {
        return new Vector2Int(
            (int)Mathf.Floor(transform.position.x / gridSize) * gridSize,
            (int)Mathf.Floor(transform.position.z / gridSize) * gridSize
        );
    }

    public void SetColor(Color color)
    {
        Queue<Color> colors = new Queue<Color>();
        foreach (Renderer r in childrenRenderers)
        {
            r.material.color = color;
            colors.Enqueue(color);
        }
        SendMessage("UpdateBaseColorQueue", colors);
    }

    //DEPRECIATED Use SetTileColor instead
    //Update all faces material
    //public void UpdateMaterial(Material material)
    //{
    //    foreach (Renderer renderer in childrenRenderers)
    //    {
    //        renderer.material = material;
    //    }
    //}

    void OnMouseDown(){
        GameManager.instance.TileClicked(this);
	}

    //DEPRECIATED Use ResetTileColor Instead
    //public void ResetTileMaterial()
    //{
    //    UpdateMaterial(BaseMaterial);
    //}

    public void ResetTileColor()
    {
        SetColor(baseColor);
    }



    public void SetTileColor(TileColors color)
    {
        switch (color)
        {
            case TileColors.standard:
                SetColor(baseColor);
                break;
            case TileColors.move:
                SetColor(moveRangeColor);
                break;
            case TileColors.attack:
                SetColor(attackRangeColor);
                break;
            default:
                SetColor(baseColor);
                break;
        }
    }

    private void StartHighlightAnimation()
    {

    }

    private void StopHighlightAnimation()
    {

    }
}
