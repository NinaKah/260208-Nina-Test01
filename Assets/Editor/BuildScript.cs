using UnityEditor;
using UnityEngine;

public class BuildScript
{
    public static void BuildWebGL()
    {
        // Kompression deaktivieren
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        
        Debug.Log("Kompression wurde auf Disabled gesetzt");
        
        // Build-Optionen
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = "Build";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        // Build starten
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("WebGL Build erfolgreich abgeschlossen!");
        }
        else
        {
            Debug.LogError("Build fehlgeschlagen!");
        }
    }
}
