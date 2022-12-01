using Modding;
using System;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;

namespace GPZ_Precepts
{
    public class GPZ_PreceptsMod : Mod
    {
        private static GPZ_PreceptsMod? _instance;

        internal static GPZ_PreceptsMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(GPZ_PreceptsMod)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public GPZ_PreceptsMod() : base("GPZ_Precepts")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");


            On.HealthManager.Hit += OnHit;
            On.HealthManager.TakeDamage += OnTakeDamage;
            On.HutongGames.PlayMaker.Actions.StringCompare.OnEnter += OnStringCompareAction;
            On.HutongGames.PlayMaker.Actions.ConvertIntToString.OnEnter += OnConvertIntToStringAction;

            Log("Initialized");
        }

        private void OnHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
        {
            if (self.gameObject.name == "Grey Prince")
            {
                if (GameObject.Find("Dream Msg").LocateMyFSM("Display").ActiveStateName == "Idle")
                {
                    self.IsInvincible = false;
                }
            }
            orig(self, hitInstance);
        }

        private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (self.gameObject.name == "Grey Prince")
            {
                self.IsInvincible = true;
                PlayMakerFSM DreamFSM = GameObject.Find("Dream Msg").LocateMyFSM("Display");
                if (DreamFSM.ActiveStateName == "Idle")
                {
                    DreamFSM.GetFsmIntVariable("Convo Amount").Value = 57;
                    DreamFSM.GetFsmStringVariable("Convo Title").Value = "PRECEPT";
                    PlayMakerFSM.BroadcastEvent("DISPLAY ENEMY DREAM");
                }
            }
            orig(self, hitInstance);
        }
        private void OnStringCompareAction(On.HutongGames.PlayMaker.Actions.StringCompare.orig_OnEnter orig, StringCompare self)
        {
            if (self.Fsm.GameObject.name == "Dream Msg" && self.Fsm.Name == "Display" && self.State.Name == "Check Convo")
            {
                self.Fsm.FsmComponent.GetFsmAction<GetLanguageString>("Set Convo", 3).sheetName.Value = (self.Fsm.FsmComponent.GetFsmStringVariable("Convo Title").Value == "PRECEPT") ? "Zote" : "Enemy Dreams";
            }
            orig(self);
        }
        private void OnConvertIntToStringAction(On.HutongGames.PlayMaker.Actions.ConvertIntToString.orig_OnEnter orig, ConvertIntToString self)
        {
            orig(self);

            if (self.Fsm.GameObject.name == "Dream Msg" && self.Fsm.Name == "Display" && self.State.Name == "Set Convo" && self.Fsm.FsmComponent.GetFsmStringVariable("Convo Title").Value == "PRECEPT"  && self.intVariable.Value == 1)
            {
                self.Fsm.FsmComponent.GetFsmStringVariable("Convo Num Str").Value = "1_R";
            }
        }
    }
}
