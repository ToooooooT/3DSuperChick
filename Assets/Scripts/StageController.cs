using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    private enum Stage {BEFORE_SELECT_ITEM, SELECT_ITEM, PLACE_ITEM, PLAY, SCOREBOARD};

    private string gameMode;
    private Stage stage;
    private GameObject[] playerObjects;
    private GameObject itemGenerator;
    private GameObject[] cameraObjects;
    private GameObject scoreBoardObject;

    void Start() {
        gameMode = PlayerPrefs.GetString("GameMode", "Party");
        if (gameMode == "Party") {
            stage = Stage.BEFORE_SELECT_ITEM;
        } 
        // other mode initial stage not sure yet

        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        itemGenerator = GameObject.FindGameObjectWithTag("ItemGenerator");
        cameraObjects = GameObject.FindGameObjectsWithTag("Camera");
        scoreBoardObject = GameObject.FindGameObjectWithTag("ScoreBoard");
    }

    void Update() {
        switch (stage) {
        case Stage.BEFORE_SELECT_ITEM:
            playerSelectItem();
            EnableFollowCamera();
            itemGenerator.GetComponent<ItemGenerator>().GenerateItems();
            stage = Stage.SELECT_ITEM;
            break;
        case Stage.SELECT_ITEM:
            if (!IsAllPlayersSelectItem()) {
                break;
            }
            ClearChoosingItems();
            EnableVirtualCamera();
            stage = Stage.PLACE_ITEM;
            break;
        case Stage.PLACE_ITEM:
            if (!IsAllPlayersPlaceItem()) {
                break;
            }
            SetPlayersPlay();
            EnableFollowCamera();
            stage = Stage.PLAY;
            break;
        case Stage.PLAY:
            CheckPlayersState();
            CheckAllLose();
            CheckWin();
            break;
        case Stage.SCOREBOARD:
            moveWinner();
            break;
        }
    }

    private void playerSelectItem() {
        for (int i = 0; i < playerObjects.Length; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            player.state = Player.State.SELECT_ITEM;
            RandomPositionToSelectItem(player);
        }
    }

    private bool IsAllPlayersSelectItem() {
        for (int i = 0; i < playerObjects.Length; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.state != Player.State.STOP) {
                return false;
            }
        }
        return true;
    }

    private bool IsAllPlayersPlaceItem() {
        for (int i = 0; i < playerObjects.Length; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.HaveItem()) {
                return false;
            }
        }
        return true;
    }

    private void SetPlayersPlay() {
        for (int i = 0; i < playerObjects.Length; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            player.state = Player.State.GAME;
        }
    }

    private void ClearChoosingItems() {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ChoosingItem");
        for (int i = 0; i < items.Length; ++i) {
            Destroy(items[i]);
        }
    }

    private void RandomPositionToSelectItem(Player player) {
        Vector3 spawnArea = new(0, 3000, 0);
        Vector3 size = new(20, 0, 20);
        float randomX = Random.Range(spawnArea.x - size.x / 2, spawnArea.x + size.x / 2);
        float randomY = Random.Range(spawnArea.y - size.y / 2, spawnArea.y + size.y / 2);
        float randomZ = Random.Range(spawnArea.z - size.z / 2, spawnArea.z + size.z / 2);
        Vector3 randomPosition = new(randomX, randomY, randomZ);
        player.ModifyPosition(randomPosition);
    }

    private void EnableFollowCamera() {
        for (int i = 0; i < cameraObjects.Length; ++i) {
            EnableCamera virtualCameraEnable = cameraObjects[i].GetComponent<EnableCamera>();
            if (cameraObjects[i].name == "FollowCamera") {
                virtualCameraEnable.Enable();
            } 
            if (cameraObjects[i].name == "VirtualCamera") {
                virtualCameraEnable.Disable();
            } 
        }
    }

    private void EnableVirtualCamera() {
        for (int i = 0; i < cameraObjects.Length; ++i) {
            EnableCamera virtualCameraEnable = cameraObjects[i].GetComponent<EnableCamera>();
            if (cameraObjects[i].name == "FollowCamera") {
                virtualCameraEnable.Disable();
            } 
            if (cameraObjects[i].name == "VirtualCamera") {
                Vector3 position = new(0, 10, 0);
                cameraObjects[i].transform.position = position;
                cameraObjects[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                virtualCameraEnable.Enable();
            } 
        }
    }

    private void CheckPlayersState() {
        for (int i = 0; i < playerObjects.Length; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.transform.position.y < -50) {
                player.state = Player.State.LOSE;
                SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
            }
        }
    }

    private void CheckWin() {
        for (int i = 0; i < playerObjects.Length; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.state == Player.State.WIN) {
                stage = Stage.SCOREBOARD;
            }
        }
    }

    private void CheckAllLose() {
        for (int i = 0; i < playerObjects.Length; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.state != Player.State.LOSE) {
                return;
            }
        }
        stage = Stage.BEFORE_SELECT_ITEM;
    }

    private void moveWinner() {
        WinnerMoving winnerMoving = scoreBoardObject.GetComponent<WinnerMoving>();
        for (int i = 0; i < playerObjects.Length; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.state == Player.State.WIN) {
                Vector3 position = winnerMoving.GetPlayerCubePosition(i);
                position.y += 1;
                player.ModifyPosition(position);
                player.state = Player.State.GAME;
                winnerMoving.winner = i;
            }
        }
        // TODO: load video when really win
        // SceneManager.LoadScene("WIN", LoadSceneMode.Additive);
        if (winnerMoving.IsFinishMoving()) {
            stage = Stage.BEFORE_SELECT_ITEM;
        }
    }
}
