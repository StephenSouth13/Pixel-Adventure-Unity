using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public float horizontalAxis;
    public NetworkBool jumpPressed;
    public NetworkBool downPressed;
    public NetworkBool interactPressed;
}