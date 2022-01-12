public static class SceneUtils
{
    public static UnityEngine.SceneManagement.Scene[] GetAllOpenedScenes()
    {
        int countLoaded = UnityEngine.SceneManagement.SceneManager.sceneCount;
        UnityEngine.SceneManagement.Scene[] loadedScenes = new UnityEngine.SceneManagement.Scene[countLoaded];
        for (int i = 0; i < countLoaded; i++)
            loadedScenes[i] = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
        return loadedScenes;
    }
}