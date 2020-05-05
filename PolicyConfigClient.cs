using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using SwitchDisplay.Interfaces;

namespace SwitchDisplay
{
    [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
    internal class _PolicyConfigClient
    {
    }
    class PolicyConfigClient
    {
        private readonly IPolicyConfig _PolicyConfig;
        private readonly IPolicyConfigVista _PolicyConfigVista;
        private readonly IPolicyConfig10 _PolicyConfig10;

        public PolicyConfigClient()
        {
            _PolicyConfig = new _PolicyConfigClient() as IPolicyConfig;
            if (_PolicyConfig != null)
                return;

            _PolicyConfigVista = new _PolicyConfigClient() as IPolicyConfigVista;
            if (_PolicyConfigVista != null)
                return;

            _PolicyConfig10 = new _PolicyConfigClient() as IPolicyConfig10;
        }

        public void SetDefaultEndpoint(string devID, Role role)
        {
            if (_PolicyConfig != null)
            {
                Marshal.ThrowExceptionForHR(_PolicyConfig.SetDefaultEndpoint(devID, role));
                return;
            }
            if (_PolicyConfigVista != null)
            {
                Marshal.ThrowExceptionForHR(_PolicyConfigVista.SetDefaultEndpoint(devID, role));
                return;
            }
            if (_PolicyConfig10 != null)
            {
                Marshal.ThrowExceptionForHR(_PolicyConfig10.SetDefaultEndpoint(devID, role));
            }
        }
    }
}
