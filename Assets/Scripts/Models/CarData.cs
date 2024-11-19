using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarData
{
    [SerializeField] private float acceleration;
    [SerializeField] private float initialSpeed;
    [SerializeField] private float time;

    public void SetAcceleration(float number) => acceleration = number;
    public void SetInitialSpeed(float number) => initialSpeed = number;
    public void SetTime(float number) => time = number;

    public float Acceleration => acceleration;
    public float InitialSpeed => initialSpeed;
    public float Time => time;
}
