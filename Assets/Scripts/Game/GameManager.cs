using System.Collections.Generic;
using Game.Fields;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance {
            get {
                if (null == _instance) {
                    return null;
                }
                
                return _instance;
            }
        }
        
        #region EditorVariables

        [Header("오브젝트")] 
        
        [SerializeField] 
        [Tooltip("수위 경보 타이머")]
        private Image waterLevelAlarmObject;

        [SerializeField] 
        [Tooltip("게임오버 패널")] 
        private GameObject gameOverPanel;

        [SerializeField]
        [Tooltip("수면(물) 오브젝트")]
        private Water water;

        [Header("게임 파라미터")] 
        
        [Tooltip("물이 증가하는 시간. 초 단위로 입력")]
        public float waterRiseTime;

        #endregion

        #region Variables

        private int _playerCount;

        private float _waterLevelTime;
        private float _currentTime;

        private List<Log> _timeLine;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake() {
            Screen.SetResolution(960, 540, false);
            if (_instance == null) {
                _instance = this;
                
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
            
            waterLevelAlarmObject.fillAmount = 0;
            gameOverPanel.SetActive(false);
        }

        void Start() {
            _currentTime = 0;
            _timeLine = new();
            
            _waterLevelTime = 0;
            _playerCount = 1; // PhotonNetwork.CurrentRoom.PlayerCount;
            MapGenerator.Instance.GenerateMap(1);
            NetworkManager.instance.GeneratePlayer(new Vector3 (0, 0, 0));
        }

        private void Update() {
            if (_waterLevelTime < waterRiseTime) {
                _waterLevelTime += Time.deltaTime;
            }
            else {
                _currentTime += waterRiseTime;
                _waterLevelTime = 0;
                water.Rise();
            }

            waterLevelAlarmObject.fillAmount = _waterLevelTime / waterRiseTime;
        }

        #endregion

        public void PlayerDie(string playerName) {
            _timeLine.Add(new Log {
                Name = playerName,
                Time = _currentTime + _waterLevelTime
            });

            _playerCount -= 1;

            if (_playerCount <= 0) {
                GameOver();
            }
        }

        private void GameOver() {
            Time.timeScale = 0;

            gameOverPanel.SetActive(true);
            Text gameInfoText = gameOverPanel.transform.GetChild(1).gameObject.GetComponent<Text>();

            gameInfoText.text = string.Empty;
            foreach (var log in _timeLine) {
                gameInfoText.text = $"{log.Name}님은 {log.Time:F2}초 동안 생존하였습니다\n\n";
            }
        }
    }
}
