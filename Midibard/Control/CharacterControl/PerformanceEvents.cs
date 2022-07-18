using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using MidiBard.Managers.Agents;

namespace MidiBard.Control.CharacterControl;

class PerformanceEvents
{
    private PerformanceEvents()
    {

    }

    public static PerformanceEvents Instance { get; } = new PerformanceEvents();

    private void EnteringPerformance()
    {
        if (Configuration.config.AutoOpenPlayerWhenPerforming)
            if (!SwitchInstrument.SwitchingInstrument)
	            MidiBard.Ui.Open();

        //if (Configuration.config.AutoSetBackgroundFrameLimit)
        //{
	       // MidiBard.AgentConfigSystem.BackgroundFrameLimit = false;
	       // MidiBard.AgentConfigSystem.ApplyGraphicSettings();
        //}

        if (Configuration.config.AutoSetOffAFKSwitchingTime)
        {
	        MidiBard.AgentConfigSystem.SetOptionValue(ConfigOption.AutoAfkSwitchingTime, 0);
        }
    }

    private void ExitingPerformance()
    {
        if (Configuration.config.AutoOpenPlayerWhenPerforming)
            if (!SwitchInstrument.SwitchingInstrument)
	            MidiBard.Ui.Close();

        //if (Configuration.config.AutoSetBackgroundFrameLimit)
        //{
	       // MidiBard.AgentConfigSystem.BackgroundFrameLimit = true;
	       // MidiBard.AgentConfigSystem.ApplyGraphicSettings();
        //}
    }

    private bool inPerformanceMode;

    public bool InPerformanceMode
    {
        set
        {
            if (value && !inPerformanceMode)
            {
                EnteringPerformance();
            }

            if (!value && inPerformanceMode)
            {
                ExitingPerformance();
            }

            inPerformanceMode = value;
        }
    }
}