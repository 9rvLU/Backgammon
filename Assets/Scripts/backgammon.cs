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
        Quadrant q = new Quadrant();

        q.IncreaseChip(color.black.ToString(), 0);
        q.IncreaseChip(color.white.ToString(), 1);
        q.IncreaseChip(color.white.ToString(), 2);
        q.IncreaseChip(color.black.ToString(), 0);
        q.DecreaseChip(color.black.ToString(), 2);
        q.DecreaseChip(color.black.ToString(), 5);
        q.DecreaseChip(color.white.ToString(), 3);

        bool[] array = new bool[6];
        q.GetDecreasablePoints(color.black.ToString(), ref array);

        string log = "\n";
        foreach(bool b in array)
        {
            log += $"{b}\n";
        }
        Debug.Log(log);
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
    private void ShowProperty()
    {
        string log = "\n";
        log += $"color : {_color}\n";
        log += $"chips : {_chips}\n";

        Debug.Log(log);
    }

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
        // this.ShowProperty();
    }
    public void DecreaseChip(string color)
    {
        if(!this.canDecreaseChip(color))
        {
            Debug.LogWarning(color + "はこのポイントからチップを取れません。");
            return;
        }
        _chips -= 1;
        // this.ShowProperty();
    }
    public string GetColor()
    {
        return _color;
    }
    public int GetChipNum()
    {
        return _chips;
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
    // メンバ変数
    private Point[] _points = new Point[6];
    private bool[] _increasableWhitePoint = new bool[6];
    private bool[] _increasableBlackPoint = new bool[6];
    private bool[] _decreasableWhitePoint = new bool[6];
    private bool[] _decreasableBlackPoint = new bool[6];
    private int _whiteChips = 0;
    private int _blackChips = 0;

    // コンストラクタ
    public Quadrant()
    {
        for(int i = 0; i < 6; i++)
        {
            _points[i] = new Point();
        }
    }

    // メンバ関数
    private void UpdateChips()
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
    private void UpdateEnablePoints()
    {
        for (int i = 0; i < 6; i++)
        {
            _increasableWhitePoint[i] = _points[i].canIncreaseChip("white");
            _increasableBlackPoint[i] = _points[i].canIncreaseChip("black");

            _decreasableWhitePoint[i] = _points[i].canDecreaseChip("white");
            _decreasableBlackPoint[i] = _points[i].canDecreaseChip("black");
        }
    }

    public void IncreaseChip(string color, int itr)
    {
        _points[itr % 6].IncreaseChip(color);
        this.UpdateChips();
        this.ShowProperty();
    }
    public void DecreaseChip(string color, int itr)
    {
        _points[itr % 6].DecreaseChip(color);
        this.UpdateChips();
        this.ShowProperty();
    }

    public void GetIncreasablePoints(string color, ref bool[] array)
    {
        this.UpdateEnablePoints();
        switch (color)
        {
            case "white":
                array = _increasableWhitePoint;
                break;
            case "black":
                array = _increasableBlackPoint;
                break;
        }
    }
    public void GetDecreasablePoints(string color, ref bool[] array)
    {
        this.UpdateEnablePoints();
        switch (color)
        {
            case "white":
                array = _decreasableWhitePoint;
                break;
            case "black":
                array = _decreasableBlackPoint;
                break;
        }
    }

    public void ShowProperty()
    {
        string log = "\n";

        foreach (Point point in _points)
        {
            log += $"color : {point.GetColor()}  chips : {point.GetChipNum()}\n";
        }
        log += $"white chips : { _whiteChips}\n";
        log += $"black chips : { _blackChips}\n";

        Debug.Log(log);
    }
}

public class Board
{
    // メンバ変数
    private Quadrant[] _quadrants = new Quadrant[4];
    private int _boreWhitechips;
    private int _boreBlackchips;
}