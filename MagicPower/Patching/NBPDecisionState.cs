using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using JK = JumpKing.GameManager;

namespace MagicPower.Patching;
public class NBPDecisionState
{
    public NBPDecisionState (Harmony harmony)
    {
        Type type = typeof(JK.NBPDecisionState);
        MethodInfo CreateMenu = AccessTools.Method(type, "CreateMenu");
        harmony.Patch(
            CreateMenu,
            transpiler: new HarmonyMethod(AccessTools.Method(typeof(NBPDecisionState), nameof(transpileCreateMenu)))
        );
    }

    private static IEnumerable<CodeInstruction> transpileCreateMenu(IEnumerable<CodeInstruction> instructions , ILGenerator generator)
    {
        CodeMatcher matcher = new CodeMatcher(instructions , generator);

        matcher.MatchStartForward(
            //`if (SkinManager.IsWearingSkin(Items.CrownNBP))`
            new CodeMatch(OpCodes.Ldc_I4_3),
            new CodeMatch(OpCodes.Call, AccessTools.Method("JumpKing.Player.Skins.SkinManager:IsWearingSkin")),
            new CodeMatch(OpCodes.Brfalse_S)
        )
        .ThrowIfInvalid($"Cant find code in {nameof(NBPDecisionState)}")
        .Advance(2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NBPDecisionState), nameof(orMagicPower)))
        );

        return matcher.Instructions();
    }

    private static bool orMagicPower(bool origin)
    {
        return true;
        // return origin || MagicPower.Prefs.hasMagicPower;
    }
}