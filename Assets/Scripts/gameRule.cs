using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameRule : MonoBehaviour
{
    public enum eHand
    {
        RoyalStraightFlush,
        StraightFlush,
        FourCards,
        FullHouse,
        Flush,
        Straight,
        ThreeCards,
        TwoPair,
        OnePair,
        HighCard,
    }
    public class cHandInfo
    {
        string _name;
        int _score;
        public cHandInfo(string n, int s)
        {
            _name = n;
            _score = s;
        }
        public string name
        {
            get { return _name; }
        }
        public int score
        {
            get { return _score; }
        }
    }
    static public readonly cHandInfo[] HandInfo = new cHandInfo[]
        {
            new cHandInfo("ロイヤルストレートフラッシュ", 10000),
            new cHandInfo("ストレートフラッシュ", 5000),
            new cHandInfo("フォーカード", 2000),
            new cHandInfo("フルハウス", 750),
            new cHandInfo("フラッシュ", 500),
            new cHandInfo("ストレート", 250),
            new cHandInfo("スリーカード", 100),
            new cHandInfo("ツーペア", 50),
            new cHandInfo("ワンペア", 20),
            new cHandInfo("ハイカード", 0),
        };

    public Text[] TextHandName;
    public Text[] TextHandScore;
    // Start is called before the first frame update
    void Start()
    {
        int n = HandInfo.Length / 2;
        int i = 0;
        for(int t=0; t<TextHandName.Length; t++)
        {
            TextHandName[t].text = "";
            TextHandScore[t].text = "";
        }
        foreach(cHandInfo info in HandInfo)
        {
            TextHandName[i / n].text += info.name;
            TextHandName[i / n].text += "\n";
            TextHandScore[i / n].text += info.score!=0 ? info.score.ToString() : "2～14";
            TextHandScore[i / n].text += "\n";
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
