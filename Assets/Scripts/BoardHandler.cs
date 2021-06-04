using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Backgammon
{
    public class BoardHandler : MonoBehaviour, IChipMover
    {
        // フィールド
        private Board _board = new Board();


        // プロパティ


        // Start is called before the first frame update
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }


        // メソッド
        public int GetAfterPoint(string color, int beforePoint, int dice)
        {
            // コマが置けるマスのリストを作成
            var increasablePoints = new List<bool>();
            var opponent = "";
            switch (color)
            {
                case "white":
                    opponent = "black";
                    break;
                case "black":
                    opponent = "white";
                    break;
                default:
                    Debug.LogError($"BoardHandler: GetAfterPoint に不正な引数color = {color}が代入されました。");
                    return beforePoint;
            }
            foreach (var item in _board.ChipsCollection[opponent])
            {
                increasablePoints.Add(2 > item);
            }


            // 色ごとに違う処理をする値を変数にラップ
            var afterPoint = beforePoint;
            var allChipsAreInHome = false;
            var distanceToGoal = 25;
            var goalPoint = 0;
            switch (color)
            {
                case "white":
                    afterPoint = beforePoint - dice;
                    allChipsAreInHome = 15 == _board.CountChips(0, 6);
                    goalPoint = 0;
                    distanceToGoal = increasablePoints.FindIndex(value => true == value);
                    break;
                case "black":
                    afterPoint = beforePoint + dice;
                    allChipsAreInHome = 15 == _board.CountChips(19, 25);
                    goalPoint = 25;
                    distanceToGoal = goalPoint - increasablePoints.FindLastIndex(value => true == value);
                    break;
                default:
                    Debug.LogError($"BoardHandler: GetAfterPoint に不正な引数color = {color}が代入されました。");
                    return beforePoint;
            }


            // 0 <= afterPoint <= 25 のとき
            // 移動先のマスにコマが置ける状態ならば移動先のマスIDを返す
            if (0 < afterPoint && afterPoint < 25)
            {
                if (increasablePoints[afterPoint])
                {
                    return afterPoint;
                }
            }
            // afterPoint == 0 or 25 のとき
            // ゴール（bear off）が可能（すべてのコマが自陣にある）ならばゴールマスを返す
            else if (0 == afterPoint || 25 == afterPoint)
            {
                if (allChipsAreInHome)
                {
                    return afterPoint;
                }
            }
            // afterPoint < 0 or 25 < afterPoint のとき
            // ゴールとゴールからもっとも遠いコマがダイス目以下しか離れていない、かつ
            // すべてのコマが自陣にあるとき、ゴールマスを返す
            else
            {
                if (allChipsAreInHome && distanceToGoal <= dice)
                {
                    return afterPoint;
                }
            }


            // 全部に当てはまらないとき、もとのマスIDを返す
            return beforePoint;
        }
        public IEnumerable<int> GetBeforePointCollection(string color)
        {
            // バーにコマがないことをチェック
            var BarPointIndex = 0;
            var GoalPointIndex = 0;
            var BarHasChip = false;
            switch (color)
            {
                case "white":
                    BarPointIndex = 25;
                    GoalPointIndex = 0;
                    break;
                case "black":
                    BarPointIndex = 0;
                    GoalPointIndex = 25;
                    break;
            }
            if (0 < _board.ChipsCollection[color][BarPointIndex])
            {
                BarHasChip = true;
            }


            // バーにコマがあるとき、バー以外のすべてのマスが選択不可能
            if (BarHasChip)
            {
                yield return BarPointIndex;
            }
            // バーにコマがないとき、ゴール以外のコマを1つ以上持っているマスを返す
            else
            {
                for (var i = 0; i < _board.ChipsCollection[color].Count(); i++)
                {
                    if (GoalPointIndex != i && 0 < _board.ChipsCollection[color][i])
                    {
                        yield return i;
                    }
                }
            }
        }
        public void MoveChip(string color, int beforePoint, int afterPoint)
        {
            _board.MoveChip(color, beforePoint, afterPoint);
        }
        //public void LoadRecord()
        //{

        //}
        public bool isCloseOut(string color)
        {
            var other = "";
            var opponentHomeStart = -1;
            var opponentHomeEnd = -1;
            var barID = -1;
            switch (color)
            {
                case "white":
                    other = "black";
                    opponentHomeStart = 19;
                    opponentHomeEnd = 24;
                    barID = 25;
                    break;
                case "black":
                    other = "white";
                    opponentHomeStart = 1;
                    opponentHomeEnd = 6;
                    barID = 0;
                    break;
            }


            var result = true;
            result &= 0 != _board.ChipsCollection[color][barID];
            for (var i = opponentHomeStart; i <= opponentHomeEnd; i++)
            {
                result &= 1 < _board.ChipsCollection[other][i];
            }


            return result;
        }
        public bool cannotMoveChip(string color, int dice)
        {
            var result = true;
            foreach(var item in this.GetBeforePointCollection(color))
            {
                result &= item == this.GetAfterPoint(color, item, dice);
            }


            return result;
        }
        public bool isWinner(string color)
        {
            var goalCount = 0;
            switch (color)
            {
                case "white":
                    goalCount = _board.ChipsCollection[color][0];
                    break;
                case "black":
                    goalCount = _board.ChipsCollection[color][25];
                    break;
            }


            return 15 == goalCount;
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
        public void Initialize(int[] initialChips)
        {
            // すべてのマスについてコマの数を0にする
            for (int i = 0; i < _point.Length; i++)
            {
                _point[i] = ("white", 0);
            }


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
        public void Initialize()
        {
            // コマの初期配置を定義
            int[] initialChips = new int[15] {
            6, 6, 6, 6,6,
            8, 8, 8,
            13, 13, 13, 13, 13,
            24, 24
            };


            this.Initialize(initialChips);
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


// チップの移動用メッセージインターフェース
public interface IChipMover : IEventSystemHandler
{
    void MoveChip(string player, int before, int after);
    int GetAfterPoint(string player, int before, int dice);
}