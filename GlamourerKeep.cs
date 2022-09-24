using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Ipc;
using SimpleTweaksPlugin;

namespace SecretTweaks;

public class GlamourerKeep : SecretTweaks.SubTweak {
    public override string Name => "Keep Glamourer customization when changing zones";

    private ICallGateSubscriber<string, GameObject?, object>? applyAll;
    private ICallGateSubscriber<GameObject?, string>? getAll;
    
    public override void Enable() {
        
        applyAll = Service.PluginInterface.GetIpcSubscriber<string, GameObject?, object>("Glamourer.ApplyAllToCharacter");
        getAll = Service.PluginInterface.GetIpcSubscriber<GameObject?, string>("Glamourer.GetAllCustomizationFromCharacter");

        if (applyAll == null || getAll == null) {
            SimpleLog.Error("Glamourer Not Available");
            return;
        }
        
        Service.Condition.ConditionChange += ConditionOnConditionChange;
        base.Enable();
    }

    private string tempCustomization = string.Empty;
    
    private void ConditionOnConditionChange(ConditionFlag flag, bool value) {
        if (flag != ConditionFlag.BetweenAreas) return;
        if (applyAll == null || getAll == null) return;
        if (value) { 
            tempCustomization = getAll.InvokeFunc(Service.ClientState.LocalPlayer);
        } else {
            applyAll.InvokeAction(tempCustomization, Service.ClientState.LocalPlayer);
        }
    }

    public override void Disable() {
        Service.Condition.ConditionChange -= ConditionOnConditionChange;
        base.Disable();
    }
}
