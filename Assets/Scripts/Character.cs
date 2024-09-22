using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//角色，其实是功能最简单的玩家角色
public class Character : MonoBehaviour
{
    [SerializeField] private FrameAnim anim;
    public float moveSpeed = 5;
    public Vector2 bodySize = Vector2.one;
    
    //能否穿墙，有的敌人可以穿墙，玩家吃某个power up后也可以
    public bool crossWall = false;

    public bool Dead { get; private set; } = false;

    private const float DeadCleanInSec = 1.2f;
    private float _elapsed = 0;
    
    private void Update()
    {
        if (Dead)
        {
            if (_elapsed >= DeadCleanInSec) return;
            _elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(_elapsed / DeadCleanInSec);
            anim.SetAlpha(1 - p);
            //todo 这很危险，就是在这里自我消除，但是先这么做，包括动画也最好能做timeline，不要直接写update
            if (_elapsed >= DeadCleanInSec) Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放动画并且返回【如果没阻挡】我想去哪儿
    /// </summary>
    /// <param name="wishDirection"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public Vector3 TryMove(MoveDirection wishDirection, float delta)
    {
        float moveInTick = delta * moveSpeed;
        Vector3 meAt = transform.position;
        Vector3 dest = wishDirection switch
        {
            MoveDirection.Up => meAt + Vector3.up * moveInTick,
            MoveDirection.Down => meAt + Vector3.down * moveInTick,
            MoveDirection.Left => meAt + Vector3.left * moveInTick,
            MoveDirection.Right => meAt + Vector3.right * moveInTick,
            _ => meAt
        };
        string dirId = wishDirection switch
        {
            MoveDirection.Up => "Up",
            MoveDirection.Down => "Down",
            MoveDirection.Left => "Left",
            MoveDirection.Right => "Right",
            _ => anim.Current.id
        };
        anim.Play(dirId, true);
        return dest;
    }

    public void Kill()
    {
        Dead = true;
        anim.Pause();
        //todo 播放死亡动画
    }
}

[Serializable]
public enum MoveDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}