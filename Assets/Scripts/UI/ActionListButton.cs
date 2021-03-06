﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Customize action panel button
//attach onclick function
public class ActionListButton : MonoBehaviour
{
    [SerializeField] private Text ActionText;
    [SerializeField] public BattleManager BM;
    [SerializeField] public int Damage;
    [SerializeField] public int Range;
    [SerializeField] private int Activate;

    //set text for button
    public void SetText(string ActionText, int Damage, int Range, int i)
    {

        if (!(Activate == 1))
        {
            Activate = 1;
            this.gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }
        this.ActionText.text = ActionText;
        this.Damage = Damage;
        this.Range = Range;
    }
    public void OnClick()
    {
        GameObject.Find("ActionScrollList").transform.position = new Vector3(-1000f, 1000f, 0.0f);
        
        if (this.ActionText.text == "Close Attack")
        {
            //BM.AddShortRangeModule();
            Debug.Log("Attack Close" + " -- Power:" + Damage + "  Range:" + Range);
            BM.AddDamage(Damage);
        }
        else if (this.ActionText.text == "Ranged Attack")
        {
            //BM.AddLongRangeModule();
            Debug.Log("Attack at Range" + " -- Power:" + Damage + "  Range:" + Range);
            BM.AddDamage(Damage);
        }
        else if (this.ActionText.text == "Heal")
        {
            //BM.AddHealModule();
            Debug.Log("Heal Target" + " -- Power:" + Damage + "  Range:" + Range);
            BM.AddDamage(Damage);
        }
        else if (this.ActionText.text == "Slow")
        {
            //BM.AddSlowModule();
            Debug.Log("Slow Target" + " -- Power:" + Damage + "  Range:" + Range);
            BM.AddDamage(Damage);
        }
    }
}