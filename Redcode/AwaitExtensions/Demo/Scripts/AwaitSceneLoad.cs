using UnityEngine;
using UnityEngine.SceneManagement;

namespace Redcode.Awaiting.Demo
{
    public class AwaitSceneLoad : MonoBehaviour
    {
        [SerializeField]
        private string _sceneName;

        private async void Start()
        {
            await SceneManager.LoadSceneAsync(_sceneName);
        }
    }
}