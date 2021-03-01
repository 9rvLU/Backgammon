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
        Board b = new Board();

        b.moveChip(color.white.ToString(), 5, 7);

        b.ShowProperty();
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
    public bool IncreaseChip(string color)
    {
        if (!this.canIncreaseChip(color))
        {
            Debug.LogWarning(color + "はこのポイントにチップを置けません。");
            return false;
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
        return true;
    }
    public bool DecreaseChip(string color)
    {
        if(!this.canDecreaseChip(color))
        {
            Debug.LogWarning(color + "はこのポイントからチップを取れません。");
            return false;
        }
        _chips -= 1;
        // this.ShowProperty();
        return true;
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
    public string GetProperty()
    {
        string log = "";
        log += $"color : {_color}";
        log += $"        chips : {_chips}\n";

        // Debug.Log(log);

        return log;
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
    private Dictionary<string, int> _chipNum = new Dictionary<string, int>();

    // コンストラクタ
    public Quadrant()
    {
        for(int i = 0; i < 6; i++)
        {
            _points[i] = new Point();
        }

        _chipNum.Add("white", 0);
        _chipNum.Add("black", 0);
    }

    // メンバ関数
    private void UpdateChips()
    {
        _chipNum["white"] = 0;
        _chipNum["black"] = 0;

        foreach (Point point in _points)
        {
            _chipNum[point.GetColor()] += point.GetChipNum();
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

    public bool IncreaseChip(string color, int itr)
    {
        bool hasDone;
        hasDone = _points[itr % 6].IncreaseChip(color);
        this.UpdateChips();

        return hasDone;
    }
    public bool DecreaseChip(string color, int itr)
    {
        bool hasDone;
        hasDone = _points[itr % 6].DecreaseChip(color);
        this.UpdateChips();

        return hasDone;
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
    public int CountChips(string color)
    {
        return _chipNum[color];
    }
    public string GetProperty()
    {
        string log = "\n";

        foreach (Point point in _points)
        {
            log += point.GetProperty();
        }
        log += $"white chips : { _chipNum["white"]}\n";
        log += $"black chips : { _chipNum["black"]}\n";

        // Debug.Log(log);

        return log;
    }
}

public class Board
{
    // メンバ変数
    private Quadrant[] _quadrants = new Quadrant[4];
    private Dictionary<string, int> _bar = new Dictionary<string, int>();
    private Dictionary<string, int> _goal = new Dictionary<string, int>();

    // コンストラクタ
    public Board()
    {
        for (int i = 0; i < 4; i++)
        {
            _quadrants[i] = new Quadrant();
        }

        _bar.Add("black", 0);
        _bar.Add("white", 0);
        _goal.Add("black", 0);
        _goal.Add("white", 0);

        this.initChips();
    }

    // メンバ関数
    private bool IncreaseChip(string color, int itr)
    {
        int quadItr = (int)(itr / 6);
        int pointItr = itr % 6;

        return _quadrants[quadItr].IncreaseChip(color, pointItr);
    }
    private bool DecreaseChip(string color, int itr)
    {
        int quadItr = (int)(itr / 6);
        int pointItr = itr % 6;

        return _quadrants[quadItr].DecreaseChip(color, pointItr);
    }
    private void initChips()
    {
        int[] initialChips = new int[15] {
            5, 5, 5, 5, 5,
            7, 7, 7,
            12, 12, 12, 12, 12,
            23, 23
        };
        foreach(int i in initialChips)
        {
            this.IncreaseChip("white", i);
            this.IncreaseChip("black", 23-i);
        }
    }
    private int CountChips(string color)
    {
        int sum = 0;

        foreach(Quadrant quad in _quadrants)
        {
            sum += quad.CountChips(color);
        }
        return sum;
    }

    public bool moveChip(string color, int beforePoint, int afterPoint)
    {
        if(!this.DecreaseChip(color, beforePoint))
        {
            Debug.LogWarning("chipの移動を中止しました。");
            return false;
        }
        this.IncreaseChip(color, afterPoint);

        return true;
    }
    public void ShowProperty()
    {
        string log = "\n";

        foreach (Quadrant quad in _quadrants)
        {
            log += quad.GetProperty();
        }

        log += $"\ntotal white chips : {this.CountChips("white")}";
        log += $"\ntotal black chips : {this.CountChips("black")}";
        log += "\n";

        log += $"\nwhite bar : {_bar["white"]}";
        log += $"\nblack bar : {_bar["black"]}";
        log += "\n";

        log += $"\nwhite goal : {_goal["white"]}";
        log += $"\nblack goal : {_goal["black"]}";
        log += "\n";

        Debug.Log(log);
    }
}