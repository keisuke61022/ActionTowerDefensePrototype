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
                _statusText.text = $"Player Base HP: {gm.PlayerBase.CurrentHp}   Enemy Base HP: {gm.EnemyBase.CurrentHp}\n" +
                                   $"Player HP: {gm.Player.CurrentHp}   Cost: {gm.CostManager.Current}/{gm.CostManager.Max}   Wave: {gm.WaveManager.CurrentWave}/5";
                UpdateCostGauge(gm.CostManager.Current, gm.CostManager.Max);
            }
        }

        public Vector2 GetStickDirection()
        {
            return _joystick == null ? Vector2.zero : _joystick.Direction;
        }

        public void BuildUI()
        {
            Canvas canvas = CreateCanvas();
            _statusText = CreateText(canvas.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -10f), 18, TextAnchor.UpperCenter);
            _helpText = CreateText(canvas.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 110f), 16, TextAnchor.MiddleCenter);
            _helpText.text = "Shoot: Space/Click/右ボタン  Place Turret: E/ユニット1\nMelee and Unit2-4 are placeholders";
            _messageText = CreateText(canvas.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 34, TextAnchor.MiddleCenter);

            BuildBottomPanel(canvas.transform);
        }

        public void SetMessage(string text)
        {
            if (_messageText != null) _messageText.text = text;
        }

        private void BuildBottomPanel(Transform parent)
        {
            var panel = CreateUIObject("BottomPanel", parent).AddComponent<Image>();
            panel.color = new Color(0f, 0f, 0f, 0.5f);
            var rt = panel.rectTransform;
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 0.26f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var stickArea = CreateUIObject("Joystick", panel.transform).AddComponent<Image>();
            stickArea.color = new Color(1f, 1f, 1f, 0.2f);
            var stickRt = stickArea.rectTransform;
            stickRt.anchorMin = new Vector2(0f, 0f);
            stickRt.anchorMax = new Vector2(0f, 1f);
            stickRt.sizeDelta = new Vector2(180f, 0f);
            stickRt.anchoredPosition = new Vector2(90f, 0f);
            _joystick = stickArea.gameObject.AddComponent<JoystickArea>();

            _shootButton = BuildButton(panel.transform, "Shoot", new Vector2(1f, 0.5f), new Vector2(-110f, 35f), new Vector2(80f, 60f), Color.red).gameObject.AddComponent<HoldButton>();
            BuildButton(panel.transform, "Melee\nN/A", new Vector2(1f, 0.5f), new Vector2(-20f, 35f), new Vector2(80f, 60f), Color.gray);

            _unitButton = BuildButton(panel.transform, "Unit1", new Vector2(1f, 0.5f), new Vector2(-110f, -35f), new Vector2(80f, 60f), new Color(0.2f, 0.7f, 1f)).gameObject.AddComponent<PressButton>();
            BuildButton(panel.transform, "Unit2\nN/A", new Vector2(1f, 0.5f), new Vector2(-20f, -35f), new Vector2(80f, 60f), Color.gray);
            BuildButton(panel.transform, "Unit3\nN/A", new Vector2(1f, 0.5f), new Vector2(-200f, -35f), new Vector2(80f, 60f), Color.gray);
            BuildButton(panel.transform, "Unit4\nN/A", new Vector2(1f, 0.5f), new Vector2(-290f, -35f), new Vector2(80f, 60f), Color.gray);

            _costGaugeParent = CreateUIObject("CostGauge", parent).GetComponent<RectTransform>();
            _costGaugeParent.anchorMin = new Vector2(0f, 0f);
            _costGaugeParent.anchorMax = new Vector2(1f, 0f);
            _costGaugeParent.sizeDelta = new Vector2(0f, 32f);
            _costGaugeParent.anchoredPosition = new Vector2(0f, 16f);

            for (int i = 0; i < 10; i++)
            {
                var cell = CreateUIObject($"Cost{i + 1}", _costGaugeParent).AddComponent<Image>();
                var cellRt = cell.rectTransform;
                cellRt.anchorMin = new Vector2(i / 10f, 0f);
                cellRt.anchorMax = new Vector2((i + 1) / 10f, 1f);
                cellRt.offsetMin = new Vector2(2f, 2f);
                cellRt.offsetMax = new Vector2(-2f, -2f);
            }
        }

        private void UpdateCostGauge(int current, int max)
        {
            if (_costGaugeParent == null) return;
            for (int i = 0; i < _costGaugeParent.childCount; i++)
            {
                var img = _costGaugeParent.GetChild(i).GetComponent<Image>();
                img.color = i < current ? Color.green : new Color(0.2f, 0.2f, 0.2f);
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
            return canvas;
        }

        private Text CreateText(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, int size, TextAnchor alignment)
        {
            var textGo = CreateUIObject("Text", parent);
            var text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;
            var rt = text.rectTransform;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.sizeDelta = new Vector2(980f, 120f);
            rt.anchoredPosition = anchoredPos;
            return text;
        }

        private Image BuildButton(Transform parent, string label, Vector2 anchor, Vector2 anchoredPos, Vector2 size, Color color)
        {
            var image = CreateUIObject(label, parent).AddComponent<Image>();
            image.color = color;
            var rt = image.rectTransform;
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            var txt = CreateText(image.transform, Vector2.zero, Vector2.one, Vector2.zero, 16, TextAnchor.MiddleCenter);
            txt.text = label;
            txt.resizeTextForBestFit = true;
            return image;
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
