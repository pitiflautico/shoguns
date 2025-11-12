using UnityEngine;
using TMPro;

namespace ShogunsLegacy.Utils
{
    /// <summary>
    /// Floating damage number that appears when damage is dealt
    /// Animates upward and fades out
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageNumber : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private float riseSpeed = 2f;
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 1f);

        [Header("Movement")]
        [SerializeField] private bool randomizeDirection = true;
        [SerializeField] private float randomAngleRange = 45f;
        [SerializeField] private float driftSpeed = 0.5f;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color criticalColor = Color.yellow;
        [SerializeField] private Color healColor = Color.green;

        private TextMeshPro textMesh;
        private float timer = 0f;
        private Vector3 velocity;
        private Vector3 startScale;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
            startScale = transform.localScale;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            AnimateMovement();
            AnimateScale();
            AnimateFade();
        }

        /// <summary>
        /// Initialize damage number with value
        /// </summary>
        public void Initialize(float damageValue, bool isCritical = false, bool isHeal = false)
        {
            if (textMesh == null)
                textMesh = GetComponent<TextMeshPro>();

            // Set text
            textMesh.text = Mathf.CeilToInt(damageValue).ToString();

            // Set color
            if (isHeal)
            {
                textMesh.color = healColor;
            }
            else if (isCritical)
            {
                textMesh.color = criticalColor;
                textMesh.fontSize = textMesh.fontSize * 1.3f; // Bigger for crits
            }
            else
            {
                textMesh.color = normalColor;
            }

            // Set velocity
            velocity = Vector3.up * riseSpeed;

            if (randomizeDirection)
            {
                float angle = Random.Range(-randomAngleRange, randomAngleRange);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                velocity = rotation * velocity;
            }

            // Random horizontal drift
            velocity.x += Random.Range(-driftSpeed, driftSpeed);
        }

        private void AnimateMovement()
        {
            // Move upward (or in velocity direction)
            transform.position += velocity * Time.deltaTime;

            // Slow down over time
            velocity *= 0.98f;
        }

        private void AnimateScale()
        {
            float t = timer / lifetime;
            float scale = scaleCurve.Evaluate(t);
            transform.localScale = startScale * scale;
        }

        private void AnimateFade()
        {
            float t = timer / lifetime;
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, t * fadeSpeed);
            textMesh.color = color;
        }

        /// <summary>
        /// Spawn a damage number at position
        /// </summary>
        public static void Spawn(Vector3 position, float damage, bool isCritical = false, bool isHeal = false)
        {
            GameObject prefab = Resources.Load<GameObject>("DamageNumber");
            if (prefab == null)
            {
                Debug.LogWarning("[DamageNumber] Prefab not found in Resources folder!");
                return;
            }

            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            DamageNumber damageNumber = instance.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Initialize(damage, isCritical, isHeal);
            }
        }
    }
}
