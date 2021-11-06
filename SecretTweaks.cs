using SimpleTweaksPlugin.TweakSystem;

namespace SecretTweaks {
    public class SecretTweaks : SubTweakManager<SecretTweaks.SubTweak> {
        public abstract class SubTweak : BaseTweak {
            public override string Key => $"{nameof(SecretTweaks)}@{base.Key}";
        }

        public override bool AlwaysEnabled => true;

        public override string Name => "Secret Blah";
    }
}
