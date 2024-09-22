using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour
{
    [SerializeField] private FrameAnim anim;
    public float moveSpeed = 5;
    public Vector2 bodySize = Vector2.one;

    /// <summary>
    /// Return the position i gonna be.
    /// Play animation.
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
