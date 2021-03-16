using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Backgammon
{
    public class Dice : MonoBehaviour
    {
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

        private bool isDoubles()
        {
            return Roll1 == Roll2;
        }

        void Roll()
        {
            Roll1 = Random.Range(0, 7);
            Roll2 = Random.Range(0, 7);
        }
        public IEnumerable<int> GetRolls()
        {
            yield return Roll1;
            yield return Roll2;

            if (this.isDoubles())
            {
                yield return Roll1;
                yield return Roll2;
            }
        }
    }
}