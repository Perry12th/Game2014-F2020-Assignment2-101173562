using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip[] musicTracks;

    public enum Track
    {
        Start,
        Instuctions,
        Gameplay,
        GameOver

    }

    private AudioManager() { }
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }

        private set { }
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager[] audioManagers = FindObjectsOfType<AudioManager>();
        foreach (AudioManager mgr in audioManagers)
        {
            if (mgr != Instance)
            {
                Destroy(mgr.gameObject);
            }
        }
        FadeTrack(Track.Start);
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(transform.root);
    }

    /// <summary>
    /// switch to selected track
    /// </summary>
    /// <param name="trackID"></param>
    public void PlayTrack(Track trackID)
    {
        audioSource.clip = musicTracks[(int)trackID];
        audioSource.Play();
    }

    public void FadeTrack(Track trackID)
    {
        audioSource.volume = 0;
        PlayTrack(trackID);
        StartCoroutine(RaiseVolume(3.0f));
    }

    IEnumerator RaiseVolume(float transitionTime)
    {
        float timer = 0.0f;
        while(timer < transitionTime)
        {
            timer += Time.deltaTime;
            float normTime = timer / transitionTime;
            audioSource.volume = Mathf.SmoothStep(0, 1, normTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case ("Start"):
                FadeTrack(Track.Start);
                break;

            case ("Instructions"):
               // FadeTrack(Track.Instuctions);
                break;

            case ("Gameplay"):
                FadeTrack(Track.Gameplay);
                break;

            case ("GameOver"):
                FadeTrack(Track.GameOver);
                break;
        }

      

    }
}

