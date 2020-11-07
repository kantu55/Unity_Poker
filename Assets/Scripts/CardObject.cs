using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    public enum eState
    {
        Deal,
        Open,
        Sort,
        Draw,
        Wait,
    }
    eState _state;

    private cardManager manager;

    int _Number;
    int _Mark;

    public float baseZ = 10.0f;
    public float startZ = 10.0f;
    float dealSpeedMax = -20.0f;
    float dealSpeedMin = -0.5f;
    float slowDownZ = 2.0f;
    Vector3 startPos = Vector3.zero;

    public bool delay=true;

    float count = 0.0f;

    float OpenTime = 1.0f;
    float OpenHeight = 1.0f;

    float SortTime = 0.5f;

    Vector3 BasePos;
    Vector3 SortPos;

    Material material;

    Vector2[] markUvOffset = new Vector2[]
        {
            new Vector2(-0.4f, -0.92f),
            new Vector2( 0.1f, -0.92f),
            new Vector2(-0.4f, -1.42f),
            new Vector2( 0.1f, -1.42f),
        };
    Vector4[] markUvClamp = new Vector4[]
        {
            new Vector4(0.0f, 0.5f, 0.5f, 1.0f),
            new Vector4(0.5f, 1.0f, 0.5f, 1.0f),
            new Vector4(0.0f, 0.5f, 0.0f, 0.5f),
            new Vector4(0.5f, 1.0f, 0.0f, 0.5f),
        };

    //bool draw = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = this.transform.localPosition;
        startPos.z = startZ;
        this.transform.localPosition = startPos;

        material = this.GetComponent<Renderer>().materials[1];

        GameObject obj = GameObject.Find("GameManager");
        if(obj)
        {
            manager = obj.GetComponent<cardManager>();
        }

        state = eState.Deal;
    }

    // Update is called once per frame
    void Update()
    {
        switch(_state)
        {
            case eState.Deal:
                updateDeal();
                break;
            case eState.Open:
                updateOpen();
                break;
            case eState.Sort:
                updateSort();
                break;
            case eState.Draw:
                updateDraw();
                break;
        }
    }

    void updateDeal()
    {
        Vector3 pos = this.transform.localPosition;
        float rate = Mathf.Clamp(pos.z, 0.0f, slowDownZ) / slowDownZ;
        float speed = Mathf.Lerp(dealSpeedMin, dealSpeedMax, rate);
        pos.z += speed * Time.deltaTime;
        if(pos.z<=0.0f)
        {
            manager.draw(out _Number, out _Mark);
            //_Number = Random.Range(2,15);
            setNumberTexture();

            //_Mark = Random.Range(0, 4);
            setMarkTexture();

            pos.z = 0.0f;
            state = eState.Wait;
        }
        this.transform.localPosition = pos;
    }

    void updateOpen()
    {
        count += Time.deltaTime;
        float rate = count / OpenTime;
        if(rate >= 1.0f)
        {
            rate = 1.0f;
            count = 0.0f;
            state = eState.Wait;
        }
        Vector3 pos = this.transform.localPosition;
        pos.y = Mathf.Sin(rate * Mathf.PI) * OpenHeight;
        this.transform.localPosition = pos;

        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(0.0f, 180.0f, rate));
    }

    void updateSort()
    {
        count += Time.deltaTime;
        float rate = count / SortTime;
        if (rate >= 1.0f)
        {
            rate = 1.0f;
            count = 0.0f;
            state = eState.Wait;
        }
        this.transform.localPosition = Vector3.Lerp(BasePos, SortPos, rate);
    }

    void updateDraw()
    {
        Vector3 pos = this.transform.localPosition;
        pos.z -= dealSpeedMax * Time.deltaTime;
        if(pos.z >= baseZ)
        {
            pos.z = delay==true ? startZ : baseZ;
            this.transform.rotation = Quaternion.identity;
            state = eState.Deal;
        }
        this.transform.localPosition = pos;
    }

    void setNumberTexture()
    {
        int u = _Number % 4;
        int v = 3 - _Number / 4;

        Vector2 uv = new Vector2((float)u, (float)v);
        uv *= 1.0f / 4.0f;
        //Debug.Log("setNumber : " + _Number + " : (" + u + " ," + v + ")");
        //material.SetTextureScale("_NumTex", new Vector2(0.25f, 0.25f));
        material.SetTextureOffset("_NumTex", uv);
    }

    void setMarkTexture()
    {
        string[] str = System.Enum.GetNames(typeof(cardManager.eKind));
        //Debug.Log("setMark : " + str[_Mark]);
        material.SetTextureOffset("_MarkTex", markUvOffset[_Mark]);
        material.SetVector("_ClampUV", markUvClamp[_Mark]);
    }

    public void setSortPos(Vector3 p)
    {
        BasePos = this.transform.localPosition;
        SortPos = p;
    }

    public eState state
    {
        set 
        {
            _state = value;
            count = 0.0f;
        }
        get { return _state; }
    }

    public int Number
    {
        get { return _Number; }
    }

    public int Mark
    {
        get { return _Mark; }
    }
}
