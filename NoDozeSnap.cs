using SimpleTweaksPlugin.Utility;

namespace SecretTweaks {
    public class DozeSnap : SecretTweaks.SubTweak {
        public override string Name => "Disable Doze Snap";
        private delegate bool ShouldSnap();
        private HookWrapper<ShouldSnap> shouldSnapHook;

        public override void Enable() {
            shouldSnapHook ??= Common.Hook<ShouldSnap>("E8 ?? ?? ?? ?? 84 C0 74 44 4C 8D 6D C7", ShouldSnapDetour);
            shouldSnapHook?.Enable();
            base.Enable();
        }

        private static bool ShouldSnapDetour() => false;

        public override void Disable() {
            shouldSnapHook?.Disable();
            base.Disable();
        }

        public override void Dispose() {
            shouldSnapHook?.Dispose();
            base.Dispose();
        }
    }
}
