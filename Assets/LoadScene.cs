using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private bool isLoading;
    public void Load(string sceneName)
    {
        if (isLoading == false)
        {
            SceneManager.LoadScene(sceneName);
            isLoading = true;
        }
    }
}
