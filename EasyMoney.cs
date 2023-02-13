using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using KBEngine;
using UnityEngine;
using HarmonyLib;
using HarmonyLib.Tools;
using Logger = BepInEx.Logging.Logger;
using Fungus;
using GUIPackage;
using BepInEx.Configuration;

namespace EasyMoney
{
    [BepInPlugin("EasyMoney.J","EasyMoney","1.0.1")]
    public class EasyMoney : BaseUnityPlugin
    {


        void Start()
        {
            multiplier = Config.Bind<int>("config", "multiplier", 2, "灵石获得倍率");
            run = Config.Bind<bool>("config", "run", true, "开启/关闭mod");
            Harmony.CreateAndPatchAll(typeof(EasyMoney));
            //Logger.LogInfo("EasyMoney加载成功!");
        }


        [HarmonyPrefix, HarmonyPatch(typeof(UIBiGuanXiuLianPanel), "OkEvent")]
        public static bool UIBiGuanXiuLianPanel_OkEvent_PrePatch()
        {
            _isBiGuan = true;
            return true;
        }


        [HarmonyPostfix, HarmonyPatch(typeof(UIBiGuanXiuLianPanel), "OkEvent")]
        public static void UIBiGuanXiuLianPanel_OkEvent_PostPatch()
        {
            if (!run.Value) return;
            _isBiGuan = false;
            if (PlayerEx.Player.getStaticID() != 0)
            {
                float totalSpeed = PlayerEx.Player.getTimeExpSpeed() + UIBiGuanXiuLianPanel.GetBiguanSpeed(false, UIBiGuanPanel.Inst.BiGuanType, "");   //总修炼速度 = 基础修炼速度 + 闭关修炼速度
                PlayerEx.Player.AddMoney(multiplier.Value * (int)((float)(_year * 12 + _month) * totalSpeed));
            }
        }


        [HarmonyPostfix, HarmonyPatch(typeof(Avatar), "AddTime")]
        public static void Avatar_AddTime_PostPatch(int addMonth, int Addyear)
        {
            if (!run.Value) return;
            _month = addMonth;
            _year = Addyear;
            if(_isBiGuan) 
            {
                return;
            }
            int exp = (int)((float)(_year * 12 + _month) * PlayerEx.Player.getTimeExpSpeed());      //时间 * 基础修炼速度
            PlayerEx.Player.AddMoney(exp * multiplier.Value);
        }


        static ConfigEntry<int> multiplier;
        static ConfigEntry<bool> run;
        public static int _month;
        public static int _year;
        public static bool _isBiGuan;
    }
}
