using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resultObject : MonoBehaviour
{
    public enum eState
    {
        Appear,
        Stable,
        Disappear,
    }
    eState _state = eState.Stable;

    float count = 0.0f;
    float appearTime = 0.5f;
    float disappearTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _state = eState.Stable;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(_state)
        {
            case eState.Appear:
                updateAppear();
                break;
            case eState.Disappear:
                updateDisappear();
                break;
        }
    }

    void updateAppear()
    {
        count += Time.deltaTime;
        float rate = count / appearTime;
        if(rate>=1.0f)
        {
            rate = 1.0f;
            _state = eState.Stable;
        }
        this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, rate);
    }

    void updateDisappear()
    {
        count += Time.deltaTime;
        float rate = count / disappearTime;
        if (rate >= 1.0f)
        {
            rate = 1.0f;
            _state = eState.Stable;
            this.gameObject.SetActive(false);
        }
        this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, 1.0f-rate);
    }

    public eState state
    {
        set
        {
            _state = value;
            switch(_state)
            {
                case eState.Appear:
                    this.gameObject.SetActive(true);
                    this.transform.localScale = Vector3.zero;
                    break;
            }
            count = 0.0f;
        }
        get { return _state; }
    }
}
