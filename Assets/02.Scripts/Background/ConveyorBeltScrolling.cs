using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltScrolling : MonoBehaviour
{
    private MeshRenderer _renderer;

    public float Speed;
    private float _offset;

    private bool _isScrolling = false;
    private float _currentSpeed = 0f;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if(!_isScrolling)
        {
            return;
        }

        _offset += Time.deltaTime * Speed;
        _renderer.material.mainTextureOffset = new Vector2(0, _offset);
    }

    public void StartScrolling(float speed)
    {
        _currentSpeed = speed;
        _isScrolling = true;
    }

    public void StopScrolling()
    {
        _isScrolling = false;
    }
}
