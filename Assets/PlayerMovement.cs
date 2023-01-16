using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D _rb;

    [Header("Movement Variables")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _sprintModifier;

    [Header("Jumping Variables")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Wall Variables")]
    [SerializeField] private float _wallSlidingSpeed;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private LayerMask _wallLayer;

    [SerializeField] private Vector2 _wallJumpForce;
    [SerializeField] private float _wallJumpTimer;
    [SerializeField] private float _wallJumpDuration;
    private float _wallJumpCounter;
    private float _wallJumpDirection;

    /**
     * Crisp Movement
     * Animations
     * Attack
     * 
     */


    /**
     * TODO:
     * Dashing
     * Swords/weapons
     * monster
     * sprites
     */

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        Movement();
        FlipPlayer();
        Jump();

        WallSliding();
        WallJumping();
    }

    private float _horizontalInput;
    private void Movement() {
        if(_isWallJumping) {
            return;
        }

        _horizontalInput = Input.GetAxis("Horizontal");

        if(Input.GetKey(KeyCode.LeftShift)) {
            _horizontalInput *= _sprintModifier;
        }

        _rb.velocity = new Vector2(_horizontalInput * _movementSpeed, _rb.velocity.y);
    }

    private void Jump() {
        if(Input.GetButtonDown("Jump") && IsGrounded()) {
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool _isFacingRight = true;
    private void FlipPlayer() {
        if(_isWallJumping) {
            return;
        }

        if(_isFacingRight && _horizontalInput < 0f || !_isFacingRight && _horizontalInput > 0f) {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool _isWallSliding = false;
    private void WallSliding() {
        if(IsWalled() && !IsGrounded() && _horizontalInput != 0) {
            _isWallSliding = true;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -_wallSlidingSpeed, float.MaxValue));
        } else {
            _isWallSliding = false;
        }
    }

    private bool _isWallJumping = false;
    private void WallJumping() {
        if(_isWallSliding) {
            _isWallJumping = false;
            _wallJumpDirection = -transform.localScale.x;
            _wallJumpCounter = _wallJumpTimer;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            _wallJumpCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && _wallJumpCounter > 0f) {
            _isWallJumping = true;
            _rb.velocity = new Vector2(_wallJumpDirection * _wallJumpForce.x, _wallJumpForce.y);
            _wallJumpCounter = 0;

            if(transform.localScale.x != _wallJumpDirection) {
                _isFacingRight = !_isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            Invoke(nameof(StopWallJumping), _wallJumpDuration);
        }
    }

    private void StopWallJumping() {
        _isWallJumping = false;
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }
    private bool IsWalled() {
        return Physics2D.OverlapCircle(_wallCheck.position, 0.2f, _wallLayer);
    }
}