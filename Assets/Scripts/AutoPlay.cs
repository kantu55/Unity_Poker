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
        public bool straight;
    };
    

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 10.0f;
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
        int straightCount = 0;
        bool straightReach = true;
        int wrong_num = 0;

        for (int i = 1; i < cards.Length; i++)
        {
            if (cards[i - 1].num + 1 != cards[i].num)
            {
                if(straightCount > 0)
                {
                    wrong_num = i - 1;
                }
                straightCount++;
                straight = false;
            }
        }
        if(straightCount <= 2 && !straight)
        {
            int count = 0;
            int n = 0;
            int leave_num = 0;
            cards[0].straight = true;
            if(wrong_num == 0)
            {
                if(cards[0].num == cards[1].num - 1)
                {
                    wrong_num = 4;
                }
                else
                {
                    n = cards[0].num;
                    cards[0].num = cards[1].num - 1;
                    leave_num = 0;
                    cards[0].straight = false;
                }
            }
            for (int i = 1; i < cards.Length; i++)
            {
                cards[i].straight = false;
                if (cards[0].num + i != cards[i].num)
                {
                    if (count == 0)
                    {
                        n = cards[i].num;
                        cards[i].num = cards[0].num + i;
                        leave_num = i;
                        count++;
                    }
                }
                else
                {
                    cards[i].straight = true;
                }
                if (cards[i - 1].num + 1 != cards[i].num)
                {
                    straightReach = false;
                    cards[i - 1].num = n;
                    break;
                }
            }
            if (straightReach)
            {
                Time.timeScale = 0.5f;
                float prob = (float)card.remain(cards[leave_num].num) / (float)card.remain() * 100.0f;
                if (leave_num == 0 || leave_num == 4)
                    prob *= 2.0f;
                if (prob >= 7)
                {
                    for (int i = 0; i < cards.Length; i++)
                    {
                        if (cards[i].straight)
                        {
                            cards[i].leave = true;
                        }
                    }
                }
                else
                    straightReach = false;
                cards[leave_num].num = n;
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
        else if(!straightReach)
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
                float prob_A = (float)card.remain(pairs[0]) / (float)card.remain() * 100.0f;
                float prob_B = (float)card.remain(pairs[1]) / (float)card.remain() * 100.0f;
                int prob = 0;
                int remain_num = 0;
                if (prob_A > prob_B)
                {
                    prob = (int)prob_A;
                    remain_num = 0;
                }
                else
                {
                    prob = (int)prob_B;
                    remain_num = 1;
                }
                if(prob <= 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (cards[i].num == pairs[0])
                        {
                            cards[i].leave = true;
                        }
                        else if (cards[i].num == pairs[1])
                        {
                            cards[i].leave = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (cards[i].num == pairs[remain_num])
                        {
                            cards[i].leave = true;
                        }
                    }
                }
            }
            else if (pair == 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (cards[i].num == pairs[0])
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
            cards[i].straight = false;
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
