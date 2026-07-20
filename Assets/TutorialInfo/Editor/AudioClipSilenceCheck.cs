using UnityEditor;
using UnityEngine;

namespace TutorialInfo.Editor
{
    public static class AudioClipSilenceCheck
    {
        private const float Threshold = 0.001f;
    
        [MenuItem("Tools/Audio/Check Selected Clip Silence")]
        private static void Check()
        {
            var clip = Selection.activeObject as AudioClip;
            if (!clip)
            {
                Debug.LogWarning("Select an AudioClip in the Project window first.");
                return;
            }

            var samples = new float[clip.samples * clip.channels];
            if (!clip.GetData(samples, 0))
            {
                Debug.LogWarning(
                    $"Could not read '{clip.name}'. Set Load Type to Decompress On Load " +
                    "and Preload Audio Data, then retry.");
                return;
            }

            int first = -1;
            int last  = -1;

            for (int i = 0; i < samples.Length; i++)
            {
                if (Mathf.Abs(samples[i]) <= Threshold)
                    continue;

                if (first < 0)
                    first = i;

                last = i;
            }

            if (first < 0)
            {
                Debug.Log($"'{clip.name}': entirely silent.");
                return;
            }

            // Sample index -> seconds, accounting for interleaved channels.
            float perSample = 1f / (clip.frequency * clip.channels);
            float lead      = first * perSample;
            float trail     = (samples.Length - 1 - last) * perSample;

            Debug.Log(
                $"'{clip.name}' ({clip.length:F3}s)\n" +
                $"Leading silence:  {lead * 1000f:F1} ms\n" +
                $"Trailing silence: {trail * 1000f:F1} ms\n" +
                $"First sample value: {samples[first]:F4} | Last: {samples[last]:F4}");
        }
    }
}