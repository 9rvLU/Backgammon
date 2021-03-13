using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Backgammon
{
    public class BoardHandler : MonoBehaviour
    {
        // フィールド
        private Board _board = new Board();


        // プロパティ


        // Start is called before the first frame update
        void Start()
        {


            var white = "white";
            var black = "black";

            _board.ChipsCollection["white"][0] = 100;
            Debug.Log(string.Join("\n", _board.ChipsCollection["white"]));


            _board.MoveChip(white, 6, 9);
            _board.MoveChip(black, 12, 9); // hit!

            _board.ShowProperty();

            _board.MoveChip(white, 25, 8); // enter
            _board.MoveChip(white, 8, 0); // bear off

            _board.ShowProperty();


            Debug.Log(string.Join("\n", _board.ChipsCollection["white"]));
            //Debug.Log(string.Join("\n", BeforePointCollection["white"]));
        }
        // Update is called once per frame
        void Update()
        {

        }

        // メソッド
        public int GetAfterPoint(string color, int beforePoint, int dice)
        {
            var increasablePoints = new List<bool>();
            foreach (var item in _board.ChipsCollection[color])
            {
                increasablePoints.Add(2 > item);
            }


            var afterPoint = beforePoint;
            var allChipsAreInHome = false;
            var distanceToGoal = 25;
            var goalIndex = 0;
            switch (color)
            {
                case "white":
                    afterPoint = beforePoint - dice;
                    allChipsAreInHome = 15 == _board.CountChips(0, 6);
                    goalIndex = 0;
                    distanceToGoal = increasablePoints.FindIndex(value => true == value);
                    break;
                case "black":
                    afterPoint = beforePoint + dice;
                    allChipsAreInHome = 15 == _board.CountChips(19, 25);
                    goalIndex = 25;
                    distanceToGoal = goalIndex - increasablePoints.FindLastIndex(value => true == value);
                    break;
            }


            if (0 < afterPoint && afterPoint < 25)
            {
                if (increasablePoints[afterPoint])
                {
                    return afterPoint;
                }
            }
            else if (0 == afterPoint || 25 == afterPoint)
            {
                if (allChipsAreInHome)
                {
                    return afterPoint;
                }
            }
            else
            {
                if (allChipsAreInHome && distanceToGoal <= dice)
                {
                    return goalIndex;
                }
            }
            return beforePoint;
        }
        public void LoadHand()
        {

        }
        public void LoadRecord()
        {

        }
    }
  
    public class Player
    {
        // プロパティ
        public string Color { get; private set; } = "white";
        public string Other { get; private set; } = "black";


        // メソッド
        public void Init(string color)
        {
            if (Other == color)
            {
                this.Toggle();
            }
        }
        public bool Toggle()
        {
            if ("white" == Color)
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

        private Dictionary<string, (string Color, int ChipNum)> _bar = new Dictionary<string, (string, int)>();
        private Dictionary<string, (string Color, int ChipNum)> _goal = new Dictionary<string, (string, int)>();
        

        // プロパティ
        public Dictionary<string, List<int>> ChipsCollection { get; private set; } = new Dictionary<string, List<int>>();


        // コンストラクタ
        public Board()
        {
            ChipsCollection.Add("white", new List<int>());
            ChipsCollection.Add("black", new List<int>());


            _bar.Add("white", ("white", 0));
            _bar.Add("black", ("black", 0));


            _goal.Add("white", ("white", 0));
            _goal.Add("black", ("black", 0));


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
                        _point[0] = _goal["white"];
                        _point[25] = _bar["white"];
                        break;
                    case "black":
                        _point[25] = _goal["black"];
                        _point[0] = _bar["black"];
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
        public void Initialize()
        {
            // すべてのマスについてコマの数を0にする
            for (int i = 0; i < _point.Length; i++)
            {
                _point[i] = ("white", 0);
            }


            // コマの初期配置を定義
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

                _point[25 - i].Color = "black";
                _point[25 - i].ChipNum += 1;
            }


            // ChipCollectionを更新
            this.UpdateChipsCollection();
        }
        public int CountChips(int beforePoint, int afterPoint)
        {
            var sum = 0;
            for (var i = beforePoint; i < afterPoint + 1; i++)
            {
                sum += _point[i].ChipNum;
            }

            return sum;
        }
        public void MoveChip(string color, int beforePoint, int afterPoint)
        {
            // 色にあわせてバーとゴールを代入
            switch (color)
            {
                case "white":
                    _point[0] = _goal["white"];
                    _point[25] = _bar["white"];
                    break;
                case "black":
                    _point[25] = _goal["black"];
                    _point[0] = _bar["black"];
                    break;
            }


            // 条件を確認
            if ((_point[beforePoint].Color != color) || (_point[beforePoint].ChipNum == 0))
            {
                Debug.LogWarning($"{beforePoint}=>{afterPoint} : {color} はこのポイントからチップを取れません。");
                return;
            }
            if ((color != _point[afterPoint].Color) && (_point[afterPoint].ChipNum > 1))
            {
                Debug.LogWarning($"{beforePoint}=>{afterPoint} : {color}はこのポイントにチップを置けません。");
                return;
            }


            // コマの操作
            // コマを減らす
            _point[beforePoint].ChipNum--;
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


            // バーとゴールの情報を戻す
            switch (color)
            {
                case "white":
                    _goal["white"] = _point[0];
                    _bar["white"] = _point[25];
                    break;
                case "black":
                    _goal["black"] = _point[25];
                    _bar["black"] = _point[0];
                    break;
            }


            // バーの数を更新する
            // コマの数は15個なので、ヒットされて数が減った分だけバーを増やす
            string[] colors = { "white", "black" };
            foreach(var item in colors){
                int sum = 0;
                for (int i = 1; i < _point.Length - 1; i++)
                {
                    if (item == _point[i].Color)
                    {
                        sum += _point[i].ChipNum;
                    }
                }
                _bar[item] = (item, 15 - sum - _goal[item].ChipNum);
            }
            

            // ChipsCollectionを更新
            this.UpdateChipsCollection();
        }
        public void ShowProperty()
        {
            string log = "\n";


            // pointの情報を追加
            for (int i = 1; i < _point.Length - 1; i++)
            {
                log += $"{i} {_point[i].Color} : {_point[i].ChipNum}\n";
            }


            // pointの合計
            int sum = 0;
            for (int i = 1; i < _point.Length - 1; i++)
            {
                if ("white" == _point[i].Color)
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
            log += $"\nwhite bar : {_bar["white"].ChipNum}";
            log += $"\nblack bar : {_bar["black"].ChipNum}";
            log += "\n";


            // goal
            log += $"\nwhite goal : {_goal["white"].ChipNum}";
            log += $"\nblack goal : {_goal["black"].ChipNum}";
            log += "\n";


            // 描画
            Debug.Log(log);
        }
    }
}