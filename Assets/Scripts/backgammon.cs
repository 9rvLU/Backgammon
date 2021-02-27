using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgammon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Point p = new Point();

        p.showPointProperty();
        p.IncreaseChip(1);
        p.showPointProperty();
        p.IncreaseChip(-1);
        p.showPointProperty();
        p.IncreaseChip(-1);
        p.showPointProperty();
        p.DecreaseChip(-1);
        p.showPointProperty();
        p.DecreaseChip(1);
        p.showPointProperty();
        p.DecreaseChip(-1);
        p.showPointProperty();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Point
{
    // メンバ変数
    private int _color = 1;
    private int _chips = 0;

    // メンバ関数
    public void IncreaseChip(int color)
    {
        if (!this.canIncreaseChip(color))
        {
            Debug.LogWarning("このポイントにチップは置けません。");
            return;
        }

        switch(_chips)
        {
            case 0:
                _color = color;
                _chips += 1;
                break;

            case 1:
                if(_color == color)
                {
                    _chips += 1;
                }
                else
                {
                    _color = color;
                }
                break;

            default:
                _chips += 1;
                break;
        }
    }
    public void DecreaseChip(int color)
    {
        if(!this.canDecreaseChip(color))
        {
            Debug.LogWarning("このポイントからチップを取れません。");
            return;
        }
        _chips -= 1;
    }
    public int getColor()
    {
        return _color;
    }
    public int getChipNum()
    {
        return _chips;
    }
    public void showPointProperty()
    {
        Debug.Log("color : " + _color + "        chips : " + _chips);
    }

    public bool canIncreaseChip(int color)
    {
        if(_chips > 1 & _color != color)
        {
            return false;
        }
        return true;
    }
    public bool canDecreaseChip(int color)
    {
        if(_chips == 0 | _color != color)
        {
            return false;
        }
        return true;
    }
}

public class Quadrant
{
    private Point[] _points = new Point[6];
}

public class Board
{
    // メンバ変数
    private Quadrant[] _quadrants = new Quadrant[4];
    private int _boreWhitechips;
    private int _boreBlackchips;
}