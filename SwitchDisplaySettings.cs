using Newtonsoft.Json;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.CoreAudioApi;

namespace SwitchDisplay
{
    public class SwitchDisplaySettings : ISettings
    {
        private readonly SwitchDisplay plugin;

        public string FullscreenDisplay { get; set; } = string.Empty;
        public string DefaultDisplay { get; set; } = string.Empty;

        public string FullscreenAudioDevice { get; set; } = string.Empty;

        public string DefaultAudioDevice { get; set; } = string.Empty;

        public bool SwitchDisplays { get; set; } = true;

        public bool SwitchAudio { get; set; } = true;

        [JsonIgnore]
        private Dictionary<string, string> _audioDevices;

        // Parameterless constructor must exist if you want to use LoadPluginSettings method.
        public SwitchDisplaySettings()
        {
        }

        public SwitchDisplaySettings(SwitchDisplay plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<SwitchDisplaySettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                FullscreenDisplay = savedSettings.FullscreenDisplay;
                DefaultDisplay = savedSettings.DefaultDisplay;
                FullscreenAudioDevice = savedSettings.FullscreenAudioDevice;
                DefaultAudioDevice = savedSettings.DefaultAudioDevice;
            }

        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(this);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }

        [JsonIgnore]
        public Dictionary<string, string> EnumerateDisplays
        {
            get => plugin.Handler.Enumerate().ToDictionary(display => display.DeviceName, display => String.Format("{0} at {1}", display.MonitorString, display.DeviceString));
        }
        [JsonIgnore]
        public Dictionary<string, string> EnumerateAudioDevices
        {
            get
            {
                if(_audioDevices == null)
                {
                    _audioDevices = plugin.AudioEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToDictionary(audio => audio.ID, audio => audio.FriendlyName);
                }
                return _audioDevices;
            }
        }
    }
}