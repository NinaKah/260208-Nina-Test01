using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class ScrollControlledVideo : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [Header("Videoquelle (StreamingAssets)")]
    [SerializeField]
    private string streamingAssetsFileName = "BA_Test_Scrollen_linear_01_VP8.webm";

    [Header("Debug / Test")] 
    [SerializeField]
    private bool autoPlayLoop = false; // Standard: Scroll-Steuerung, kein Autoplay

    [Header("Aktivierung nach Distanz")]
    [SerializeField]
    private bool useActivationDistance = false;

    [SerializeField]
    private Transform referenceTransform; // z.B. Kamera oder Spieler

    [SerializeField]
    private float activationDistance = 3f; // ab dieser Distanz wird das Video gesteuert

    [SerializeField]
    [Range(0f, 1f)]
    private float currentProgress = 0f;

    private bool isPrepared;

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (referenceTransform == null && Camera.main != null)
        {
            referenceTransform = Camera.main.transform;
        }
    }

    private void Start()
    {
        if (videoPlayer != null)
        {
            // URL aus StreamingAssets setzen (wichtig fuer WebGL)
            if (!string.IsNullOrEmpty(streamingAssetsFileName))
            {
                string path = Path.Combine(Application.streamingAssetsPath, streamingAssetsFileName);
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = path;
            }

            videoPlayer.isLooping = false;
            videoPlayer.playOnAwake = false;
            videoPlayer.Pause();
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        isPrepared = true;

        if (autoPlayLoop)
        {
            // Testmodus: Video einfach in Dauerschleife abspielen
            source.isLooping = true;
            source.Play();
        }
        else
        {
            // Scrollmodus: Start auf Frame 0, pausiert
            currentProgress = Mathf.Clamp01(currentProgress);
            ApplyProgress(currentProgress);
        }
    }

    // Wird von JavaScript via SendMessage("ScrollControlledVideo", "SetScrollProgress", progressString) aufgerufen
    public void SetScrollProgress(string progressString)
    {
        if (!float.TryParse(progressString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var progress))
        {
            return;
        }

        SetScrollProgress(progress);
    }

    // Kann auch direkt aus Unity (ohne JS) aufgerufen werden
    public void SetScrollProgress(float progress)
    {
        // Wenn wir im Auto-Play-Testmodus sind, ignorieren wir Scroll-Eingaben
        if (autoPlayLoop)
        {
            return;
        }

        // Optional nur reagieren, wenn wir nah genug dran sind
        if (useActivationDistance && !IsWithinActivationDistance())
        {
            return;
        }

        currentProgress = Mathf.Clamp01(progress);
        ApplyProgress(currentProgress);
    }

    private void ApplyProgress(float progress)
    {
        if (videoPlayer == null || !isPrepared || videoPlayer.length <= 0.0)
        {
            return;
        }

        // Canvas/Scroll steuert die Zeit direkt, Video bleibt quasi pausiert,
        // wir stoßen aber kurz ein Play/Pause an, damit der Frame sicher aktualisiert.
        var targetTime = progress * videoPlayer.length;
        videoPlayer.time = targetTime;

        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            videoPlayer.Pause();
        }
    }

    private bool IsWithinActivationDistance()
    {
        if (referenceTransform == null)
        {
            return true; // falls nichts zugewiesen ist, immer aktiv
        }

        float distance = Vector3.Distance(referenceTransform.position, transform.position);
        return distance <= activationDistance;
    }
}
