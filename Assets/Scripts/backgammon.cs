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

        b.MoveChip(color.white.ToString(), 5, 8);
        b.MoveChip(color.black.ToString(), 11, 8);

        b.ShowProperty();

        Debug.Log(b.canGoal(color.black.ToString()));

        List<bool> array = new List<bool>();
        string log = "\n";
        b.GetDecreasablePoints(color.black.ToString(), ref array);
        foreach(bool isX in array)
        {
            log += $"{isX}\n";
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
    private Dictionary<string, List<bool>> _increasablePoint = new Dictionary<string, List<bool>>();
    private Dictionary<string, List<bool>> _decreasablePoint = new Dictionary<string, List<bool>>();
    private Dictionary<string, int> _chipNum = new Dictionary<string, int>();

    // コンストラクタ
    public Quadrant()
    {
        for(int i = 0; i < 6; i++)
        {
            _points[i] = new Point();
        }

        _increasablePoint.Add("white", new List<bool>());
        _increasablePoint.Add("black", new List<bool>());

        _decreasablePoint.Add("white", new List<bool>());
        _decreasablePoint.Add("black", new List<bool>());

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
            _increasablePoint["white"].Add(_points[i].canIncreaseChip("white"));
            _increasablePoint["black"].Add(_points[i].canIncreaseChip("black"));

            _decreasablePoint["white"].Add(_points[i].canDecreaseChip("white"));
            _decreasablePoint["black"].Add(_points[i].canDecreaseChip("black"));
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

    public void GetIncreasablePoints(string color, ref List<bool> array)
    {
        this.UpdateEnablePoints();
        array = _increasablePoint[color];
       
    }
    public void GetDecreasablePoints(string color, ref List<bool> array)
    {
        this.UpdateEnablePoints();
        array = _decreasablePoint[color];
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
    private void UpdateBar()
    {
        string[] colors = { "white", "black" };
        foreach(string color in colors)
        {
            _bar[color] = 15 - this.CountChips(color) - _goal[color];
        }
    }

    public bool MoveChip(string color, int beforePoint, int afterPoint)
    {
        if(!this.DecreaseChip(color, beforePoint))
        {
            Debug.LogWarning("chipの移動を中止しました。");
            return false;
        }
        this.IncreaseChip(color, afterPoint);

        this.UpdateBar();

        return true;
    }
    public int GetBarChips(string color)
    {
        return _bar[color];
    }
    public int GetGoalChips(string color)
    {
        return _goal[color];
    }
    public void GetIncreasablePoints(string color, ref List<bool> array)
    {
        List<bool> tmp = new List<bool>();
        foreach(Quadrant quad in _quadrants)
        {
            quad.GetIncreasablePoints(color, ref tmp);
            array.AddRange(tmp);
        }
    }
    public void GetDecreasablePoints(string color, ref List<bool> array)
    {
        List<bool> tmp = new List<bool>();
        foreach (Quadrant quad in _quadrants)
        {
            quad.GetDecreasablePoints(color, ref tmp);
            array.AddRange(tmp);
        }
    }
    public bool canGoal(string color)
    {
        switch (color)
        {
            case "white":
                if(15 == _quadrants[3].CountChips("white"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                // break;
            case "black":
                if (15 == _quadrants[0].CountChips("black"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                // break;
            default:
                Debug.LogError($"関数canGoalに不正な値color={color}が渡されました。");
                return false;
                // break;
        }
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