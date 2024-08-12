using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableDestroyWhenDisable : MonoBehaviour
{
    private void OnDisable()
    {
        Addressables.ReleaseInstance(gameObject);
    }
}