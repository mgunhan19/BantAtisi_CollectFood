using UnityEngine;
using UnityEngine.SceneManagement;
public class AnaMenuManager : MonoBehaviour
{
    private void Awake()
    {
        if(!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("OyunSesi", 1);
            PlayerPrefs.SetInt("EfektSesi", 1);
        }
    }
    public void OyunBasla()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }
}
