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
        private PressButton _turretButton;
        private PressButton _meleeButton;
        private PressButton _unit2Button;
        private PressButton _unit3Button;
        private PressButton _unit4Button;
        private Image _turretButtonImage;

        public bool ShootPressed => _shootButton != null && _shootButton.Pressed;
        public bool PlaceUnit1Pressed => _turretButton != null && _turretButton.ConsumePressed();

        private void Update()
        {
            if (_statusText == null) return;

            var gm = GameManager.Instance;
            _statusText.text = $"拠点HP: {gm.PlayerBase.CurrentHp}/{gm.PlayerBase.MaxHp}  敵拠点HP: {gm.EnemyBase.CurrentHp}/{gm.EnemyBase.MaxHp}\n" +
                               $"プレイヤーHP: {gm.Player.CurrentHp}  コスト: {gm.CostManager.Current}/{gm.CostManager.Max}  Wave: {gm.WaveManager.CurrentWave}/5";
            _costLabel.text = $"コスト {gm.CostManager.Current}/{gm.CostManager.Max}";
            UpdateCostGauge(gm.CostManager.Current);
            UpdateButtonStates(gm.CostManager.Current);
            HandleUnimplementedButtonMessage();
        }

        public Vector2 GetStickDirection() => _joystick == null ? Vector2.zero : _joystick.Direction;

        public void BuildUI()
        {
            Canvas canvas = CreateCanvas();
            _statusText = CreateText(canvas.transform, "Status", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), 20, TextAnchor.UpperCenter, new Vector2(1020f, 120f));
            _helpText = CreateText(canvas.transform, "Help", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 378f), 18, TextAnchor.MiddleCenter, new Vector2(1020f, 72f));
            _helpText.text = "移動: WASD/矢印/スティック  射撃: Space  設置: E";
            _messageText = CreateText(canvas.transform, "Message", new Vector2(0.5f, 0.67f), new Vector2(0.5f, 0.67f), Vector2.zero, 52, TextAnchor.MiddleCenter, new Vector2(980f, 200f));

            BuildBottomPanel(canvas.transform);
        }

        public void SetMessage(string text)
        {
            if (_messageText != null) _messageText.text = text;
        }

        private void HandleUnimplementedButtonMessage()
        {
            if ((_meleeButton != null && _meleeButton.ConsumePressed()) ||
                (_unit2Button != null && _unit2Button.ConsumePressed()) ||
                (_unit3Button != null && _unit3Button.ConsumePressed()) ||
                (_unit4Button != null && _unit4Button.ConsumePressed()))
            {
                SetMessage("未実装");
            }
        }

        private void BuildBottomPanel(Transform parent)
        {
            var panel = CreateUIObject("BottomPanel", parent).AddComponent<Image>();
            panel.color = new Color(0f, 0f, 0f, 0.6f);
            var rt = panel.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.09f);
            rt.anchorMax = new Vector2(1f, 0.36f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var stickArea = CreateUIObject("Joystick", panel.transform).AddComponent<Image>();
            stickArea.color = new Color(1f, 1f, 1f, 0.14f);
            var stickRt = stickArea.rectTransform;
            stickRt.anchorMin = new Vector2(0.03f, 0.14f);
            stickRt.anchorMax = new Vector2(0.35f, 0.86f);
            stickRt.offsetMin = Vector2.zero;
            stickRt.offsetMax = Vector2.zero;
            _joystick = stickArea.gameObject.AddComponent<JoystickArea>();
            _joystick.CreateKnobVisual();

            var actions = CreateUIObject("Actions", panel.transform).GetComponent<RectTransform>();
            actions.anchorMin = new Vector2(0.40f, 0.1f);
            actions.anchorMax = new Vector2(0.98f, 0.9f);
            actions.offsetMin = Vector2.zero;
            actions.offsetMax = Vector2.zero;

            _shootButton = BuildGridButton(actions, "射撃", 0, 0, new Color(0.88f, 0.27f, 0.22f)).gameObject.AddComponent<HoldButton>();
            _meleeButton = BuildGridButton(actions, "近接\n未実装", 0, 1, new Color(0.25f, 0.25f, 0.25f)).gameObject.AddComponent<PressButton>();
            _turretButtonImage = BuildGridButton(actions, "砲台\n3コスト", 1, 0, new Color(0.2f, 0.58f, 0.95f));
            _turretButton = _turretButtonImage.gameObject.AddComponent<PressButton>();
            _unit2Button = BuildGridButton(actions, "兵士\n4コスト", 1, 1, new Color(0.25f, 0.25f, 0.25f)).gameObject.AddComponent<PressButton>();
            _unit3Button = BuildGridButton(actions, "範囲\n5コスト", 2, 0, new Color(0.25f, 0.25f, 0.25f)).gameObject.AddComponent<PressButton>();
            _unit4Button = BuildGridButton(actions, "回復\n4コスト", 2, 1, new Color(0.25f, 0.25f, 0.25f)).gameObject.AddComponent<PressButton>();

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

        private void UpdateButtonStates(int currentCost)
        {
            if (_turretButtonImage == null) return;
            _turretButtonImage.color = currentCost >= 3 ? new Color(0.2f, 0.58f, 0.95f) : new Color(0.12f, 0.32f, 0.52f);
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
            rt.offsetMin = new Vector2(6f, 6f);
            rt.offsetMax = new Vector2(-6f, -6f);

            var outline = image.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.55f);
            outline.effectDistance = new Vector2(2f, -2f);

            var txt = CreateText(image.transform, "Label", Vector2.zero, Vector2.one, Vector2.zero, 22, TextAnchor.MiddleCenter, Vector2.zero);
            txt.text = label;
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = 11;
            txt.resizeTextMaxSize = 22;
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
        private RectTransform _knob;
        private float _knobRange = 48f;

        public void CreateKnobVisual()
        {
            var knobObj = new GameObject("Knob", typeof(RectTransform), typeof(Image));
            knobObj.transform.SetParent(transform, false);
            _knob = knobObj.GetComponent<RectTransform>();
            _knob.anchorMin = new Vector2(0.5f, 0.5f);
            _knob.anchorMax = new Vector2(0.5f, 0.5f);
            _knob.sizeDelta = new Vector2(84f, 84f);
            _knob.anchoredPosition = Vector2.zero;
            var image = knobObj.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.65f);
        }

        public void OnPointerDown(PointerEventData eventData) => UpdateDirection(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateDirection(eventData);
        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            if (_knob != null) _knob.anchoredPosition = Vector2.zero;
        }

        private void UpdateDirection(PointerEventData data)
        {
            RectTransform rt = (RectTransform)transform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, data.position, data.pressEventCamera, out var local))
            {
                Vector2 normalized = new Vector2(local.x / (rt.rect.width * 0.5f), local.y / (rt.rect.height * 0.5f));
                Direction = Vector2.ClampMagnitude(normalized, 1f);
                if (_knob != null)
                {
                    _knob.anchoredPosition = Direction * _knobRange;
                }
            }
        }
    }
}
