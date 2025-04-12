using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
	async void Start() {
		await UnityServices.InitializeAsync();
		if (!AuthenticationService.Instance.IsSignedIn) {
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
		RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
	}

	void ApplyRemoteConfig(ConfigResponse configResponse) {
		if (configResponse.requestOrigin == ConfigOrigin.Remote) {
			string jsonData = RemoteConfigService.Instance.appConfig.GetJson("TestLevelData");
			Debug.Log("Полученные данные: " + jsonData);
			// Обработайте JSON, например, с помощью JsonUtility
		}
	}

	struct userAttributes { }
	struct appAttributes { }
}
