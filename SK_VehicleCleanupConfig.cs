using Rocket.API;

namespace SK_VehicleCleanup
{
	public class SK_VehicleCleanupConfig : IRocketPluginConfiguration
	{
		public bool Automatic;
		
		public bool SendClearMessage;
		public bool SendWarningMessage;

		public bool ClearAll;
		public bool ClearLocked;
		public bool ClearExploded;
		public bool ClearDrowned;
		public bool ClearNoTires;
		
		public float WarningTime;
		public float ClearInterval;

        public void LoadDefaults()
		{
			Automatic = false;
			
			SendClearMessage = true;
			SendWarningMessage = true;
			
			ClearAll = false;
			ClearLocked = false;
			ClearExploded = true;
			ClearDrowned = true;
			ClearNoTires = false;

			WarningTime = 30f;
			ClearInterval = 500f;

        }
	}
}
