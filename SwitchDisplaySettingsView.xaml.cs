using System.Windows.Controls;
using System;
using System.Windows;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SwitchDisplay
{
    public partial class SwitchDisplaySettingsView : UserControl
    {
        private SwitchDisplaySettings settings { get; set; }

        public SwitchDisplaySettingsView(SwitchDisplaySettings settings)
        {
            this.settings = settings;
            InitializeComponent();
        }

        private void AddFullscreenAudioDevice_Click(object sender, RoutedEventArgs e)
        {
            var device =((ComboBox)FullscreenAudioDeviceList).SelectedItem;
            if(device != null)
            {
                settings.AddFullscreenDeviceById(((KeyValuePair<string, string>)device).Key, ((KeyValuePair<string, string>)device).Value);
            }

        }
        private void MoveAudioDeviceUp_Click(object sender, RoutedEventArgs e)
        {
            settings.MoveFullscreenDeviceUp(DevicesOrderList.SelectedIndex);
        }
        private void MoveAudioDeviceDown_Click(object sender, RoutedEventArgs e)
        {
            settings.MoveFullscreenDeviceDown(DevicesOrderList.SelectedIndex);
        }
        private void RemoveAudioDevice_Click(object sender, RoutedEventArgs e)
        {
            settings.RemoveFullscreenDeviceByIndex(DevicesOrderList.SelectedIndex);
        }
    }
}
