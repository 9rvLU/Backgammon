using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Backgammon
{
    public class GameManager : MonoBehaviour
    {
        // シングルトンにする処理
        public static GameManager instance;


        // field
        bool _isChanged = false;


        private enum _gameProcess : int
        {
            DiceRoll,
            SelectingBeforePoint,
            SelectingAfterPoint,
            Result
        }
        private _gameProcess _currentGameProcess = _gameProcess.DiceRoll;


        private List<int> _rolls = new List<int>();


        private Dice _dice = new Dice();
        private BoardHandler _boardHandler = new BoardHandler();


        // property
        public string CurrentPlayer { get; private set; } = "";
        public int CurrentBeforePoint { get; private set; } = 0;
        public int CurrentAfterPoint { get; private set; } = 0;


        // awake
        void Awake()
        {
            // シングルトンにする処理
            instance = this;


            // 参照
            _dice = GameObject.Find("Scripts/Dice").GetComponent<Dice>();
            _boardHandler = GameObject.Find("Scripts/BoardHandler").GetComponent<BoardHandler>();
        }
        // Update is called once per frame
        void Update()
        {
            if (!_isChanged)
            {
                return;
            }


            switch (_currentGameProcess)
            {
                case _gameProcess.DiceRoll:
                    this.DiceRoll();
                    break;
                case _gameProcess.SelectingBeforePoint:
                    break;
                case _gameProcess.SelectingAfterPoint:
                    break;
                default:
                    break;
            }


            _isChanged = false;
        }


        // method
        public void DiceRoll()
        {
            // 先手が決まっていないとき、先手を決定する
            if ("" == CurrentPlayer)
            {
                // ダイス目がゾロ目でなくなるまで振り直す
                while (_dice.isDoubles())
                {
                    _dice.Roll();
                }

                // ダイスの目に応じて先手決定
                if(_dice.Roll1 > _dice.Roll2)
                {
                    CurrentPlayer = "white";
                }
                else
                {
                    CurrentPlayer = "black";
                }
            }
            else
            {
                _dice.Roll();
            }


            // 出目を保持
            _rolls.Clear();
            foreach (var roll in _dice.GetRolls())
            {
                _rolls.Add(roll);
            }


            // クローズアウトの処理
            var isCloseOut = false;
            isCloseOut = _boardHandler.isCloseOut(CurrentPlayer);

            // プレイヤーを交代する
            if (isCloseOut) 
            {
                switch (CurrentPlayer)
                {
                    case "white":
                        CurrentPlayer = "black";
                        break;
                    case "black":
                        CurrentPlayer = "white";
                        break;
                }
                return;
            }


            // ダンスの処理
            var isDance = true;
            foreach(var roll in _rolls)
            {
                isDance &= _boardHandler.cannotMoveChip(CurrentPlayer, roll);
            }

            // プレイヤーを交代し、ダイスロールプロセスに戻る
            if (isDance)
            {
                switch (CurrentPlayer)
                {
                    case "white":
                        CurrentPlayer = "black";
                        break;
                    case "black":
                        CurrentPlayer = "white";
                        break;
                }
                _currentGameProcess = _gameProcess.DiceRoll;
                return;
            }


            // ゲームの状態をすすめる
            _currentGameProcess = _gameProcess.SelectingBeforePoint;
        }
        public void SelectBeforePoint(int beforePoint)
        {
            CurrentBeforePoint = beforePoint;


            _currentGameProcess = _gameProcess.SelectingAfterPoint;
        }
        public void SelectAfterPoint(int afterPoint)
        {
            // 移動がキャンセルされたとき
            if (CurrentBeforePoint == afterPoint)
            {
                _currentGameProcess = _gameProcess.SelectingBeforePoint;
                return;
            }


            // 移動処理
            _boardHandler.MoveChip(CurrentPlayer, CurrentBeforePoint, afterPoint);


            // ダイス目を消費
            int dice = Mathf.Abs(afterPoint - CurrentBeforePoint);
            _rolls.Remove(dice);


            //  勝利判定
            if (_boardHandler.isWinner(CurrentPlayer))
            {
                _currentGameProcess = _gameProcess.Result;
            }


            // ダイスの目が残っているとき
            if (0 < _rolls.Count)
            {
                _currentGameProcess = _gameProcess.SelectingBeforePoint;
            }
            else
            {
                switch (CurrentPlayer)
                {
                    case "white":
                        CurrentPlayer = "black";
                        break;
                    case "black":
                        CurrentPlayer = "white";
                        break;
                }
                _currentGameProcess = _gameProcess.DiceRoll;
            }
        }
    }
}