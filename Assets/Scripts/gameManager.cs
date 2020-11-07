using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    enum eState
    {
        Start,
        Open,
        Sort,
        Wait,
        Draw,
        Open2,
        Sort2,
        Result,
        Reset,
    }
    eState state;

    public GameObject HandRoot;
    public GameObject ButtonRoot;
    public Button ButtonDeal;
    public resultObject resultObj;
    public AutoPlay autoPlay;

    public Text scoreText;
    public Text playText;

    int _PlayCount = 1;
    int _Score = 0;

    readonly float startZ = 7.5f;
    readonly float delayZ = 3.0f;

    readonly string[] buttonLabel = new string[] { "かえる", "のこす" };

    Vector3[] HandPos;

    public class HandComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            cHand cx = x as cHand;
            cHand cy = y as cHand;
            if(cx.obj.Number == cy.obj.Number)
            {
                return cx.obj.Mark - cy.obj.Mark;
            }
            return cx.obj.Number - cy.obj.Number;
        }
    }
    HandComparer comparer = new HandComparer();
    class cHand
    {
        public CardObject obj;
        public bool Keep;
    }
    cHand[] Hands;
    Button[] Buttons;

    // Start is called before the first frame update
    void Awake()
    {
        Hands = new cHand[HandRoot.transform.childCount];
        Buttons = new Button[Hands.Length];
        HandPos = new Vector3[Hands.Length];
        for (int i=0; i< Hands.Length; i++)
        {
            Hands[i] = new cHand();
            Hands[i].obj = HandRoot.transform.GetChild(i).GetComponent<CardObject>();
            Hands[i].Keep = false;

            Hands[i].obj.startZ = startZ + delayZ * (float)i;

            Buttons[i] = ButtonRoot.transform.GetChild(i).GetComponent<Button>();
            Buttons[i].interactable = false;
            Text t = Buttons[i].GetComponentInChildren<Text>();
            t.text = "";

            HandPos[i] = Hands[i].obj.transform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(state);
        switch(state)
        {
            case eState.Start:
                updateStart();
                break;
            case eState.Open:
                updateOpen();
                break;
            case eState.Sort:
                updateSort();
                break;
            case eState.Wait:
                updateWait();
                break;
            case eState.Draw:
                updateDraw();
                break;
            case eState.Open2:
                updateOpen2();
                break;
            case eState.Sort2:
                updateSort2();
                break;
            case eState.Result:
                updateResult();
                break;
        }
    }

    void updateStart()
    {
        if (waitForCardMove() == false)
        {
            return;
        }
        for (int i = 0; i < Hands.Length; i++)
        {
            Hands[i].obj.state = CardObject.eState.Open;
        }
        state = eState.Open;
    }

    void updateOpen()
    {
        if (waitForCardMove() == false)
        {
            return;
        }

        System.Array.Sort(Hands, comparer);

        for (int i = 0; i < Hands.Length; i++)
        {
            Hands[i].obj.setSortPos(HandPos[i]);
            Hands[i].obj.state = CardObject.eState.Sort;
        }

        state = eState.Sort;
    }

    void updateSort()
    {
        if (waitForCardMove() == false)
        {
            return;
        }

        for (int i = 0; i < Hands.Length; i++)
        {
            Buttons[i].interactable = true;
            Text t = Buttons[i].GetComponentInChildren<Text>();
            t.text = buttonLabel[Hands[i].Keep ? 1 : 0];
        }
        ButtonDeal.interactable = true;

        state = eState.Wait;
    }

    void updateWait()
    {
        if(autoPlay.enabled==true)
        {
            judgeHands();
            autoPlay.evaluate();
            draw();
        }
    }

    void updateDraw()
    {
        if (waitForCardMove() == false)
        {
            return;
        }
        for (int i=0; i< Hands.Length; i++)
        {
            if(Hands[i].Keep==false)
            {
                Hands[i].obj.state = CardObject.eState.Open;
            }
        }
        state = eState.Open2;
    }

    void updateOpen2()
    {
        if(waitForCardMove() == false)
        {
            return;
        }

        System.Array.Sort(Hands, comparer);

        for (int i = 0; i < Hands.Length; i++)
        {
            Hands[i].obj.setSortPos(HandPos[i]);
            Hands[i].obj.state = CardObject.eState.Sort;
        }

        state = eState.Sort2;
    }

    void updateSort2()
    {
        if (waitForCardMove() == false)
        {
            return;
        }

        judgeHands();

        resultObj.state = resultObject.eState.Appear;
        state = eState.Result;
    }

    void updateResult()
    {
        if(resultObj.state==resultObject.eState.Stable)
        {
            if(autoPlay.enabled==true)
            {
                scoreText.text = _Score.ToString();
                ButtonDeal.interactable = true;
                draw();
            }
            if (ButtonDeal.interactable == false)
            {
                scoreText.text = _Score.ToString();
                ButtonDeal.interactable = true;
            }
        }
    }

    bool waitForCardMove()
    {
        foreach (cHand h in Hands)
        {
            if (h.obj.state != CardObject.eState.Wait)
            {
                return false;
            }
        }
        return true;
    }

    void judgeHands()
    {
        gameRule.eHand hand=gameRule.eHand.HighCard;
        
        bool straight = true;
        bool flush = true;
        for(int i=1; i<Hands.Length; i++)
        {
            if(Hands[i-1].obj.Number+1 != Hands[i].obj.Number)
            {
                straight = false;
                break;
            }
        }
        for (int i = 1; i < Hands.Length; i++)
        {
            if (Hands[i - 1].obj.Mark != Hands[i].obj.Mark)
            {
                flush = false;
                break;
            }
        }
        if(straight && flush)
        {
            if(Hands[0].obj.Number == 10)
            {
                hand = gameRule.eHand.RoyalStraightFlush;
            }
            else
            {
                hand = gameRule.eHand.StraightFlush;
            }
        }
        else if(flush)
        {
            hand = gameRule.eHand.Flush;
        }
        else if(straight)
        {
            hand = gameRule.eHand.Straight;
        }

        int[] n = new int[15];
        for(int i=0; i<Hands.Length; i++)
        {
            n[Hands[i].obj.Number]++;
        }

        bool four=false;
        bool three = false;
        int pair = 0;
        for(int i=2; i<n.Length; i++)
        {
            if(n[i]==4)
            {
                four = true;
                break;
            }
            else if(n[i]==3)
            {
                three = true;
            }
            else if(n[i]==2)
            {
                pair++;
            }
        }

        if(four)
        {
            hand = gameRule.eHand.FourCards;
        }
        else if(three)
        {
            if(pair>0)
            {
                hand = gameRule.eHand.FullHouse;
            }
            else
            {
                hand = gameRule.eHand.ThreeCards;
            }
        }
        else if(pair==2)
        {
            hand = gameRule.eHand.TwoPair;
        }
        else if(pair==1)
        {
            hand = gameRule.eHand.OnePair;
        }

        Text t = resultObj.GetComponentInChildren<Text>();
        if(t)
        {
            t.text = gameRule.HandInfo[(int)hand].name;
        }

        if(hand==gameRule.eHand.HighCard)
        {
            _Score += Hands[4].obj.Number;
        }
        else
        {
            _Score += gameRule.HandInfo[(int)hand].score;
        }

        if(state == eState.Wait)
        {
            autoPlay.currentHand = hand;
        }
        else
        {
            autoPlay.pastHand = hand;
            autoPlay.pastScore = _Score;
        }
    }

    //手札を配る
    void draw()
    {
        if(state==eState.Result)
        {
            for (int i = 0; i < Hands.Length; i++)
            {
                Hands[i].Keep = false;
                Hands[i].obj.state = CardObject.eState.Draw;
                Hands[i].obj.delay = true;

                Hands[i].obj.startZ = startZ + delayZ * (float)i;

                Buttons[i].interactable = false;
                Text t = Buttons[i].GetComponentInChildren<Text>();
                t.text = "";
            }
            resultObj.state = resultObject.eState.Disappear;
            ButtonDeal.interactable = false;

            _PlayCount++;
            playText.text = _PlayCount.ToString();

            state = eState.Start;
            return;
        }
        for (int i = 0; i < Hands.Length; i++)
        {
            Buttons[i].interactable = false;
            if (Hands[i].Keep == false)
            {
                Hands[i].obj.delay = false;
                Hands[i].obj.state = CardObject.eState.Draw;
            }
        }
        ButtonDeal.interactable = false;

        state = eState.Draw;
    }

    //手札の"かえる/のこす"を切り替える
    //[Argument]
    //[in]int id : 手札の番号(0～4)
    public void changeCardState(int id)
    {
        if (id < 0 || id >= Hands.Length) return;

        Hands[id].Keep = !Hands[id].Keep;
        Text t = Buttons[id].GetComponentInChildren<Text>();
        if (t != null)
        {
            t.text = buttonLabel[Hands[id].Keep ? 1 : 0];
        }
    }

    //手札の数値を取得
    public int getHandNumber(int id)
    {
        return Hands[id].obj.Number;
    }

    //手札のマークを取得
    public int getHandMark(int id)
    {
        return Hands[id].obj.Mark;
    }

    //得点の参照
    public int Score
    {
        get { return _Score; }
    }

    //プレイ回数の参照
    public int PlayCount
    {
        get { return _PlayCount; }
    }
}
