﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using BetterDPS.UI.DPS;

namespace BetterDPS.UI.Commands
{
    public class EnableCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat; // Makes the command available in chat
        public override string Command => "enable"; // The main command is "/enable"
        public override string Usage => "Use: /enable dps"; // Usage instructions
        public override string Description => "Enable DPS UI panels.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
            {
                throw new UsageException("[BetterDPS] Use: /enable dps");
            }

            // Access the UISystem to manage the panels
            var uiSystem = ModContent.GetInstance<DPSPanelSystem>();

            // Determine the target
            string target = args[0].ToLower(); // "dps" or "panel"

            if (target == "dps")
            {
                uiSystem.state.ShowDPSPanel();
            }
            else
            {
                throw new UsageException("[BetterDPS] Invalid target. Use: /enable dps");
            }
        }
    }
}
