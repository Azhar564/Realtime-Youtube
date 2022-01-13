using System.Collections;

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

using VideoLibrary;

namespace MyYoutube
{
    public class GrabVideo : MonoBehaviour
    {
        [Header("Setup")]
        public VideoPlayer player;
        public RenderTexture texture;
        public string URL;

        [Header("UI/UX")]
        public Slider videoTimeline;
        public Slider audioController;

        private AudioSource audioSource;

        private static GrabVideo instance = null;
        public static GrabVideo Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<GrabVideo>();

                return instance;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            ShowVideo();
        }

        //Update is called once per frame
        void Update()
        {
            if (videoTimeline != null && player.isPlaying)
                videoTimeline.value = (float)player.frame / (float)player.frameCount;

            if (audioController != null)
                audioController.value = audioSource.volume;
        }

        #region UI/UX method
        /// <summary>
        /// Updated Timeline For UI
        /// </summary>
        public void TimelineUpdate()
        {
            player.frame = (long)(videoTimeline.value * player.frameCount);
            player.Play();
        }

        public void TimelineUpdateDrag()
        {
            player.Pause();
        }

        /// <summary>
        /// Updated Sound by UI
        /// </summary>
        public void SoundUpdate()
        {
            audioSource.volume = audioController.value;
        }
        #endregion

        public void ShowVideo()
        {
            audioSource = GetComponent<AudioSource>();

            var youTube = YouTube.Default; // starting point for YouTube actions
            var video = youTube.GetVideo(URL); // gets a Video object with info about the video

            player.source = VideoSource.Url;
            player.url = video.Uri;

            player.SetTargetAudioSource(0, audioSource);

            StartCoroutine(PrepareAndPlayVideo());

            GetComponent<VideoPlayer>().targetTexture = texture;
        }

        IEnumerator PrepareAndPlayVideo()
        {
            player.Prepare();

            while (!player.isPrepared)
            {
                Debug.Log("Preparing Video");
                yield return null;
            }

            Debug.Log("Done prepping.");

            player.Play();
            audioSource.Play();
        }
    }

}
