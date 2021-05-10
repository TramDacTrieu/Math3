using System;
using UnityEngine;

public interface IHittable
{
    void Hit(Vector2 pos, Action callback);

}