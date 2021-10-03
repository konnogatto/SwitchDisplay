using NAudio.CoreAudioApi;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Playnite.SDK.Events;

namespace SwitchDisplay
{
    public class SwitchDisplay : GenericPlugin
    {

        private static readonly ILogger logger = LogManager.GetLogger();

        private SwitchDisplaySettings settings { get; set; }

        private IPlayniteAPI api { get; set; }

        public override Guid Id { get; } = Guid.Parse("75b4d2cc-8308-4c34-8aeb-4dd9a012586d");

        private PolicyConfigClient _policyConfigClient;

        public MMDeviceEnumerator AudioEnumerator { get; set; }

        public DisplayHandler Handler { get; set; }

        public SwitchDisplay(IPlayniteAPI api) : base(api)
        {
            settings = new SwitchDisplaySettings(this);
            this.api = api;
            Handler = new DisplayHandler();
            AudioEnumerator = new MMDeviceEnumerator();
            _policyConfigClient = new PolicyConfigClient();

            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }


        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            if(api.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {
                //Display
                if (settings.SwitchDisplays && !String.IsNullOrEmpty(settings.FullscreenDisplay))
                {
                    if (!Handler.SwitchPrimaryDisplay(settings.FullscreenDisplay))
                    {
                        logger.Error(String.Format("Error setting primary display: {0}", settings.FullscreenDisplay));
                    }

                }

                //Audio
                if (settings.SwitchAudio && settings.FullScreenAudioDeviceList.Count > 0)
                {
                    //search available device
                    foreach(KeyValuePair<string, string> device in settings.FullScreenAudioDeviceList)
                    {
                        string id = "";
                        if (settings.EnumerateAudioDevices.ContainsKey(device.Key))
                        {
                            id = device.Key;
                        } else if (settings.EnumerateAudioDevices.ContainsValue(device.Value))
                        {
                            id = settings.EnumerateAudioDevices.First(p => p.Value == device.Value).Value;
                        }
                        if(id.Length > 0)
                        {
                            _policyConfigClient.SetDefaultEndpoint(id, Role.Multimedia);
                            break;
                        }

                    }

                }
            }

        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            if (api.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {
                //Display
                if (settings.SwitchDisplays && !String.IsNullOrEmpty(settings.DefaultDisplay))
                {
                    if (!Handler.SwitchPrimaryDisplay(settings.DefaultDisplay))
                    {
                        logger.Error(String.Format("Error setting primary display: {0}", settings.DefaultDisplay));
                    }

                }
                //Audio
                if (settings.SwitchAudio && !String.IsNullOrEmpty(settings.DefaultAudioDevice))
                {
                    _policyConfigClient.SetDefaultEndpoint(settings.DefaultAudioDevice, Role.Multimedia);
                }
            }
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new SwitchDisplaySettingsView(settings);
        }

    }
}
