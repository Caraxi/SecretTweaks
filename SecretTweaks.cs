using SimpleTweaksPlugin.TweakSystem;

namespace SecretTweaks {
    
    
    
    [TweakName("Secret Tweaks")]
    public class SecretTweaks : SubTweakManager<SecretTweaks.SubTweak> {
        public abstract class SubTweak : BaseTweak {
            public override string Key => $"{nameof(SecretTweaks)}@{base.Key}";
        }
    }
}
