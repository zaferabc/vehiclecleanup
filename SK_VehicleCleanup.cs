using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Core.Commands;
using Rocket.API;
using SDG.Unturned;
using System.Linq;
using Rocket.API.Collections;
using UnityEngine;

namespace SK_VehicleCleanup
{
	public class SK_VehicleCleanup : RocketPlugin<SK_VehicleCleanupConfig>

	{
		private SK_VehicleCleanupConfig config;

        public UnityEngine.Color MessageColor { get; private set; }
        protected override void Load()
		{
            config = Configuration.Instance;

            Rocket.Core.Logging.Logger.Log("SK_VehicleCleanup plugin active!");
            Rocket.Core.Logging.Logger.Log("Would you like more free plugins? Join now: https://discord.gg/y3rYs7ZXFs");
            if (config.Automatic) {
				InvokeRepeating("SendWarning", config.ClearInterval, config.ClearInterval);
			}
		}

		protected override void Unload()
		{
            Rocket.Core.Logging.Logger.Log("SK_VehicleCleanup plugin disabled!");
            Rocket.Core.Logging.Logger.Log("Would you like more free plugins? Join now: https://discord.gg/y3rYs7ZXFs");
            
			CancelInvoke("SendWarning");
			CancelInvoke("ClearVehicles");
		}

		[RocketCommand("clearvehicles", "")]
		[RocketCommandAlias("cv")]
		public void ClearCommand(IRocketPlayer caller, string[] command)
		{
			if (config.SendWarningMessage) {
				SendWarning();
			} else {
				ClearVehicles();
			}
		}
		public void SendWarning()
		{
			if (config.SendWarningMessage) {
                UnturnedChat.Say(Translate("warning", config.WarningTime), UnityEngine.Color.red);
            }
			Invoke("ClearVehicles", config.WarningTime);
		}

		public void ClearVehicles()
		{
			Rocket.Core.Logging.Logger.Log("Clearing vehicles!");

			var cleared = 0;
			var vehicles = VehicleManager.vehicles;
			for (int i = vehicles.Count - 1; i >= 0; i--)
			{
				var vehicle = vehicles[i];
				if (CanClearVehicle(vehicle))
				{
					VehicleManager.instance.channel.send("tellVehicleDestroy", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, vehicle.instanceID);
					cleared++;
				}
			}

			Rocket.Core.Logging.Logger.Log($"Cleared {cleared} vehicles!");
			if (config.SendClearMessage && cleared > 0)
			{
                UnturnedChat.Say(Translate("cleared_vehicles", cleared), UnityEngine.Color.green);
			}
		}

		public bool CanClearVehicle(InteractableVehicle vehicle)
		{
			if (vehicle.passengers.Any(p => p.player != null) || vehicle.asset.engine == EEngine.TRAIN)
			{
				return false;
			}
			if (config.ClearLocked && vehicle.isLocked)
			{
				return false;
			}
			if (config.ClearAll)
			{
				return true;
			}
			if (config.ClearExploded && vehicle.isExploded)
			{
				return true;
			}
			if (config.ClearDrowned)
			{
				if (vehicle.isDrowned && vehicle.transform.FindChild("Buoyancy") == null)
				{
					return true;
				}
			}
			if (config.ClearNoTires)
			{
				var tires = vehicle.transform.FindChild("Tires");
				if (tires != null)
				{
					var totalTires = vehicle.transform.FindChild("Tires").childCount;
					if (totalTires == 0)
					{
						return false;
					}

					var aliveTires = 0;
					for (var i = 0; i < totalTires; i++)
					{
						if (tires.GetChild(i).gameObject.activeSelf)
						{
							aliveTires++;
						}
					}
					if (aliveTires == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override TranslationList DefaultTranslations
		{
			get
			{
				return new TranslationList()
				{
					{"cleared_vehicles", "Cleared {0} vehicles!"},
					{"warning", "Clearing vehicles in {0} seconds!"}
				};
			}
		}
	}
}
