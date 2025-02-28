using UnityEngine;

public class MusicPersistent : MonoBehaviour
{
    private static MusicPersistent instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate music players
            return;
        }
    }
}
