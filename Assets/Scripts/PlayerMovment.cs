using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _speed;
    private bool _beingControledLocaly;
    private float[] _currenDirection =  new float[3];
    private Vector3 _dir;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateInputs();

        MltJogador.ClientScript.MovmentDataCollection(_currenDirection);
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
    }

    private void UpdateInputs()
    {
        if (_beingControledLocaly)
        {
            float movX = Input.GetAxis("Horizontal");
            float movZ = Input.GetAxis("Vertical");
            float up = Input.GetKeyDown(KeyCode.Q) ? 1 : 0;
            float down = Input.GetKeyDown(KeyCode.E) ? -1 : 0;
            float movY = up + down;

            SetCurrentDirection(new Vector3(movX, movY, movZ).normalized);
        }
    }

    public void SetCurrentDirection(float[] dir)
    {
        _currenDirection = dir;
        _dir = new Vector3(dir[0], dir[1], dir[2]);
    }

    private void SetCurrentDirection(Vector3 dir)
    {
        for(int i = 0; i < _currenDirection.Length; i++)
        {
            _currenDirection[i] = dir[i];
        }
        _dir = dir;
    }

    public void SetBeingControledLocaly(bool beingControled)
    {
        _beingControledLocaly = beingControled;
    }
}
