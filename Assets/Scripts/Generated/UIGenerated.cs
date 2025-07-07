using Core;

public struct UIName
{
	public const int IN_GAME_PANEL = -26392309;

	public const int SPLASH_SCREEN_PANEL = 1849654287;

	public const int LOADING_SCREEN_TRANSITION = 776405849;

}

public class UIRegistration
{
	[UnityEngine.RuntimeInitializeOnLoadMethod]
	static void AssignUI()
	{
		UIHandler.AddView (-26392309, "InGamePanel", typeof(InGamePanel), "Assets/Prefabs/UI/Panel/InGamePanel.prefab", "Assets/Panel/InGamePanel", UILayer.Panel);

		UIHandler.AddView (1849654287, "SplashScreenPanel", typeof(SplashPanel), "Assets/Prefabs/UI/Panel/SplashScreenPanel.prefab", "Assets/Panel/SplashScreenPanel", UILayer.Panel);

		UIHandler.AddView (776405849, "LoadingScreenTransition", typeof(LoadingTransition), "Assets/Prefabs/UI/ScreenTransition/LoadingScreenTransition.prefab", "Assets/ScreenTransition/LoadingScreenTransition", UILayer.ScreenTransition);

	}
}