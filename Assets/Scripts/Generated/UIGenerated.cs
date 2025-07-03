using Core;

public struct UIName
{
	public const int IN_GAME_PANEL = -26392309;

	public const int LOADING_PANEL = -621129860;

	public const int SPLASH_SCREEN_PANEL = 1849654287;

}

public class UIRegistration
{
	[UnityEngine.RuntimeInitializeOnLoadMethod]
	static void AssignUI()
	{
		UIHandler.AddView (-26392309, "InGamePanel", typeof(InGamePanel), "Assets/Prefabs/UI/Panel/InGamePanel.prefab", "Assets/Panel/InGamePanel", UILayer.Panel);

		UIHandler.AddView (-621129860, "LoadingPanel", typeof(LoadingPanel), "Assets/Prefabs/UI/Panel/LoadingPanel.prefab", "Assets/Panel/LoadingPanel", UILayer.Panel);

		UIHandler.AddView (1849654287, "SplashScreenPanel", typeof(SplashPanel), "Assets/Prefabs/UI/Panel/SplashScreenPanel.prefab", "Assets/Panel/SplashScreenPanel", UILayer.Panel);

	}
}