using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [SerializeField] private Transform[] _wheels;
    [SerializeField] private float _rotationSpeed = 100f * 3;
    [SerializeField] private bool _useCarSpeed = true;
    [SerializeField] private CarController _carController;

    private void Update()
    {
        foreach (Transform wheel in _wheels)
        {
            float speed;

            if (_useCarSpeed && _carController != null)
            {
                speed = _carController.CurrentSpeed * _rotationSpeed * Time.deltaTime;
            }
            else
            {
                speed = _rotationSpeed * Time.deltaTime;
            }
            wheel.Rotate(speed, 0f, 0f, Space.Self);
        }
    }
}

