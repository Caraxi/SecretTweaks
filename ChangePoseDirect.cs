using System;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using SimpleTweaksPlugin;
using SimpleTweaksPlugin.TweakSystem;
using SimpleTweaksPlugin.Utility;

namespace SecretTweaks;

[TweakName("Change Pose Direct")]
[TweakDescription("Allow changing pose to a specific pose number. /cpose [number]")]
public unsafe class ChangePoseDirect : SecretTweaks.SubTweak {
    private delegate byte ProcessChatInputDelegate(UIModule* uiModule, byte** a2, nint a3);

    [TweakHook, Signature("E8 ?? ?? ?? ?? FE 86 ?? ?? ?? ?? C7 86", DetourName = nameof(ProcessChatDetour))]
    private HookWrapper<ProcessChatInputDelegate> processChatHook = null!;

    private void ChangePose(EmoteController.PoseType poseType, byte poseNum, bool changingPose, ref bool abort, bool noErrorMessage = false) {
        var groupMax = EmoteController.GetAvailablePoses(poseType);
        var groupCurrent = UIState.Instance()->PlayerState.SelectedPoses[(byte)poseType];

        if (poseNum > groupMax) {
            if (!noErrorMessage) Service.Chat.PrintError($"{poseType} only has {groupMax + 1} positions.");
            abort = true;
            return;
        }

        if (groupCurrent == poseNum) {
            abort = true;
            return;
        }

        if (changingPose) {
            if (poseNum == 0) {
                UIState.Instance()->PlayerState.SelectedPoses[(byte)poseType] = groupMax;
            } else {
                UIState.Instance()->PlayerState.SelectedPoses[(byte)poseType] = (byte)(poseNum - 1);
            }
        } else {
            UIState.Instance()->PlayerState.SelectedPoses[(byte)poseType] = poseNum;
        }
    }

    private void ParseMessage(byte** message, out bool abort) {
        abort = false;
        try {
            var character = (Character*)Service.ClientState.LocalPlayer?.Address;
            if (character == null) return;

            var inputString = Common.ReadString(*message);
            if (inputString[0] == '/' && inputString.Length > 1) {
                var split = inputString[1..].Split(' ');

                if (split.Length == 2) {
                    if (split[0] is "changepose" or "cpose" or "sit" or "doze" or "gsit" or "groundsit" && split[1].StartsWith("w", StringComparison.InvariantCultureIgnoreCase)) {
                        abort = true;
                        Service.Chat.Print($"Current Pose: {(EmoteController.PoseType)character->EmoteController.GetPoseKind()} {character->EmoteController.CPoseState + 1}");
                        return;
                    }

                    if (!byte.TryParse(split[1], out var poseNum)) return;
                    if (poseNum == 0) return;
                    poseNum--;

                    var currentPoseType = (EmoteController.PoseType)character->EmoteController.GetPoseKind();
                    switch (split[0]) {
                        case "changepose":
                        case "cpose": {
                            if (currentPoseType > EmoteController.PoseType.Umbrella) return;
                            ChangePose(currentPoseType, poseNum, true, ref abort);
                            break;
                        }
                        case "sit": {
                            if (currentPoseType is EmoteController.PoseType.GroundSit or EmoteController.PoseType.Sit) {
                                abort = true;
                                ChatHelper.SendMessage($"/cpose {poseNum + 1}");
                            } else {
                                var a = false;
                                var b = false;
                                ChangePose(EmoteController.PoseType.GroundSit, poseNum, false, ref a, true);
                                ChangePose(EmoteController.PoseType.Sit, poseNum, false, ref b, true);
                            }

                            break;
                        }
                        case "doze":
                            if (currentPoseType is EmoteController.PoseType.Doze) {
                                abort = true;
                                ChatHelper.SendMessage($"/cpose {poseNum + 1}");
                            } else {
                                var a = false;
                                ChangePose(EmoteController.PoseType.Doze, poseNum, false, ref a, true);
                            }

                            break;
                        case "gsit":
                        case "groundsit":
                            break;
                    }
                }
            }
        } catch {
            // 
        }
    }

    private byte ProcessChatDetour(UIModule* uiModule, byte** message, nint a3) {
        ParseMessage(message, out var abort);
        if (abort) return 0;
        return processChatHook.Original(uiModule, message, a3);
    }
}
