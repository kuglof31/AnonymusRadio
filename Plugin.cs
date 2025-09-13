using AnonymusRadio;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Loader.Features.Plugins;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using AnonymusRadio.PlayerInfos;
using LabApi.Features.Console;
using InventorySystem.Items.Radio;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using NetworkManagerUtils.Dummies;
using LabApi.Events.Arguments.ServerEvents;

namespace AnonymusRadio
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "AnonymusRadio";

        public override string Description => "Makes people anonymus when talking on the radio";

        public override string Author => "NOVA";

        public override Version Version => new Version(1, 0, 1, 0);

        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

        public static Plugin Instance { get; private set; }

        private EventHandlers eventHandlers;

        public override void Disable()
        {
            Instance = null;
            eventHandlers.UnregisterEvents();

            eventHandlers = null;
        }

        public override void Enable()
        {
            Instance = this;
            eventHandlers = new EventHandlers();

            eventHandlers.RegisterEvents();

        }

        private class EventHandlers
        {

            public void RegisterEvents()
            {
                LabApi.Events.Handlers.PlayerEvents.SendingVoiceMessage += OnPlayerSendingVoiceMessages;
                LabApi.Events.Handlers.PlayerEvents.ChangedNickname += OnChangedNickname;
                LabApi.Events.Handlers.PlayerEvents.Spawned += OnSpawned;
                LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;

            }
            public void UnregisterEvents()
            {
                LabApi.Events.Handlers.PlayerEvents.SendingVoiceMessage -= OnPlayerSendingVoiceMessages;
                LabApi.Events.Handlers.PlayerEvents.ChangedNickname -= OnChangedNickname;
                LabApi.Events.Handlers.PlayerEvents.Spawned -= OnSpawned;
                LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;

            }

            public void OnPlayerSendingVoiceMessages(PlayerSendingVoiceMessageEventArgs ev)
            {
                Timing.RunCoroutine(seeIfTalking(ev.Player));
            }

            public void OnChangedNickname(PlayerChangedNicknameEventArgs ev)
            {
                string? newNick = ev.NewNickname;
                int playerId = ev.Player.PlayerId;

                if (newNick != null && newNick != Instance.Config.displayNameToChangeTo)
                {
                    PlayerRadioInfo.originalName[playerId] = newNick;
                }
                else if (newNick == null) 
                {
                    if (!PlayerRadioInfo.originalName.ContainsKey(playerId))
                    {
                        PlayerRadioInfo.originalName[playerId] = ev.Player.DisplayName;
                    }
                }
            }
            public void OnSpawned(PlayerSpawnedEventArgs ev)
            {
                if (!PlayerRadioInfo.originalName.ContainsKey(ev.Player.PlayerId))
                {
                    PlayerRadioInfo.originalName[ev.Player.PlayerId] = ev.Player.DisplayName;
                }
            }
            public void OnRoundEnded(RoundEndedEventArgs ev)
            {
                PlayerRadioInfo.originalName.Clear();
                PlayerRadioInfo.playersUsingRadio.Clear();
                Timing.KillCoroutines();
            }

            public IEnumerator<float> seeIfTalking(Player player)
            {

                while (true)
                {
                    yield return 0.2f;

                    if (player.IsUsingRadio)
                    {
                        PlayerRadioInfo.GetAndSaveInfos(player);
                    }
                    else
                    {
                        
                        PlayerRadioInfo.SetDeafultInfos(player);
                        Timing.KillCoroutines();
                    }
                }
            }

        }


    }
}
