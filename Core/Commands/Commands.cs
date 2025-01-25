﻿using DPSPanel.Core.Configs;
using DPSPanel.Core.Panel;
using Terraria.ModLoader;

namespace DPSPanel.Core.Commands
{
    public class Commands : ModCommand
    {
        public override CommandType Type => CommandType.Chat; // Makes the command available in chat
        public override string Command => "dps"; // The main command is "/dps"
        public override string Usage => "Use: /dps"; // Usage instructions
        public override string Description => "Usage: /dps toggle"; // Is shown when using "/help"

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
                throw new UsageException("Error: Please enter an argument. Valid arguments are: /dps hide, show, clear.");

            var sys = ModContent.GetInstance<PanelSystem>();

            string target = args[0].ToLower(); // "dps" or "panel"
            if (target == "toggle")
            {
                ModContent.GetInstance<Config>().EnableButton = !ModContent.GetInstance<Config>().EnableButton;
            }
            else
                throw new UsageException("Error: Incorrect argument. Valid arguments are: /dps hide, show, clear.");
        }
    }

    public class SpawnCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat; // Makes the command available in chat
        public override string Command => "a"; // main action (/a)
        public override string Description => "use /spawn item"; // Is shown when using "/help"

        int i = 0;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // /s will add an item to the panel
            if (args[0] == "item")
            {
                PanelSystem sys = ModContent.GetInstance<PanelSystem>();
                sys.state.container.panel.CreateDamageBar($"PlayerName {i}");
                i++;
            }
            else if (args[0] == "clear")
            {
                PanelSystem sys = ModContent.GetInstance<PanelSystem>();
                sys.state.container.panel.ClearPanelAndAllItems();
                sys.state.container.panel.SetBossTitle($"Reset{i}");
            }
        }
    }

    public class ReverseCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat; // Makes the command available in chat
        public override string Command => "toggle"; // The main command is "/dps"
        public override string Usage => "Use: /dps"; // Usage instructions
        public override string Description => "Usage: /toggle dps"; // Is shown when using "/help"

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
                throw new UsageException("Error: Please enter an argument. Valid arguments are: /dps hide, show, clear.");
            var sys = ModContent.GetInstance<PanelSystem>();
            string target = args[0].ToLower(); // "dps" or "panel"

            if (target == "dps")
                ModContent.GetInstance<Config>().EnableButton = !ModContent.GetInstance<Config>().EnableButton;

            // sys?.state.container.TogglePanel();
            else
                throw new UsageException("Error: Incorrect argument. Valid arguments are: /dps hide, show, clear.");
        }
    }
}
