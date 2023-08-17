using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _speed;
    private bool _beingControledLocaly;
    private Vector3 _currenDirection;

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
        _rb.velocity = _currenDirection * _speed;
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

    public void SetCurrentDirection(Vector3 dir)
    {
        _currenDirection = dir;
    }

    public void SetBeingControledLocaly(bool beingControled)
    {
        _beingControledLocaly = beingControled;
    }
}
