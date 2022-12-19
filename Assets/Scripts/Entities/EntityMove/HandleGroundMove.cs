using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleGroundMove : ISetupable {
    private float _maxJumpHeight;
	private float _minJumpHeight;
	private float _timeToJumpApex;
    private float _moveSpeed;

    private Vector3 _velocity;
	private float _gravity;
	private float _maxJumpVelocity;
	private float _minJumpVelocity;
    private Vector2 _directionalInput;

    private GroundController _controller;
	public Vector3 Velocity { get { return _velocity; } }

    public HandleGroundMove(GroundController controller) {
        _controller = controller;
    }

    public void Initalize() {
        _controller.Initalize();

		_gravity = -(2 * _maxJumpHeight) / Mathf.Pow (_timeToJumpApex, 2);
		_maxJumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;
		_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs (_gravity) * _minJumpHeight);
    }

    public void Progress() {
		CalculateVelocity(_directionalInput);
		MoveObject(_directionalInput);
		PostCalculateVelocity();
    }

    private void MoveObject(Vector2 dir) {
        _controller.Move(_velocity * Time.deltaTime, dir);
    }

    private void CalculateVelocity(Vector2 dir) {
        float targetVelocityX = dir.x * _moveSpeed;
		_velocity.x = targetVelocityX;
		_velocity.y += _gravity * Time.deltaTime;
    }

    private void PostCalculateVelocity() {
        if (_controller.collisions.above || _controller.collisions.below) {
			if (_controller.collisions.slidingDownMaxSlope) {
				_velocity.y += _controller.collisions.slopeNormal.y * -_gravity * Time.deltaTime;
			} else {
				_velocity.y = 0f;
			}
		}
    }

    public void SetDirectionalInput(Vector2 input) {
        _directionalInput = input;
    }

    public void OnJumpInputDown() {
		if (_controller.collisions.below) {
            _velocity.y = _maxJumpVelocity;
		}
    }

    public void OnJumpInputUp() {
		if (_velocity.y > _minJumpVelocity) {
			_velocity.y = _minJumpVelocity;
		}
	}
}