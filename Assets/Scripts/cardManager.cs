using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class cardManager : MonoBehaviour
{
    public enum eKind
    {
        Club,
        Diamond,
        Heart,
        Spade,
    }
    public class cCard
    {
        public int Number;
        public eKind Kind;
        public cCard(eKind k, int n)
        {
            Kind = k;
            Number = n+2;
        }
    }
    cCard[] Cards;

    List<cCard> Remain;

    void Awake()
    {
        Remain = new List<cCard>();

        Cards = new cCard[13 * 4];

        createCards();
    }

    void createCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Cards[i] = new cCard((eKind)i, j);
                Remain.Add(Cards[i]);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void draw(out int num, out int kind)
    {
        if(Remain.Count==0)
        {
            createCards();
        }
        int c = Random.Range(0, Remain.Count);
        num = Remain[c].Number;
        kind = (int)Remain[c].Kind;

        Remain.RemoveAt(c);
    }

    //カードの残り枚数を取得
    public int remain()
    {
        return Remain.Count;
    }

    //指定した数字のカードの残り枚数を取得
    public int remain(int number)
    {
        int ret = 0;
        foreach(cCard c in Remain)
        {
            if (c.Number == number) ret++;
        }
        return ret;
    }

    //指定したマークのカードの残り枚数を取得
    public int remain(eKind mark)
    {
        int ret = 0;
        foreach (cCard c in Remain)
        {
            if (c.Kind == mark) ret++;
        }
        return ret;
    }
}
