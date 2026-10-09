using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Persistent Audio Manager providing continuous BGM and SFX playback across all scenes.
    /// Fulfills Question 9 (Volume Control), Question 10 (Background Music),
    /// Question 11 (Item SFX), and Question 12 (Obstacle SFX).
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindAnyObjectByType<AudioManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("AudioManager (Auto)");
                        _instance = go.AddComponent<AudioManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
            private set => _instance = value;
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        public AudioClip bgmClip;
        public AudioClip itemBonusClip;
        public AudioClip itemPenaltyClip;
        public AudioClip obstacleHitClip;
        public AudioClip winClip;
        public AudioClip gameOverClip;

        [Header("Volume Levels (0.0 to 1.0)")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float bgmVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                LoadVolumeSettings();
                InitializeAudioSources();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            PlayBGM();
        }

        private void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioListener.volume = masterVolume;
        }

        private void InitializeAudioSources()
        {
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
                bgmSource.spatialBlend = 0f; // 2D Stereo
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f; // 2D Stereo
            }

            // Auto-load BGM clip from Resources_Exam if missing
#if UNITY_EDITOR
            if (bgmClip == null)
            {
                bgmClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/BGM/JumpyGame.wav");
            }
            if (itemBonusClip == null)
            {
                itemBonusClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Coin Pickup 4.wav");
            }
            if (itemPenaltyClip == null)
            {
                itemPenaltyClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Debuff Downgrade 1.wav");
            }
            if (obstacleHitClip == null)
            {
                obstacleHitClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Bounce_Jump 3.wav");
            }
            if (winClip == null)
            {
                winClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Win sound 1.wav");
            }
            if (gameOverClip == null)
            {
                gameOverClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Game Over 2.wav");
            }
#endif

            UpdateVolumes();
        }

        public void PlayBGM()
        {
            if (bgmSource == null) InitializeAudioSources();

            if (bgmClip != null)
            {
                if (!bgmSource.isPlaying || bgmSource.clip != bgmClip)
                {
                    bgmSource.clip = bgmClip;
                    bgmSource.volume = bgmVolume;
                    bgmSource.Play();
                    Debug.Log($"[AudioManager] Playing BGM: {bgmClip.name} (Volume: {bgmSource.volume})");
                }
            }
        }

        public void StopBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip != null)
            {
                if (sfxSource == null) InitializeAudioSources();
                sfxSource.PlayOneShot(clip, volumeScale * sfxVolume);
            }
        }

        public void PlayItemBonusSFX()
        {
            PlaySFX(itemBonusClip);
        }

        public void PlayItemPenaltySFX()
        {
            PlaySFX(itemPenaltyClip);
        }

        public void PlayObstacleHitSFX()
        {
            PlaySFX(obstacleHitClip);
        }

        public void PlayWinSFX()
        {
            PlaySFX(winClip);
        }

        public void PlayGameOverSFX()
        {
            PlaySFX(gameOverClip);
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            AudioListener.volume = masterVolume;
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.Save();
            UpdateVolumes();
            Debug.Log($"[AudioManager] Master Volume set to {masterVolume:P0}");
        }

        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
            PlayerPrefs.Save();
            UpdateVolumes();
            Debug.Log($"[AudioManager] BGM Volume set to {bgmVolume:P0}");
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.Save();
            UpdateVolumes();
            Debug.Log($"[AudioManager] SFX Volume set to {sfxVolume:P0}");
        }

        private void UpdateVolumes()
        {
            AudioListener.volume = masterVolume;

            if (bgmSource != null)
            {
                bgmSource.volume = bgmVolume;
            }
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }
    }
}
