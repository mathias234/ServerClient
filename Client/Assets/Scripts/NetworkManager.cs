using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Assets;
using Battlehub.Dispatcher;
using Shared;
using Shared.Packets;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
    private static byte[] _buffer = new byte[1024];
    // this clients socket
    private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public string IpAdress;

    public GameObject ProxyCharacter;

    public GameObject Login;
    public GameObject CharadcterSelection;

    public GameObject Username;
    public GameObject Password;

    public int SocketId;

    public bool Send;


    public enum LoginStatus {
        Idle,
        Connecting,
        Authenticating,
        Connected,
        FailedToConnect,
        Registering
    }

    public LoginStatus CurrentLoginStatus = LoginStatus.Idle;

    public void ConnectToServer() {
        _clientSocket.Connect(IPAddress.Parse(IpAdress), 3003);
        _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, null);
    }

    public void AttemptLogin() {
        CurrentLoginStatus = LoginStatus.Connecting;

        try {
            if (!_clientSocket.Connected)
                ConnectToServer();
            else {
                Debug.Log("Authenticating");
                Authenticate();
            }


        } catch (SocketException ex) {
            Debug.LogError(ex.Message);
            CurrentLoginStatus = LoginStatus.FailedToConnect;
        }
    }

    public void RegisterAccount() {
        if (!_clientSocket.Connected)
            ConnectToServer();

        CurrentLoginStatus = LoginStatus.Registering;

        var buffer = new AccountRegister(SocketId, Username.GetComponent<InputField>().text, Encrypt(Username.GetComponent<InputField>().text + ":" + Password.GetComponent<InputField>().text)).ToByteArray();

        SendData(buffer);
    }

    private void SendMovement() {
        try {
            var player = GameObject.FindGameObjectWithTag("Player");
            var position = player.transform.position;
            var yRotation = player.transform.rotation.eulerAngles.y;

            var animator = player.GetComponent<Animator>();

            var forward = animator.GetFloat("Forward");
            var turn = animator.GetFloat("Turn");
            var crouch = animator.GetBool("Crouch");
            var onGround = animator.GetBool("OnGround");
            var jump = animator.GetFloat("Jump");
            var jumpLeg = animator.GetFloat("JumpLeg");

            var buffer = new Movement(player.GetComponent<NetworkCharacter>().socketId, new NetworkVector3(
                position.x, position.y, position.z), yRotation, forward, turn, crouch, onGround, jump, jumpLeg).ToByteArray();

            SendData(buffer);
        } catch (Exception ex) {
            Debug.Log("Failed to send data " + ex.Message);
        }
    }

    private void RecievedCallback(IAsyncResult ar) {
        try {
            var received = _clientSocket.EndReceive(ar);

            if (received <= 0) {
                return;
            }

            var dataBuffer = new byte[received];
            Array.Copy(_buffer, dataBuffer, received);


            var packet = PacketCreator.ReadPacket(dataBuffer);

            // handle the packets on the main thread as it will interact with the unity systems
            Dispatcher.Current.BeginInvoke(() => {
                GameObject.FindGameObjectWithTag("Player");

                switch (packet.Header) {
                    case PacketHeader.NewProxyCharacter:
                        var character = Instantiate(ProxyCharacter);
                        character.GetComponent<NetworkCharacter>().socketId = ((NewProxyCharacter)packet.Value).SocketId;
                        character.GetComponent<NetworkCharacter>().isLocal = false;
                        break;

                    case PacketHeader.GetTime:
                        break;
                    case PacketHeader.Ping:
                        break;
                    case PacketHeader.Movement:
                        var movement = (Movement)packet.Value;

                        foreach (var networkCharacter in FindObjectsOfType<NetworkCharacter>()) {
                            if (networkCharacter.socketId == movement.SocketId) {
                                var oldPos = networkCharacter.gameObject.transform.position;
                                var newPos = new Vector3(movement.NewPosition.X, movement.NewPosition.Y, movement.NewPosition.Z);
                                //networkCharacter.gameObject.transform.position = newPos;
                                networkCharacter.gameObject.transform.position = Vector3.Lerp(oldPos, newPos, Time.deltaTime * 5);
                                networkCharacter.gameObject.transform.rotation = Quaternion.Euler(0, movement.YRotation, 0);

                                // ANIMATIONS:
                                var animator = networkCharacter.GetComponent<Animator>();
                                animator.SetFloat("Forward", movement.Forward);
                                animator.SetFloat("Turn", movement.Turn);
                                animator.SetBool("Crouch", movement.Crouch);
                                animator.SetBool("OnGround", movement.OnGround);
                                animator.SetFloat("Jump", movement.Jump);
                                animator.SetFloat("JumpLeg", movement.JumpLeg);
                            }
                        }
                        break;
                    case PacketHeader.GetOtherPlayers:
                        var players = ((List<int>)packet.Value);

                        for (int i = 0; i < players.Count; i++) {
                            var proxy = Instantiate(ProxyCharacter);
                            proxy.GetComponent<NetworkCharacter>().socketId = players[i];
                            proxy.GetComponent<NetworkCharacter>().isLocal = false;
                        }
                        break;
                    case PacketHeader.CharacterDisconnect:
                        var peopleOnline = FindObjectsOfType<NetworkCharacter>();
                        foreach (var online in peopleOnline) {
                            if (online.socketId == ((CharacterDisconnect)packet.Value).SocketId) {
                                Destroy(online.gameObject);
                            }
                        }
                        break;
                    case PacketHeader.Connected:
                        SocketId = ((Connected)packet.Value).SocketId;

                        if (CurrentLoginStatus == LoginStatus.Connecting)
                            Authenticate();
                        break;
                    case PacketHeader.RegisterRespons:
                        break;
                    case PacketHeader.AuthenticationRespons:
                        var respons = (AuthenticationRespons)packet.Value;

                        if (respons.Respons == AuthenticationRespons.AuthenticationResponses.Success) {
                            CurrentLoginStatus = LoginStatus.Connected;
                            RequestCharacters();
                        } else {
                            CurrentLoginStatus = LoginStatus.FailedToConnect;
                        }

                        Debug.Log("Recieved authentication respons: " + respons.Respons.ToString());
                        break;
                    case PacketHeader.RequestCharacters:
                        var characters = (RequestCharacters) packet.Value;

                        var characterSelection = FindObjectOfType<PopulateCharacterSelection>();
                        foreach (var requestCharacter in characters.Characters) {
                            Debug.Log("name: " + requestCharacter.Name + " level: " + requestCharacter.Level);
                        }

                        characterSelection.Populate(characters.Characters);


                        break;
                    default:
                        Debug.LogError("Unhandled packet received");
                        break;
                }
            });

            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, null);
        } catch (SocketException) {
            Debug.LogWarning("Unexpcted Disconnect");
        }
    }

    private void RequestCharacters() {
        Login.SetActive(false);
        CharadcterSelection.SetActive(true);
        var buffer = new RequestCharacters(SocketId, new List<Character>()).ToByteArray();
        SendData(buffer);
    }

    private void Authenticate() {
        CurrentLoginStatus = LoginStatus.Authenticating;

        var encryptedPassword = Encrypt(Username.GetComponent<InputField>().text + ":" + Password.GetComponent<InputField>().text);

        var buffer = new Login(SocketId, Username.GetComponent<InputField>().text, encryptedPassword).ToByteArray();

        SendData(buffer);
    }

    public void SendData(byte[] buffer) {
        _clientSocket.Send(buffer);
        _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, null);
    }

    private string Encrypt(string text) {
        var textInBytes = Encoding.ASCII.GetBytes(text);

        SHA1 sha1 = new SHA1CryptoServiceProvider();

        var result = sha1.ComputeHash(textInBytes);

        var encryptedPassword = Encoding.ASCII.GetString(result);

        return encryptedPassword;
    }
}
