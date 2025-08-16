using Abstracts.Input;
using Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RbMovement: MonoBehaviour
    {
        [SerializeField] float _jumpForce=10f;
        [SerializeField] float _horizontalSpeed=10f;
        private float _horizontalDirection;

        Rigidbody2D _rb;
       
        public float VelocityY => _rb.linearVelocity.y;

        public float HorizontalDirection { get => _horizontalDirection; set => _horizontalDirection = value; }

        private void Awake()
        {
            _rb= GetComponent<Rigidbody2D>();
        }
        public void Jump()
        {
            _rb.linearVelocity = Vector2.up * _jumpForce;
            // _rb.AddForce(Vector2.up * _jumpForce);
        }
        public void HorizontalMove(float direction)
        {
            HorizontalDirection = Mathf.Sign(direction);

            //_rb.position += Vector2.right * direction * _horizontalSpeed * Time.deltaTime;
            _rb.linearVelocity = new Vector2(direction * _horizontalSpeed, _rb.linearVelocity.y);
        }

    }
}
