using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Backgammon
{
    public class Dice : MonoBehaviour
    {
        // フィールド
        public List<int> _rolls { get; set; } = new List<int>();


        // プロパティ
        public int Roll1 { get; private set; } = 1;
        public int Roll2 { get; private set; } = 1;


        // Start is called before the first frame update
        void Start()
        {
            this.Roll();
        }

        // Update is called once per frame
        void Update()
        {

        }


        public void Roll()
        {
            Roll1 = Random.Range(0, 7);
            Roll2 = Random.Range(0, 7);

            _rolls.Add(Roll1);
            _rolls.Add(Roll2);


            if (this.isDoubles())
            {
                _rolls.Add(Roll1);
                _rolls.Add(Roll2);
            }
        }
        public IEnumerable<int> GetRolls()
        {
            foreach(var roll in _rolls)
            {
                yield return roll;
            }
        }
        public void RemoveRoll(int roll)
        {
            _rolls.Remove(roll);
        }
        public bool isDoubles()
        {
            return Roll1 == Roll2;
        }
        public bool isEmpty()
        {
            return 0 == _rolls.Count;
        }
    }
}