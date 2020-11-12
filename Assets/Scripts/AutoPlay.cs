using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlay : MonoBehaviour
{
    public gameManager game;    //ゲームの進行管理 & 手札の管理
    public cardManager card;    //カード全体(52枚)の管理
    public gameRule rule;       //ゲームのルール(役や得点)

    public gameRule.eHand pastHand; //前ゲームでの役
    public int pastScore;   //前ゲームでのスコア
    public cardManager.eKind currentMark;
    public gameRule.eHand currentHand;

    struct Card
    {
        public int num;
        public int mark;
        public bool leave;
    };
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //カードの"かえる/のこす"を判定して、設定します。
    public void evaluate()
    {
        Change_Card();
    }

    Card[] CheckCard(Card[] cards)
    {
        bool straight = true;
        bool flush = true;
        for (int i = 1; i < cards.Length; i++)
        {
            if (cards[i - 1].num + 1 != cards[i].num)
            {
                straight = false;
                break;
            }
        }
        for (int i = 1; i < cards.Length; i++)
        {
            if (cards[i - 1].mark != cards[i].mark)
            {
                flush = false;
                break;
            }
        }
        if (straight || flush)
        {
            for (int i = 0; i < 5; i++)
            {
                cards[i].leave = true;
            }
        }
        else
        {
            int[] n = new int[15];
            for (int i = 0; i < cards.Length; i++)
            {
                n[cards[i].num]++;
            }
            bool four = false;
            bool three = false;
            int[] pairs = new int[2];
            int pair = 0;
            int threeCard = 0;
            int fourCard = 0;
            for (int i = 2; i < n.Length; i++)
            {
                if (n[i] == 4)
                {
                    four = true;
                    fourCard = i;
                    break;
                }
                else if (n[i] == 3)
                {
                    three = true;
                    threeCard = i;
                }
                else if (n[i] == 2)
                {
                    pairs[pair] = i;
                    pair++;
                }
            }
            if (four)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (cards[i].num == fourCard)
                    {
                        cards[i].leave = true;
                    }
                }
            }
            if (three)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (cards[i].num == threeCard)
                    {
                        cards[i].leave = true;
                    }
                }
            }
            if (pair == 2)
            {
                float prob_A = card.remain(pairs[0]) / card.remain();
                prob_A *= 100;
                print(prob_A);
                float prob_B = pairs[1] / card.remain();
                prob_B *= 100;
                print(prob_B);
                for (int i = 0; i < 5; i++)
                {
                    if (cards[i].num == pairs[pair - 1])
                    {
                        cards[i].leave = true;
                    }
                    else if (cards[i].num == pairs[pair - 2])
                    {
                        cards[i].leave = true;
                    }
                }
            }
            else if (pair == 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (cards[i].num == pairs[pair - 1])
                    {
                        cards[i].leave = true;
                    }
                }
            }
            else if(pair == 0 && !three && !four)
            {
                int[] c = new int[5];
                int remain = 0;
                int remain_num = 0;
                for (int i = 0; i < 5; i++)
                {
                    c[i] = card.remain(cards[i].num);
                    if(c[i] > remain)
                    {
                        remain = c[i];
                        remain_num = i;
                    }
                }
                if (remain == 0)
                    cards[4].leave = true;
                else
                    cards[remain_num].leave = true;
            }
        }
        return cards;
    }

    void Change_Card()
    {
        Card[] cards = new Card[5];
        
        for (int i = 0; i < 5; i++)
        {
            cards[i].num = game.getHandNumber(i);  //i番目の手札の番号を取得
            cards[i].mark = game.getHandMark(i);
            cards[i].leave = false;
        }
        cards = CheckCard(cards);
        for (int i = 0; i < 5; i++)
        {
            if (cards[i].leave)
                game.changeCardState(i);
        }
    }

    void Change_Random()
    {
        //各手札をランダムで残す
        for (int i = 0; i < 5; i++)
        {
            int n = game.getHandNumber(i);  //i番目の手札の番号を取得
            //手札の数字は2～A(14)

            //数字の大きさに依存した確立を作る(20%~80%)
            float r = (float)(n - 2) / 12.0f;
            r = Mathf.Lerp(0.2f, 0.8f, r);

            //確立により、ランダムで手札を残すか決定
            bool keep = (Random.value < r);

            if (keep)
            {
                game.changeCardState(i);    //i番目の手札を"のこす"に設定
            }
        }
    }
}
