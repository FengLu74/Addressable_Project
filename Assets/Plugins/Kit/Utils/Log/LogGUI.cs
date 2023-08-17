using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Kit
{
    public interface ILogCommander
    {
        string ExecuteString(string text);
        string HistoryUp();
        string HistoryDown();
        List<string> GetActionOfStartWith(string text);
    }

    public class LogGUI : MonoBehaviour, ILog
    {
#pragma warning disable 649
        [SerializeField] private Transform panel;
        [SerializeField] private InputField inputField;
        [SerializeField] private Text textArea;
        [SerializeField] private KeyCode toggleConsoleKey;
        [SerializeField] private Text buildIdText;
        [SerializeField] private int maxLogLines = 100;
#pragma warning restore 649
        private readonly List<string> _lines = new List<string>();
        private int _wantedCaretPosition = -1;
        private readonly List<Action> _actionList = new List<Action>();
        private readonly object _actionLock = new object();
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private bool _isOpen;
        public ILogCommander gmCommander;

        private void Awake()
        {
            _isOpen = false;
            panel.gameObject.SetActive(false);
            inputField.onEndEdit.AddListener(OnSubmit);
            inputField.onValueChanged.AddListener(OnValueChanged);
            Log.RegisterLogListen(this);
        }

        private void OnDestroy() => Log.UnregisterLogListen(this);

        public void Init() => buildIdText.text = "0.0.0";

        public void LogMessage(string logContent, LogType logType)
        {
            if (_isOpen)
            {
                RunOnMainThread(() => ShowLog(logContent, logType));
            }
        }

        public void Shutdown()
        {
        }

        private bool IsOpen() => _isOpen;

        [Button]
        public void Open() => SetOpen(true);

        public void SetOpen(bool open)
        {
            _isOpen = open;
            panel.gameObject.SetActive(open);
            // ReSharper disable once InvertIf
            if (open)
            {
                inputField.ActivateInputField();
                LoadLocalLog();
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private void ShowLog(string logContent, LogType logType)
        {
            _lines.Add(logContent);

            var count = Mathf.Min(maxLogLines, _lines.Count);
            var start = _lines.Count - count;

            _stringBuilder.Clear();
            for (var i = start; i < _lines.Count; ++i)
            {
                var str = _lines[i];
                _stringBuilder.Append(str);
                _stringBuilder.Append("\n");
            }

            var text = _stringBuilder.ToString();
            var colorEndIndex = text.IndexOf("</color>", StringComparison.Ordinal);
            var colorStartIndex = text.IndexOf("<color=", StringComparison.Ordinal);
            if (colorEndIndex != -1 && colorEndIndex < colorStartIndex)
            {
                text = "<color=#CED6D6>" + text;
            }

            textArea.text = text;

            TrimExcessLogs();
        }

        private void LoadLocalLog()
        {
            var logPath = DirectoryUtils.GetBaseFilePersistentPath("logs") + "/gamelog";
            if (!File.Exists(logPath))
            {
                return;
            }

            _lines.Clear();

            var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reader = new StreamReader(fs);
            string content;
            while ((content = reader.ReadLine()) != null)
            {
                _lines.Add(content);
            }

            if (_lines.Count <= 0)
            {
                return;
            }

            var lastContent = _lines[_lines.Count - 1];
            _lines.RemoveAt(_lines.Count - 1);
            ShowLog(lastContent, LogType.Log);
        }

        private void RunOnMainThread(Action callback)
        {
            lock (_actionLock)
            {
                _actionList.Add(callback);
            }
        }

        private void ExecuteActions()
        {
            lock (_actionLock)
            {
                if (_actionList.Count == 0)
                {
                    return;
                }

                foreach (var action in _actionList)
                {
                    action?.Invoke();
                }

                _actionList.Clear();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleConsoleKey) || Input.GetKeyDown(KeyCode.Backslash))
            {
                SetOpen(!IsOpen());
            }

            if (!IsOpen())
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }

            ExecuteActions();
            OnGmKeyEvents();
        }

        private void LateUpdate()
        {
            // This has to happen here because keys like KeyUp will navigate the caret
            // int the UI event handling which runs between Update and LateUpdate
            if (_wantedCaretPosition <= -1)
            {
                return;
            }

            inputField.caretPosition = _wantedCaretPosition;
            _wantedCaretPosition = -1;
        }

        private void OnSubmit(string value)
        {
            // Only react to this if enter was actually pressed. Submit can also happen by mouse clicks.
            if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter))
            {
                return;
            }

            inputField.text = "";
            inputField.ActivateInputField();
            //Console.EnqueueCommand(value);
            if (gmCommander != null && !string.IsNullOrEmpty(value))
            {
                Log.LogInfo(gmCommander.ExecuteString(value));
            }

            ClearCandidate();
        }

        private void OnValueChanged(string value)
        {
            if (!_manualSetInput)
            {
                ClearCandidate();
            }
        }

        private List<string> _candidateCmd;
        private string _tabCmd;
        private int _candidateCmdIndex;
        private bool _manualSetInput;

        private void OnGmKeyEvents()
        {
            if (gmCommander == null)
            {
                return;
            }

            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return;
            }

            // This is to prevent clicks outside input field from removing focus
            inputField.ActivateInputField();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var inputText = inputField.text;
                // ReSharper disable once InvertIf
                if (inputField.caretPosition == inputText.Length && inputText.Length > 0)
                {
                    if (string.IsNullOrEmpty(_tabCmd))
                    {
                        _tabCmd = inputText;
                        _candidateCmd = gmCommander.GetActionOfStartWith(inputText);
                        _candidateCmdIndex = 0;
                    }

                    if (_candidateCmd.Count <= 0)
                    {
                        return;
                    }

                    var res = _candidateCmd[_candidateCmdIndex++ % _candidateCmd.Count];
                    _manualSetInput = true;
                    inputField.text = res;
                    _manualSetInput = false;
                    inputField.caretPosition = res.Length;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                var history = gmCommander.HistoryUp();
                if (string.IsNullOrEmpty(history))
                {
                    return;
                }

                inputField.text = history;
                _wantedCaretPosition = history.Length;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var history = gmCommander.HistoryDown();
                if (string.IsNullOrEmpty(history))
                {
                    return;
                }

                inputField.text = history;
                inputField.caretPosition = history.Length;
            }
        }

        private void ClearCandidate()
        {
            _candidateCmd = null;
            _tabCmd = null;
        }

        private void TrimExcessLogs()
        {
            var amountToRemove = Mathf.Max(_lines.Count - maxLogLines, 0);
            if (amountToRemove == 0)
            {
                return;
            }

            if (amountToRemove > maxLogLines / 2)
            {
                _lines.RemoveRange(0, amountToRemove);
            }
        }
    }
}