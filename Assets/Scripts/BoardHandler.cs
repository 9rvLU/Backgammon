using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardHandler : MonoBehaviour
{
    // Start is called before the first frame update

    // フィールド
    private Player _player = new Player();
    private Board _board = new Board();

    private int _process;
    private enum _playerProcess
    {
        applyDecreasablePoints = 0,
        selectChip,
        applyIncreasablePoints,
        moveChip
    }

    void Start()
    {
        _board.MoveChip(_player.Color, 6, 9);
        _player.Toggle();
        _board.MoveChip(_player.Color, 12, 9); // hit!

        _board.ShowProperty();
        _player.Toggle();

        _board.MoveChip(_player.Color, 25, 8); // enter
        _board.MoveChip(_player.Color, 8, 0); // bere off

        _board.ShowProperty();


        Debug.Log(string.Join("\n", _board.ChipsCollection["white"]));
        Debug.Log(string.Join("\n", _board.ChipsCollection["black"]));
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // メソッド


}

public class Player
{
    // プロパティ
    public string Color { get; private set; } = "white";
    public string Other { get; private set; } = "black";

    // メソッド
    public void Init(string color)
    {
        if(Other == color)
        {
            this.Toggle();
        }
    }
    public bool Toggle()
    {
        if("white" == Color)
        {
            Color = "black";
            Other = "white";
        }
        else if ("black" == Color)
        {
            Color = "white";
            Other = "black";
        }
        else
        {
            Debug.LogError("PlayerクラスToggle関数でwhite, black以外の値がメンバ変数に代入されています。");
            return false;
        }
        return true;
    }
    
}



public class Board
{
    // フィールド
    private (string Color, int ChipNum)[] _point = new (string, int)[26];

    private (string Color, int ChipNum) _whiteBar = ("white", 0);
    private (string Color, int ChipNum) _blackBar = ("black", 0);

    private (string Color, int ChipNum) _whiteGoal = ("white", 0);
    private (string Color, int ChipNum) _blackGoal = ("black", 0);

    // プロパティ
    public Dictionary<string, List<int>> ChipsCollection { get; private set; } = new Dictionary<string, List<int>>();

    // コンストラクタ
    public Board()
    {
        for (int i = 0; i < _point.Length; i++)
        {
            _point[i] = ("white", 0);
        }

        ChipsCollection.Add("white", new List<int>());
        ChipsCollection.Add("black", new List<int>());


        this.Initialize();
    }

    // メソッド
    private void UpdateChipsCollection()
    {
        string[] colors = { "white", "black" };
        foreach (var color in colors)
        {
            // ChipCollectionを初期化
            ChipsCollection[color].Clear();


            // 色にあわせてバーとゴールを代入
            switch (color)
            {
                case "white":
                    _point[0] = _whiteGoal;
                    _point[25] = _whiteBar;
                    break;
                case "black":
                    _point[25] = _blackGoal;
                    _point[0] = _blackBar;
                    break;
            }


            // 値を代入
            for (int i = 0; i < _point.Length; i++)
            {
                if (color == _point[i].Color)
                {
                    ChipsCollection[color].Add(_point[i].ChipNum);
                }
                else
                {
                    ChipsCollection[color].Add(0);
                }
            }
        }
    }
    private void Initialize()
    {
        // コマの初期配置
        int[] initialChips = new int[15] {
            6, 6, 6, 6,6,
            8, 8, 8,
            13, 13, 13, 13, 13,
            24, 24
        };


        // 初期配置に従ってコマを増やす
        foreach (int i in initialChips)
        {
            _point[i].Color = "white";
            _point[i].ChipNum += 1;

            _point[25-i].Color = "black";
            _point[25-i].ChipNum += 1;
        }


        // ChipCollectionを更新
        this.UpdateChipsCollection();
    }
    public void MoveChip(string color, int beforePoint, int afterPoint)
    {
        // 色にあわせてバーとゴールを代入
        switch (color)
        {
            case "white":
                _point[0] = _whiteGoal;
                _point[25] = _whiteBar;
                break;
            case "black":
                _point[25] = _blackGoal;
                _point[0] = _blackBar;
                break;
        }


        // 条件を確認
        if ((_point[beforePoint].Color != color) || (_point[beforePoint].ChipNum == 0))
        {
            Debug.LogWarning($"{beforePoint}=>{afterPoint} : {color} はこのポイントからチップを取れません。");
            return;
        }
        if((color != _point[afterPoint].Color) && (_point[afterPoint].ChipNum > 1))
        {
            Debug.LogWarning($"{beforePoint}=>{afterPoint} : {color}はこのポイントにチップを置けません。");
            return;
        }


        // コマの操作
        // コマを減らす
        _point[beforePoint].ChipNum --;
        // コマを増やす
        switch (_point[afterPoint].ChipNum)
        {
            case 0:
                _point[afterPoint].Color = color;
                _point[afterPoint].ChipNum++;
                break;
            case 1:
                if (color == _point[afterPoint].Color)
                {
                    _point[afterPoint].ChipNum++;
                }
                else
                {
                    _point[afterPoint].Color = color;
                }
                break;
            default:
                _point[afterPoint].ChipNum++;
                break;
        }


        // 代入したバーとゴールの情報を戻す
        switch (color)
        {
            case "white":
                _whiteGoal = _point[0];
                _whiteBar = _point[25];
                break;
            case "black":
                _blackGoal = _point[25];
                _blackBar = _point[0];
                break;
        }


        // バーの数を更新する
        // コマの数は15個なので、ヒットされて数が減った分だけバーを増やす
        int sum = 0;
        for (int i = 1; i < _point.Length - 1; i++)
        {
            if ("white" == _point[i].Color)
            {
                sum += _point[i].ChipNum;
            }
        }
        _whiteBar.ChipNum = 15 - sum - _whiteGoal.ChipNum;
        sum = 0;
        for (int i = 1; i < _point.Length - 1; i++)
        {
            if ("black" == _point[i].Color)
            {
                sum += _point[i].ChipNum;
            }
        }
        _blackBar.ChipNum = 15 - sum - _blackGoal.ChipNum;


        // ChipsCollectionを更新
        this.UpdateChipsCollection();
    }
    public void ShowProperty()
    {
        string log = "\n";


        // pointの情報を追加
        for (int i = 1; i<_point.Length-1; i++)
        {
            log += $"{i} {_point[i].Color} : {_point[i].ChipNum}\n";
        }


        // pointの合計
        int sum = 0;
        for(int i = 1; i < _point.Length - 1; i++)
        {
            if("white" == _point[i].Color)
            {
                sum += _point[i].ChipNum;
            }
        }
        log += $"\ntotal white chips : {sum}";
        sum = 0;
        for (int i = 1; i < _point.Length - 1; i++)
        {
            if ("black" == _point[i].Color)
            {
                sum += _point[i].ChipNum;
            }
        }
        log += $"\ntotal black chips : {sum}";
        log += "\n";


        // bar
        log += $"\nwhite bar : {_whiteBar.ChipNum}";
        log += $"\nblack bar : {_blackBar.ChipNum}";
        log += "\n";


        // goal
        log += $"\nwhite goal : {_whiteGoal.ChipNum}";
        log += $"\nblack goal : {_blackGoal.ChipNum}";
        log += "\n";


        // 描画
        Debug.Log(log);
    }
}