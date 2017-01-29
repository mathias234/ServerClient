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
using UnityEngine.SceneManagement;
using System.Collections;

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
    public int CurrentMapId;

    public bool Send;

    public GameObject CharacterTemplate;
    public GameObject Character;


    public enum LoginStatus {
        Idle,
        Connecting,
        Authenticating,
        Connected,
        FailedToConnect,
        Registering,
        UsernameAlreadyInUse,
        RegisteringSuccessful
    }

    public LoginStatus CurrentLoginStatus = LoginStatus.Idle;
    public string CurrentLoginText = "";

    public void ConnectToServer() {
        _clientSocket.Connect(IPAddress.Parse(IpAdress), 3003);
        _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, null);
    }

    public void AttemptLogin() {
        CurrentLoginStatus = LoginStatus.Connecting;
        CurrentLoginText = "Connecting";

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
            CurrentLoginText = "Failed to Connect";
        }
    }

    public void RegisterAccount() {
        if (!_clientSocket.Connected)
            ConnectToServer();

        CurrentLoginStatus = LoginStatus.Registering;
        CurrentLoginText = "Registering";

        var buffer = new AccountRegister(SocketId, Username.GetComponent<InputField>().text, Encrypt(Username.GetComponent<InputField>().text + ":" + Password.GetComponent<InputField>().text)).ToByteArray();

        SendData(buffer);
    }

    public void Update() {
        SendMovement();


        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            SendData(new ChangeMap(SocketId, CurrentMapId, 0, 10.11f, 21, 0).ToByteArray());
        } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SendData(new ChangeMap(SocketId, CurrentMapId, 1, 53, 65, 35).ToByteArray());
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SendData(new ChangeMap(SocketId, CurrentMapId, 2, 145, 107, 237).ToByteArray());
        }
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

            var buffer = new Movement(SocketId, new NetworkVector3(
                position.x, position.y, position.z), yRotation, forward, turn, crouch, onGround, jump, jumpLeg).ToByteArray();

            SendData(buffer);
        } catch (Exception) {
            //Debug.Log("Failed to send data " + ex.Message);
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

                if (packet == null)
                    return;

                switch (packet.Header) {
                    case PacketHeader.Movement:
                        var movement = (Movement)packet;

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
                    case PacketHeader.CharacterDisconnect:
                        var peopleOnline = FindObjectsOfType<NetworkCharacter>();
                        foreach (var online in peopleOnline) {
                            if (online.socketId == ((CharacterDisconnect)packet).SocketId) {
                                Destroy(online.gameObject);
                            }
                        }
                        break;
                    case PacketHeader.Connected:
                        SocketId = ((Connected)packet).SocketId;

                        if (CurrentLoginStatus == LoginStatus.Connecting)
                            Authenticate();
                        break;
                    case PacketHeader.RegisterRespons:
                        var regRespons = (RegisterRespons)packet;

                        switch (regRespons.Respons) {
                            case RegisterRespons.RegisterResponses.Success:
                                CurrentLoginStatus = LoginStatus.RegisteringSuccessful;
                                CurrentLoginText = "Registering Successful";
                                break;
                            case RegisterRespons.RegisterResponses.UsernameAlreadyInUse:
                                CurrentLoginStatus = LoginStatus.UsernameAlreadyInUse;
                                CurrentLoginText = "Username Already In Use";
                                break;
                            case RegisterRespons.RegisterResponses.Failed:
                                Debug.LogError("Failed to register unknown reason");
                                break;
                        }

                        break;
                    case PacketHeader.AuthenticationRespons:
                        var respons = (AuthenticationRespons)packet;

                        switch (respons.Respons) {
                            case AuthenticationRespons.AuthenticationResponses.Success:
                                CurrentLoginStatus = LoginStatus.Connected;
                                CurrentLoginText = "Logged In";
                                RequestCharacters();
                                break;
                            case AuthenticationRespons.AuthenticationResponses.AlreadyLoggedIn:
                                CurrentLoginText = "Already Logged In";
                                break;
                            case AuthenticationRespons.AuthenticationResponses.WrongUsernameAndPassword:
                                CurrentLoginStatus = LoginStatus.FailedToConnect;
                                CurrentLoginText = "Wrong Username or Password";
                                break;
                            case AuthenticationRespons.AuthenticationResponses.Failed:
                                CurrentLoginStatus = LoginStatus.FailedToConnect;
                                CurrentLoginText = "Failed to login unknown reason";
                                break;
                        }

                        Debug.Log("Recieved authentication respons: " + respons.Respons.ToString());
                        break;
                    case PacketHeader.RequestCharacters:
                        var characters = (RequestCharacters)packet;

                        var characterSelection = FindObjectOfType<CharacterSelection>();
                        foreach (var requestCharacter in characters.Characters) {
                            Debug.Log("name: " + requestCharacter.Name + " level: " + requestCharacter.Level);
                        }

                        characterSelection.Populate(characters.Characters);
                        break;
                    case PacketHeader.CreateCharacterRespons:
                        var characterCreationRespons = (CreateCharacterRespons)packet;

                        CharacterCreation cr = FindObjectOfType<CharacterCreation>();
                        cr.HandleCreationRespons(characterCreationRespons);
                        break;
                    case PacketHeader.FullCharacterUpdate:
                        // the server forces all this information to be updated
                        var fullCharacterUpdate = (FullCharacterUpdate)packet;
                        // move to the correct zone
                        var asyncOperation = SceneManager.LoadSceneAsync("map" + fullCharacterUpdate.NewCharacter.MapId);
                        CurrentMapId = fullCharacterUpdate.NewCharacter.MapId;
                        StartCoroutine(LoadMap(asyncOperation, fullCharacterUpdate));
                        break;
                    case PacketHeader.CharactersInMap:
                        var characersInMap = (CharactersInMap)packet;
                        foreach (var character in characersInMap.Characters) {
                            if (character.SocketId != SocketId) {
                                Debug.Log("name: " + character.Name + " level: " + character.Level + " newMap: " + character.MapId);

                                var proxy = Instantiate(ProxyCharacter);
                                proxy.transform.position = new Vector3(character.X, character.Y, character.Z);
                                proxy.GetComponent<NetworkCharacter>().socketId = character.SocketId;
                            }
                        }

                        break;
                    case PacketHeader.NotifyOtherPlayerMapChange:
                        var notifyOtherPlayerMapChange = (NotifyOtherPlayerMapChange)packet;

                        //Debug.Log("name: " + notifyOtherPlayerMapChange.Character.Name + " level: " + notifyOtherPlayerMapChange.Character.Level + " newMap: " + notifyOtherPlayerMapChange.Character.MapId);

                        if (notifyOtherPlayerMapChange.SocketId == SocketId) {
                            return; // this shouldnt happen. Investigate
                        }

                        foreach (var netCharacter in GameObject.FindObjectsOfType<NetworkCharacter>()) {
                            if (netCharacter.socketId == notifyOtherPlayerMapChange.Character.SocketId) {
                                Debug.Log("found a matching netCharacter");
                                if (CurrentMapId == notifyOtherPlayerMapChange.OldMapId) {
                                    // this character has either DCed or changed map
                                    Debug.Log("character left my map");
                                    Destroy(netCharacter.gameObject);
                                } else {
                                    return;
                                }
                            }
                        }

                        if (notifyOtherPlayerMapChange.Character.MapId == CurrentMapId) {
                            if (notifyOtherPlayerMapChange.Character.SocketId != SocketId) {
                                var proxy = Instantiate(ProxyCharacter);
                                proxy.transform.position = new Vector3(notifyOtherPlayerMapChange.Character.X, notifyOtherPlayerMapChange.Character.Y, notifyOtherPlayerMapChange.Character.Z);
                                proxy.GetComponent<NetworkCharacter>().socketId = notifyOtherPlayerMapChange.Character.SocketId;
                            }
                        }
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

    IEnumerator LoadMap(AsyncOperation operation, FullCharacterUpdate fcu) {
        yield return operation;

        // move the character to the correct location
        if (Character == null) {
            // instantiate the character
            Character = Instantiate(CharacterTemplate);
            Character.GetComponent<NetworkCharacter>().isLocal = true;
        }

        Character.transform.position = new Vector3(fcu.NewCharacter.X, fcu.NewCharacter.Y, fcu.NewCharacter.Z);

        Debug.Log(fcu.NewCharacter.MapId);

        // Inform the server that the client successfully connected to the map
        SendData(new ConnectedToMap(SocketId, fcu.NewCharacter.MapId).ToByteArray());
    }

    public void RequestCharacters() {
        Login.SetActive(false);
        CharadcterSelection.SetActive(true);
        var buffer = new RequestCharacters(SocketId, new List<Character>()).ToByteArray();
        SendData(buffer);
    }

    private void Authenticate() {
        CurrentLoginStatus = LoginStatus.Authenticating;
        CurrentLoginText = "Authenticating";

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
