using Exit;
using UnityEngine;

public class TriggerWin : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private CameraScript camera;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            winScreen.SetActive(true);
            player.DisableInput();
            camera.Freeze();
        }
    }
}