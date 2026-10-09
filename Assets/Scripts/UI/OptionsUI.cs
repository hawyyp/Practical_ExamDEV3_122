using UnityEngine;
using UnityEngine.UI;

namespace Ratchanon
{
    /// <summary>
    /// Controls the Options menu UI and volume sliders.
    /// Fulfills Question 1 (Scenes Management) and Question 9 (Volume Control).
    /// </summary>
    public class OptionsUI : MonoBehaviour
    {
        [Header("Volume Sliders")]
        public Slider masterSlider;
        public Slider bgmSlider;
        public Slider sfxSlider;

        [Header("Value Texts")]
        public Text masterValueText;
        public Text bgmValueText;
        public Text sfxValueText;

        [Header("Navigation")]
        public Button backButton;

        private float lastSfxPreviewTime = 0f;

        private void Start()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM();
            }

            InitializeSliders();

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void InitializeSliders()
        {
            var audioMgr = AudioManager.Instance;
            if (audioMgr == null) return;

            if (masterSlider != null)
            {
                masterSlider.value = audioMgr.masterVolume;
                UpdateMasterText(masterSlider.value);
                masterSlider.onValueChanged.RemoveAllListeners();
                masterSlider.onValueChanged.AddListener(OnMasterChanged);
            }

            if (bgmSlider != null)
            {
                bgmSlider.value = audioMgr.bgmVolume;
                UpdateBgmText(bgmSlider.value);
                bgmSlider.onValueChanged.RemoveAllListeners();
                bgmSlider.onValueChanged.AddListener(OnBgmChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = audioMgr.sfxVolume;
                UpdateSfxText(sfxSlider.value);
                sfxSlider.onValueChanged.RemoveAllListeners();
                sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            }
        }

        private void OnMasterChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(value);
            }
            UpdateMasterText(value);
        }

        private void OnBgmChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }
            UpdateBgmText(value);
        }

        private void OnSfxChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
                // Audio feedback with 150ms throttle to prevent stutter during dragging
                if (Time.unscaledTime - lastSfxPreviewTime > 0.15f)
                {
                    lastSfxPreviewTime = Time.unscaledTime;
                    AudioManager.Instance.PlayItemBonusSFX();
                }
            }
            UpdateSfxText(value);
        }

        private void UpdateMasterText(float val)
        {
            if (masterValueText != null)
                masterValueText.text = $"{Mathf.RoundToInt(val * 100)}%";
        }

        private void UpdateBgmText(float val)
        {
            if (bgmValueText != null)
                bgmValueText.text = $"{Mathf.RoundToInt(val * 100)}%";
        }

        private void UpdateSfxText(float val)
        {
            if (sfxValueText != null)
                sfxValueText.text = $"{Mathf.RoundToInt(val * 100)}%";
        }

        private void OnBackClicked()
        {
            Debug.Log("[OptionsUI] Back button clicked -> Loading MainMenu");
            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("01_MainMenu");
            }
        }
    }
}
