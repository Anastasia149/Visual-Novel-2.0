using System;
using UnityEngine;

namespace Commands
{
    public class CMD_DatabaseExtension_Audio : CMD_DatabaseExtension
    {
        private static string[] Param_SFX = new string[] { "-s", "-sfx" };
        private static string[] Param_Volume = new string[] { "-v", "-vol", "-volume" };
        private static string[] Param_Pitch = new string[] { "-p", "-pitch" };
        private static string[] Param_Loop = new string[] { "-l", "-loop" };

        private static string[] Param_Channel = new string[] { "-c", "-channel" };
        private static string[] Param_Immediate = new string[] { "-i", "-immediate" };
        private static string[] Param_StartVolume = new string[] { "-sv", "-startvolume" };
        private static string[] Param_Song = new string[] { "-s", "-song" };
        private static string[] Param_Ambiance = new string[] { "-a", "-ambiance" };
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
            database.AddCommand("stopsfx", new Action<string>(StopSFX));

            //database.AddCommand("playvoice", new Action<string[]>(PlayVoice));
            //database.AddCommand("stopvoice", new Action<string>(StopSFX));

            database.AddCommand("playsong", new Action<string[]>(PlaySong));
            database.AddCommand("playambiance", new Action<string[]>(PlayAmbiance));

            database.AddCommand("stopsong", new Action<string>(StopSong));
            database.AddCommand("stopambiance", new Action<string>(StopAmbiance));
        }

        #region SFX
        private static void PlaySFX(string[] data)
        {
            string filePath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            // Try to get the name or path to the sound effect
            parameters.TryGetValue(Param_SFX, out filePath);

            // Try to get the volume of the sound
            parameters.TryGetValue(Param_Volume, out volume, defaultValue: 1f);

            // Try to get the pitch of the sound
            parameters.TryGetValue(Param_Pitch, out pitch, defaultValue: 1f);

            // Try to get the pitch of the sound
            parameters.TryGetValue(Param_Loop, out loop, defaultValue: false);

            // Run the logic
            string resourcesPath = FilePaths.GetPathToResources(FilePaths.resources_sfx, filePath);
            AudioClip sound = Resources.Load<AudioClip>(resourcesPath);

            if (sound == null)
                return;

            AudioManager.instance.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop, filePath: resourcesPath);
        }
        //----------------------------------------------------------------------------------\\
        private static void StopSFX(string data)
        {
            AudioManager.instance.StopSoundEffect(data);
        }
        #endregion

        // currently unused, but keep anyway just in case
        #region Voice
        /*private static void PlayVoice(string[] data)
        {
            string filePath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            // Try to get the name or path to the sound effect
            parameters.TryGetValue(Param_SFX, out filePath);

            // Try to get the volume of the sound
            parameters.TryGetValue(Param_Volume, out volume, defaultValue: 1f);

            // Try to get the pitch of the sound
            parameters.TryGetValue(Param_Pitch, out pitch, defaultValue: 1f);

            // Try to get the pitch of the sound
            parameters.TryGetValue(Param_Loop, out loop, defaultValue: false);

            // Run the logic
            AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResources(FilePaths.resources_voices, filePath));

            if (sound == null)
            {
                Debug.Log($"Was not able to load voice '{filePath}'");
                return;
            }
            AudioManager.instance.PlayVoice(sound, volume: volume, pitch: pitch, loop: loop);*/ // "PlayVoice" does not exist - put new thing into audiomanager


        #endregion

        #region Music
        private static void PlaySong(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // Try to get the name or path to the track
            parameters.TryGetValue(Param_Song, out filepath);
            filepath = FilePaths.GetPathToResources(FilePaths.resources_music, filepath);

            // Try to get the channel for this track
            parameters.TryGetValue(Param_Channel, out channel, defaultValue: 1);

            PlayTrack(filepath, channel, parameters);
        }

        private static void PlayAmbiance(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // Try to get the name or path to the track
            parameters.TryGetValue(Param_Ambiance, out filepath);
            filepath = FilePaths.GetPathToResources(FilePaths.resources_ambiance, filepath);

            // Try to get the channel for this track
            parameters.TryGetValue(Param_Channel, out channel, defaultValue: 0);

            PlayTrack(filepath, channel, parameters);
        }

        private static void PlayTrack(string filepath, int channel, CommandParameters parameters)
        {
            bool loop;
            bool immediate;
            float volumeCap;
            float startVolume;
            float pitch;

            // Try to get the max volume of the track
            parameters.TryGetValue(Param_Volume, out volumeCap, defaultValue: 1);

            // Try to get the start volume of the track
            parameters.TryGetValue(Param_StartVolume, out startVolume, defaultValue: 0f);

            // Try to get the pitch of the track
            parameters.TryGetValue(Param_Pitch, out pitch, defaultValue: 1f);

            // Try to get if this track loops
            parameters.TryGetValue(Param_Loop, out loop, defaultValue: true);

            // Try to get if this is an immediate transition (Immediately at max volume)
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            // Run the logic
            AudioClip sound = Resources.Load<AudioClip>(filepath);

            if (sound == null)
            {
                Debug.Log($"Was not able to load track '{filepath}'.");
                return;
            }

            AudioManager.instance.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filepath);
        }
        #endregion

        private static void StopSong(string data)
        {
            if (data == string.Empty)
                StopTrack("1");
            else
                StopTrack(data);
        }

        private static void StopAmbiance(string data)
        {
            if (data == string.Empty)
                StopTrack("0");
            else
                StopTrack(data);
        }

        private static void StopTrack(string data)
        {
            if (int.TryParse(data, out int channel))
                AudioManager.instance.StopTrack(channel);
            else
                AudioManager.instance.StopTrack(data);
        }
    }
}

