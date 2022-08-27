using UnityEngine;

public class WaterPos : MonoBehaviour
{
    [SerializeField] Transform _player;

    void Start()
    {
        if (PlayerPrefs.HasKey("savedPlayerChords_X"))
        {
            ChangePlayerPosition();
        }
    }

    void ChangePlayerPosition()
    {
        _player.transform.position = new Vector3(PlayerPrefs.GetFloat("savedPlayerChords_X"), PlayerPrefs.GetFloat("savedPlayerChords_Y"), PlayerPrefs.GetFloat("savedPlayerChords_Z"));
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("savedPlayerChords_X", _player.transform.position.x);
        PlayerPrefs.SetFloat("savedPlayerChords_Y", _player.transform.position.y);
        PlayerPrefs.SetFloat("savedPlayerChords_Z", _player.transform.position.z);
    }

    public void Load()
    {
        ChangePlayerPosition();
    }

    public void SaveReset()
    {
        PlayerPrefs.DeleteKey("savedPlayerChords_X");
        PlayerPrefs.DeleteKey("savedPlayerChords_Y");
        PlayerPrefs.DeleteKey("savedPlayerChords_Z");
    }
}