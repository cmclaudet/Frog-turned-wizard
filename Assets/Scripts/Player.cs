using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Exit {
  public class Player : MonoBehaviour {
    [SerializeField] private GameplayCharacterController CharacterController;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject explosion;
    [SerializeField] private float walkSpeed;
    
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;

    [SerializeField] private float minTeleportDistance;
    [SerializeField] private float maxTeleportDistance;
    
    [Range(0, 1)]
    [SerializeField] private float teleportationProbability;
    [Range(0, 1)]
    [SerializeField] private float teleportDirectionShiftProbability;

    [SerializeField] private GameObject leftBounds;
    [SerializeField] private GameObject rightBounds;
    
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
      
      (Vector3 newPosition, bool shouldRevertTeleportDirection) = GetRandomTeleportDirection();
      
      while (newPosition.x < leftBounds.transform.position.x || newPosition.x > rightBounds.transform.position.x)
      {
        (newPosition, shouldRevertTeleportDirection) = GetRandomTeleportDirection();
      }
      
      Explode(shouldRevertTeleportDirection);
      transform.position = newPosition;
      currentJumpState = JumpState.Normal;
      Explode(shouldRevertTeleportDirection);
    }
    
    private (Vector3, bool) GetRandomTeleportDirection()
    {
      bool shouldRevertTeleportDirection = Random.Range(0f, 1f) < teleportDirectionShiftProbability;
      var teleportDistanceX = Random.Range(minTeleportDistance, maxTeleportDistance);
      var teleportDistanceY = Random.Range(minTeleportDistance, maxTeleportDistance);
      var horizontalDirection = (shouldRevertTeleportDirection ? -1 : 1) * horizontalInput * teleportDistanceX;
      return (transform.position + new Vector3(horizontalDirection, teleportDistanceY, 0), shouldRevertTeleportDirection);
    }

    private void Explode(bool shouldRevertTeleportDirection)
    {
      var explosionObj = Instantiate(explosion);
      explosionObj.transform.position = transform.position;
      if (shouldRevertTeleportDirection)
      {
        explosionObj.GetComponent<Animator>().Play("ExplodeSpecial");
      }
      else
      {
        explosionObj.GetComponent<Animator>().Play("Explode");
      }
    }
  }
}