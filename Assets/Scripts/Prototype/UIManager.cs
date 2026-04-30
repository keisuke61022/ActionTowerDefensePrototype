using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeTD
{
    public class UIManager : MonoBehaviour
    {
        private Text _statusText;
        private Text _messageText;
        private Text _helpText;
        private Text _costLabel;
        private RectTransform _costGaugeParent;

        private JoystickArea _joystick;
        private HoldButton _shootButton;
        private PressButton _unitButton;

        public bool ShootPressed => _shootButton != null && _shootButton.Pressed;
        public bool PlaceUnit1Pressed => _unitButton != null && _unitButton.ConsumePressed();

        private void Update()
        {
            if (_statusText != null)
            {
                var gm = GameManager.Instance;
                _statusText.text = $"Base HP: {gm.PlayerBase.CurrentHp}/{gm.PlayerBase.MaxHp}  Enemy Base HP: {gm.EnemyBase.CurrentHp}/{gm.EnemyBase.MaxHp}\n" +
                                   $"Player HP: {gm.Player.CurrentHp}  Cost: {gm.CostManager.Current}/{gm.CostManager.Max}  Wave: {gm.WaveManager.CurrentWave}/5";
                _costLabel.text = $"Cost {gm.CostManager.Current}/{gm.CostManager.Max}";
                UpdateCostGauge(gm.CostManager.Current);
            }
        }

        public Vector2 GetStickDirection() => _joystick == null ? Vector2.zero : _joystick.Direction;

        public void BuildUI()
        {
            Canvas canvas = CreateCanvas();
            _statusText = CreateText(canvas.transform, "Status", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), 20, TextAnchor.UpperCenter, new Vector2(1020f, 120f));
            _helpText = CreateText(canvas.transform, "Help", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 338f), 18, TextAnchor.MiddleCenter, new Vector2(1020f, 72f));
            _helpText.text = "Move: WASD/Arrows/Stick  Shoot: Space  Place Unit1: E";
            _messageText = CreateText(canvas.transform, "Message", new Vector2(0.5f, 0.67f), new Vector2(0.5f, 0.67f), Vector2.zero, 52, TextAnchor.MiddleCenter, new Vector2(980f, 200f));

            BuildBottomPanel(canvas.transform);
        }

        public void SetMessage(string text)
        {
            if (_messageText != null) _messageText.text = text;
        }

        private void BuildBottomPanel(Transform parent)
        {
            var panel = CreateUIObject("BottomPanel", parent).AddComponent<Image>();
            panel.color = new Color(0f, 0f, 0f, 0.55f);
            var rt = panel.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.08f);
            rt.anchorMax = new Vector2(1f, 0.34f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var stickArea = CreateUIObject("Joystick", panel.transform).AddComponent<Image>();
            stickArea.color = new Color(1f, 1f, 1f, 0.12f);
            var stickRt = stickArea.rectTransform;
            stickRt.anchorMin = new Vector2(0.03f, 0.1f);
            stickRt.anchorMax = new Vector2(0.38f, 0.9f);
            stickRt.offsetMin = Vector2.zero;
            stickRt.offsetMax = Vector2.zero;
            _joystick = stickArea.gameObject.AddComponent<JoystickArea>();

            var actions = CreateUIObject("Actions", panel.transform).GetComponent<RectTransform>();
            actions.anchorMin = new Vector2(0.50f, 0.08f);
            actions.anchorMax = new Vector2(0.98f, 0.92f);
            actions.offsetMin = Vector2.zero;
            actions.offsetMax = Vector2.zero;

            _shootButton = BuildGridButton(actions, "Shoot", 0, 0, new Color(0.82f, 0.2f, 0.2f)).gameObject.AddComponent<HoldButton>();
            BuildGridButton(actions, "Melee\nN/A", 1, 0, new Color(0.35f, 0.35f, 0.35f));
            _unitButton = BuildGridButton(actions, "Unit1", 2, 0, new Color(0.18f, 0.58f, 0.95f)).gameObject.AddComponent<PressButton>();
            BuildGridButton(actions, "Unit2\nN/A", 0, 1, new Color(0.35f, 0.35f, 0.35f));
            BuildGridButton(actions, "Unit3\nN/A", 1, 1, new Color(0.35f, 0.35f, 0.35f));
            BuildGridButton(actions, "Unit4\nN/A", 2, 1, new Color(0.35f, 0.35f, 0.35f));

            var gaugeBg = CreateUIObject("CostGaugeBg", parent).AddComponent<Image>();
            gaugeBg.color = new Color(0f, 0f, 0f, 0.82f);
            var gaugeBgRt = gaugeBg.rectTransform;
            gaugeBgRt.anchorMin = new Vector2(0f, 0f);
            gaugeBgRt.anchorMax = new Vector2(1f, 0.075f);
            gaugeBgRt.offsetMin = Vector2.zero;
            gaugeBgRt.offsetMax = Vector2.zero;

            _costLabel = CreateText(gaugeBg.transform, "CostLabel", new Vector2(0.5f, 0.68f), new Vector2(0.5f, 0.68f), Vector2.zero, 18, TextAnchor.MiddleCenter, new Vector2(320f, 26f));
            _costGaugeParent = CreateUIObject("CostGauge", gaugeBg.transform).GetComponent<RectTransform>();
            _costGaugeParent.anchorMin = new Vector2(0.02f, 0.05f);
            _costGaugeParent.anchorMax = new Vector2(0.98f, 0.45f);
            _costGaugeParent.offsetMin = Vector2.zero;
            _costGaugeParent.offsetMax = Vector2.zero;

            for (int i = 0; i < 10; i++)
            {
                var cell = CreateUIObject($"Cost{i + 1}", _costGaugeParent).AddComponent<Image>();
                var cellRt = cell.rectTransform;
                cellRt.anchorMin = new Vector2(i / 10f, 0f);
                cellRt.anchorMax = new Vector2((i + 1) / 10f, 1f);
                cellRt.offsetMin = new Vector2(2f, 1f);
                cellRt.offsetMax = new Vector2(-2f, -1f);
                cell.color = new Color(0.2f, 0.2f, 0.2f);
            }
        }

        private Image BuildGridButton(RectTransform parent, string label, int row, int col, Color color)
        {
            var image = CreateUIObject(label, parent).AddComponent<Image>();
            image.color = color;
            var rt = image.rectTransform;
            float rows = 3f;
            float cols = 2f;
            float xMin = col / cols;
            float xMax = (col + 1f) / cols;
            float yMax = 1f - row / rows;
            float yMin = 1f - (row + 1f) / rows;
            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.offsetMin = new Vector2(5f, 5f);
            rt.offsetMax = new Vector2(-5f, -5f);

            var txt = CreateText(image.transform, "Label", Vector2.zero, Vector2.one, Vector2.zero, 24, TextAnchor.MiddleCenter, Vector2.zero);
            txt.text = label;
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = 12;
            txt.resizeTextMaxSize = 24;
            return image;
        }

        private void UpdateCostGauge(int current)
        {
            if (_costGaugeParent == null) return;
            for (int i = 0; i < _costGaugeParent.childCount; i++)
            {
                var img = _costGaugeParent.GetChild(i).GetComponent<Image>();
                img.color = i < current ? new Color(0.15f, 0.85f, 0.2f) : new Color(0.2f, 0.2f, 0.2f);
            }
        }

        private Canvas CreateCanvas()
        {
            var canvasGo = new GameObject("GameCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;
            return canvas;
        }

        private Text CreateText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, int size, TextAnchor alignment, Vector2 sizeDelta)
        {
            var textGo = CreateUIObject(name, parent);
            var text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;
            var rt = text.rectTransform;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.sizeDelta = sizeDelta;
            rt.anchoredPosition = anchoredPos;
            return text;
        }

        private GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }
    }

    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool Pressed { get; private set; }
        public void OnPointerDown(PointerEventData eventData) => Pressed = true;
        public void OnPointerUp(PointerEventData eventData) => Pressed = false;
    }

    public class PressButton : MonoBehaviour, IPointerClickHandler
    {
        private bool _pressed;
        public void OnPointerClick(PointerEventData eventData) => _pressed = true;
        public bool ConsumePressed()
        {
            bool pressed = _pressed;
            _pressed = false;
            return pressed;
        }
    }

    public class JoystickArea : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Vector2 Direction { get; private set; }
        public void OnPointerDown(PointerEventData eventData) => UpdateDirection(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateDirection(eventData);
        public void OnPointerUp(PointerEventData eventData) => Direction = Vector2.zero;

        private void UpdateDirection(PointerEventData data)
        {
            RectTransform rt = (RectTransform)transform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, data.position, data.pressEventCamera, out var local))
            {
                Vector2 normalized = new Vector2(local.x / (rt.rect.width * 0.5f), local.y / (rt.rect.height * 0.5f));
                Direction = Vector2.ClampMagnitude(normalized, 1f);
            }
        }
    }
}
