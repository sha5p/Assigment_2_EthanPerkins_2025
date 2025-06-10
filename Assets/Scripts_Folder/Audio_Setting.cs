using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Audio_Setting : MonoBehaviour
{
    [Header("Mixers")]
    [SerializeField] public AudioMixer Mixer;


    [Header("Sliders")]
    [SerializeField] private Slider Masterslider;
    [SerializeField] private Slider SFX_slider;
    [SerializeField] private Slider Musicslider;


    [SerializeField] public GameObject defaultSelectedObject;
    [SerializeField] public GameObject Settings_defaultSelectedObject;
    [Header("Menu Checks")]

    [SerializeField] private GameObject music_settings;
    [SerializeField] private GameObject Main_Settings;


    [Header("Game_Checks")]

    Audio_Manager audio_manager;

    public void newSave()
    {

        string currentSceneName = SceneManager.GetActiveScene().name;

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(currentSceneName);

    }
    private void Awake()
    {

        

    }
    public void OnClickBack()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log(currentScene + "This is the current scene");
        if (currentScene == "Menu")
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
            audio_manager.PlaySFX(audio_manager.ClickSound);
            music_settings.SetActive(false);
            for (int i = 0; i < music_settings.transform.childCount; i++)
            {
                Transform child = music_settings.transform.GetChild(i);
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
            Main_Settings.SetActive(true);
            for (int i = 0; i < Main_Settings.transform.childCount; i++)
            {
                Transform child = Main_Settings.transform.GetChild(i);
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
        Time.timeScale = 1;


    }
    public void OnMusicClicked()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log(currentScene + "This is the current scene");
        if (currentScene == "Menu")
        {
            EventSystem.current.SetSelectedGameObject(Settings_defaultSelectedObject);
            audio_manager.PlaySFX(audio_manager.ClickSound);
            music_settings.SetActive(true);
            for (int i = 0; i < music_settings.transform.childCount; i++)
            {
                Transform child = music_settings.transform.GetChild(i);
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
            Main_Settings.SetActive(false);
            for (int i = 0; i < Main_Settings.transform.childCount; i++)
            {
                Transform child = Main_Settings.transform.GetChild(i);
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(Settings_defaultSelectedObject);
            audio_manager.PlaySFX(audio_manager.ClickSound);
            music_settings.SetActive(true);
            for (int i = 0; i < music_settings.transform.childCount; i++)
            {
                Transform child = music_settings.transform.GetChild(i);
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
            LoadVolume();
        }
        
    }
    private void Start()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log(currentScene + "This is the current scene");
        if (currentScene == "Menu")
        {
            
            this.gameObject.SetActive(false);
        }
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }

    }

    private void LoadVolume()
    {
        Masterslider.value = PlayerPrefs.GetFloat("MasterVolume");
        SFX_slider.value = PlayerPrefs.GetFloat("SFXVolume");
        Musicslider.value = PlayerPrefs.GetFloat("MusicVolume");
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
    public void SetMasterVolume()
    {
        float volume = Masterslider.value;
        Mixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    public void SetSFXVolume()
    {
        float volume = SFX_slider.value;
        Mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    public void SetMusicVolume()
    {
        float volume = Musicslider.value;
        Mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }


}
