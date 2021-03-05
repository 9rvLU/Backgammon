using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollsHandler : MonoBehaviour
{
    int _roll1 = 1;
    int _roll2 = 1;
    List<int> _rolls = new List<int>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool isDoubling()
    {
        if(_roll1 == _roll2)
        {
            return true;
        }
        return false;
    }
    private void UpdateRolls()
    {
        _rolls.Clear();

        _rolls.Add(_roll1);
        _rolls.Add(_roll2);

        if (this.isDoubling())
        {
            _rolls.Add(_roll1);
            _rolls.Add(_roll2);
        }
    }

    void SetRolls(int dice1, int dice2)
    {
        _roll1 = dice1;
        _roll2 = dice2;
    }
    public void GetRolls(ref List<int> rolls)
    {
        this.UpdateRolls();
        rolls = new List<int>(_rolls);
    }
}
