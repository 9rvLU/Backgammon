using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgammon : MonoBehaviour
{
    // Start is called before the first frame update

    enum color
    {
        white,
        black
    }

    void Start()
    {
        Point p = new Point();

       //
        p.IncreaseChip(color.black.ToString());
        p.IncreaseChip(color.white.ToString());
        p.IncreaseChip(color.white.ToString());
        p.IncreaseChip(color.black.ToString());
        p.DecreaseChip(color.black.ToString());
        p.DecreaseChip(color.black.ToString());
        p.DecreaseChip(color.white.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Point
{
    // メンバ変数
    private string _color = "black";
    private int _chips = 0;

    // メンバ関数
    public void IncreaseChip(string color)
    {
        if (!this.canIncreaseChip(color))
        {
            Debug.LogWarning(color + "はこのポイントにチップを置けません。");
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
        this.ShowPointProperty();
    }
    public void DecreaseChip(string color)
    {
        if(!this.canDecreaseChip(color))
        {
            Debug.LogWarning(color + "はこのポイントからチップを取れません。");
            return;
        }
        _chips -= 1;
        this.ShowPointProperty();
    }
    public string GetColor()
    {
        return _color;
    }
    public int GetChipNum()
    {
        return _chips;
    }
    public void ShowPointProperty()
    {
        Debug.Log("color : " + _color + "        chips : " + _chips);
    }

    public bool canIncreaseChip(string color)
    {
        if((_chips > 1) && (_color != color))
        {
            return false;
        }
        return true;
    }
    public bool canDecreaseChip(string color)
    {
        if((_chips == 0) || (_color != color))
        {
            return false;
        }
        return true;
    }
}

public class Quadrant
{
    private Point[] _points = new Point[6];
    private int _whiteChips = 0;
    private int _blackChips = 0;

    public void UpdateChips()
    {
        _whiteChips = 0;
        _blackChips = 0;

        foreach(Point point in _points)
        {
            switch (point.GetColor())
            {
                case "white":
                    _whiteChips += point.GetChipNum();
                    break;
                case "black":
                    _blackChips += point.GetChipNum();
                    break;
            }
        }
    }
}

public class Board
{
    // メンバ変数
    private Quadrant[] _quadrants = new Quadrant[4];
    private int _boreWhitechips;
    private int _boreBlackchips;
}