using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Exit {
  public class Player : MonoBehaviour {
    [SerializeField] private GameplayCharacterController CharacterController;
    [SerializeField] private Animator animator;
    [SerializeField] private float walkSpeed;
    
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;

    [SerializeField] private float minTeleportDistance;
    [SerializeField] private float maxTeleportDistance;
    
    [Range(0, 1)]
    [SerializeField] private float teleportationProbability;
    [Range(0, 1)]
    [SerializeField] private float teleportDirectionShiftProbability;
    
    private float horizontalInput;
    private float horizontalMovement;
    private bool isWalking;
    private float nextJumpForce;
    private float lastYPosition;
    
    private Input input;
    private JumpState currentJumpState;

    private void Awake()
    {
      input = new Input(this);
      CharacterController.OnLandEvent.AddListener(OnLanded);
    }

    private void OnLanded()
    {
      currentJumpState = JumpState.None;
      animator.SetBool(AnimationConstants.IsJumping, false);
    }

    void Update()
    {
      horizontalMovement = (currentJumpState == JumpState.Teleport || currentJumpState == JumpState.PendingTeleport)
        ? 0
        : horizontalInput * walkSpeed;
      input.Update();
      UpdateWalkAnimationState();
    }

    public void TryJump()
    {
      if (CharacterController.IsGrounded == false)
      {
        return;
      }
      var willTeleport = Random.Range(0f, 1f) < teleportationProbability;
      currentJumpState = willTeleport ? JumpState.PendingTeleport : JumpState.Normal;
      if (currentJumpState == JumpState.Normal)
      {
        nextJumpForce = Random.Range(minJumpForce, maxJumpForce);
      } 
      else
      {
        nextJumpForce = 0;
        if (currentJumpState == JumpState.PendingTeleport)
        {
          input.isPendingTeleport = true;
        }
      }
    }

    public void TryTeleport()
    {
      currentJumpState = JumpState.Teleport;
    }
    
    public void SetHorizontalInput(float input) {
      horizontalInput = input;
    }
    
    private void UpdateWalkAnimationState() {
      if (CharacterController.IsGrounded == false)
      {
        animator.SetBool(AnimationConstants.IsJumping, true);
        animator.SetBool(AnimationConstants.IsJumpingDown, lastYPosition > transform.position.y);
      }
      else
      {
        animator.SetBool(AnimationConstants.IsJumping, false);
        animator.SetBool(AnimationConstants.IsJumpingDown, false);
        
        if (Mathf.Abs(horizontalMovement) > 0 && isWalking == false)
        {
          isWalking = true;
          animator.SetBool(AnimationConstants.IsWalking, true);
        }
        else if (Math.Abs(horizontalMovement) < 0.01f && isWalking)
        {
          isWalking = false;
          animator.SetBool(AnimationConstants.IsWalking, false);
        }
      }
    }

    void FixedUpdate() {
      lastYPosition = transform.position.y;
      CharacterController.Move(horizontalMovement * Time.fixedDeltaTime, false, currentJumpState == JumpState.Normal, nextJumpForce);
      if (currentJumpState == JumpState.Teleport)
      {
        Teleport();
      }
    }
    
    private void Teleport()
    {
      CharacterController.IsGrounded = false;
      var teleportDistanceX = Random.Range(minTeleportDistance, maxTeleportDistance);
      var teleportDistanceY = Random.Range(minTeleportDistance, maxTeleportDistance);
      var shouldRevertTeleportDirection = Random.Range(0f, 1f) < teleportDirectionShiftProbability;
      var horizontalDirection = (shouldRevertTeleportDirection ? -1 : 1) * horizontalInput * teleportDistanceX;
      transform.position += new Vector3(horizontalDirection, teleportDistanceY, 0);
      currentJumpState = JumpState.Normal;
    }
  }
}