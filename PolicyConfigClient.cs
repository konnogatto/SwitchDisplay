using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using SwitchDisplay.Interfaces;

namespace SwitchDisplay
{
    class PolicyConfigClient
    {

        public void SetDefaultEndpoint(string devID, Role role)
        {
            object policyConfig = null;
            try
            {
                policyConfig = Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(InterfaceIds.POLICY_CONFIG_CID)));
                switch (policyConfig)
                {

                    case IPolicyConfigRS3 config:
                        config.SetDefaultEndpoint(devID, role);
                        break;
                    case IPolicyConfigRS2 config:
                        config.SetDefaultEndpoint(devID, role);
                        break;
                    case IPolicyConfigRS config:
                        config.SetDefaultEndpoint(devID, role);
                        break;
                    case IPolicyConfig10 config:
                        config.SetDefaultEndpoint(devID, role);
                        break;
                    case IPolicyConfig config:
                        config.SetDefaultEndpoint(devID, role);
                        break;
                    case IPolicyConfigVista config:
                        config.SetDefaultEndpoint(devID, role);
                        break;
                    case IPolicyConfigUnknown config:
                        config.SetDefaultEndpoint(devID, role);
                        break;
                }
            }
            finally
            {
                if (policyConfig != null && Marshal.IsComObject(policyConfig))
                    Marshal.FinalReleaseComObject(policyConfig);
            }

        }

    }
}
