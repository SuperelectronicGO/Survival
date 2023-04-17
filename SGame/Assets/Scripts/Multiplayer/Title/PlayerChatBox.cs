using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
public class PlayerChatBox : NetworkBehaviour
{
    public NetworkVariable<FixedString128Bytes> sentMessage = new NetworkVariable<FixedString128Bytes>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    private TMP_InputField messageSender;
    [SerializeField] private TextMeshProUGUI textBox;
    // Start is called before the first frame update
    public void OnEnable()
    {

        messageSender = GetComponent<TMP_InputField>();
        sentMessage.OnValueChanged += (FixedString128Bytes oldValue, FixedString128Bytes newValue) =>
        {
            textBox.text += newValue.ToString();
        };
        messageSender.onSubmit.AddListener((s) =>
        {
            FixedString128Bytes messageToSend = $"[{GameNetworkManager.Instance.PlayerName}] {messageSender.text}\n";
            sentMessage.Value = messageToSend;
            messageSender.text = "";
            FunnyClientRpc();
        });
    }
    [ClientRpc]
    public void FunnyClientRpc()
    {
        Debug.LogError("Funny!");
    }
}
