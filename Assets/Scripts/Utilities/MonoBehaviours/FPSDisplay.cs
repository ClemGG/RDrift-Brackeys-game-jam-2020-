using UnityEngine;

public class FPSDisplay : MonoBehaviour {

    private float frameRate;
    [SerializeField] private GUIStyle style;





    public static FPSDisplay instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            print("More than one FPSDisplay in scene !");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }




    private void Update()
    {
        frameRate = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0f, 0f, 0f, 0f), frameRate.ToString(), style);
    }
}
