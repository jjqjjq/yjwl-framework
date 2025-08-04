using UnityEngine;
using UnityEditor;

namespace JQEditor.tAssetPostprocessor
{
    
public class AudioSetting
{
    public static void ApplyMusicAudio(AudioImporter audioImporter)
    {
        AudioImporterSampleSettings aSettig = audioImporter.GetOverrideSampleSettings("Android");
        AudioImporterSampleSettings iSetting = audioImporter.GetOverrideSampleSettings("iOS");
        if (audioImporter.forceToMono == false || audioImporter.loadInBackground == false || audioImporter.defaultSampleSettings.preloadAudioData == false || aSettig.compressionFormat != AudioCompressionFormat.ADPCM || iSetting.compressionFormat != AudioCompressionFormat.MP3
            || aSettig.loadType != AudioClipLoadType.CompressedInMemory || iSetting.loadType != AudioClipLoadType.CompressedInMemory)
        {
            audioImporter.forceToMono = true;
            audioImporter.loadInBackground = true;
            audioImporter.defaultSampleSettings = new AudioImporterSampleSettings()
            {
                preloadAudioData = true
            };
            AudioImporterSampleSettings androidSettings = new AudioImporterSampleSettings();
            AudioImporterSampleSettings iosSettings = new AudioImporterSampleSettings();
            androidSettings.compressionFormat = AudioCompressionFormat.ADPCM;
            iosSettings.compressionFormat = AudioCompressionFormat.MP3;
            androidSettings.loadType = AudioClipLoadType.CompressedInMemory;
            androidSettings.quality = 1;
            iosSettings.quality = 1;
            androidSettings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            audioImporter.SetOverrideSampleSettings("Android", androidSettings);
            //audioImporter.SetOverrideSampleSettings("Standalone", androidSettings);
            iosSettings.loadType = AudioClipLoadType.CompressedInMemory;
            audioImporter.SetOverrideSampleSettings("iOS", iosSettings);
//            audioImporter.SaveAndReimport();
        }
    }
}
}