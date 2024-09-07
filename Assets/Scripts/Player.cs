using System.Collections;
using UnityEngine;

namespace Exit {
  public class Player : MonoBehaviour {
    [SerializeField] private GameplayCharacterController CharacterController;
    [SerializeField] private float walkSpeed;
    
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;

    [SerializeField] private float minTeleportDistance;
    [SerializeField] private float maxTeleportDistance;
    
    [SerializeField] private float teleportDelaySec = 1;
    
    [Range(0, 1)]
    [SerializeField] private float teleportationProbability;
    
    private float horizontalInput;
    private float horizontalMovement;
    private bool isWalking;
    private float nextJumpForce;
    
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
    }

    void Update()
    {
      horizontalMovement = (currentJumpState == JumpState.Teleport || currentJumpState == JumpState.PendingTeleport)
        ? 0
        : horizontalInput * walkSpeed;
      input.Update();
      // UpdateWalkAnimationState();
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
    
    // private void UpdateWalkAnimationState() {
    //   if (Mathf.Abs(horizontalMovement) > 0 && isWalking == false) {
    //     isWalking = true;
    //     animator.SetBool(Constants.Animation.Walking, true);
    //   } else if (Math.Abs(horizontalMovement) < 0.01f && isWalking) {
    //     isWalking = false;
    //     animator.SetBool(Constants.Animation.Walking, false);
    //   }
    // }

    void FixedUpdate() {
      CharacterController.Move(horizontalMovement * Time.fixedDeltaTime, false, currentJumpState == JumpState.Normal, nextJumpForce);
      if (currentJumpState == JumpState.Teleport)
      {
        StartCoroutine(StartTeleport());
      }
    }
    
    private IEnumerator StartTeleport()
    {
      CharacterController.IsGrounded = false;
      yield return new WaitForSeconds(teleportDelaySec);

      var teleportDistance = Random.Range(minTeleportDistance, maxTeleportDistance);
      transform.position += new Vector3(horizontalInput * teleportDistance, teleportDistance, 0);
      currentJumpState = JumpState.Normal;
    }
  }
}