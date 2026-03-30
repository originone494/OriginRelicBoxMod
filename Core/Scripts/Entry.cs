using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace OriginRelicBox.Scripts;

    // 必须要加的属性，用于注册Mod。字符串和初始化函数命名一致。
    [ModInitializer(nameof(Initialize))]
    public partial class Entry : Node
    {

        public const string ModId = "OriginRelicBox";
        // 初始化函数
        public static void Initialize()
        {
            var harmony = new Harmony(ModId);
            var assembly = Assembly.GetExecutingAssembly();
            ScriptManagerBridge.LookupScriptsInAssembly(assembly);
            harmony.PatchAll();
        }
    }




