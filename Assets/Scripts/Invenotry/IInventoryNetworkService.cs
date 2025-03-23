using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public interface IInventoryNetworkService
{
    IEnumerator SendInventoryEvent(InventoryItem item, string action);
}
