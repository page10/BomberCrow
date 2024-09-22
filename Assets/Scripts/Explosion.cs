using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private FrameAnim anim;
    
    public Vector2Int CoverGrid { get; private set; }
    private Action<Explosion> _onOver;

    public void Set(Vector2Int grid, Action<Explosion> onOver)
    {
        CoverGrid = grid;
        _onOver = onOver;
        anim.DoLoop(false);
    }

    public bool DoUpdate(float delta)
    {
        if (!this || !gameObject || !anim) return true;
        //todo 依赖于动画了
        bool done = anim.DoUpdate(delta);
        if (done)
        {
            _onOver?.Invoke(this);
        }
        return done;
    }
}
