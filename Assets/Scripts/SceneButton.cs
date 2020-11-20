using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField]

    private string sceneName;
    public void OnClicked()
    {
        SceneManager.LoadScene(sceneName);
    }
}
