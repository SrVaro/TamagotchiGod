using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour
{
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    public static event EventHandler<OnTickEventArgs> OnTick;

    [SerializeField]
    private float secondsPerTick = 60f;

    private int tick;
    private float tickTimer;

    void Awake()
    {
        tick = 0;
    }

    // Update is called once per frame
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= secondsPerTick)
        {
            //Debug.Log("1 min");
            tickTimer -= secondsPerTick;
            tick++;
            if (OnTick != null)
                OnTick(this, new OnTickEventArgs { tick = tick });
        }
    }
}
