﻿using FFXIVClientStructs.FFXIV.Client.System.Framework;
using SimpleTweaksPlugin;

namespace SecretTweaks {
    public unsafe class DisableAfkKick : SecretTweaks.SubTweak {
        public override string Name => "Disable AFK Kick";

        public override void Enable() {
            Service.Framework.Update += FrameworkOnUpdate;
            base.Enable();
        }

        private void FrameworkOnUpdate(Dalamud.Game.Framework framework) {
            if (Service.Condition.Any()) {
                var atkModule = (byte*) Framework.Instance()->UIModule->GetRaptureAtkModule();
                *(float*)(atkModule + 0x276C8) = 0;
                *(float*)(atkModule + 0x276CC) = 0;
                *(float*)(atkModule + 0x276D0) = 0;
            }
        }

        public override void Disable() {
            Service.Framework.Update -= FrameworkOnUpdate;
            base.Disable();
        }
    }
}
