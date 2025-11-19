using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public static class BilliardsSceneBootstrapper
    {
        private const string TargetSceneName = "InGame";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name.Equals(TargetSceneName) == false)
                return;

            CharacterHealth archer = EnsureCharacter("ArcherRoot", 40f, new Vector3(0f, -1.1f, 0f), false);
            CharacterHealth enemy = EnsureCharacter("EnemyDummy", 25f, new Vector3(0f, -1.1f, 0f), true);

            CardIconDisplay iconDisplay = EnsureComponent<CardIconDisplay>("ArcherRoot");
            iconDisplay?.ResetIcons();

            EnemyController enemyController = EnsureComponent<EnemyController>("EnemyDummy");
            if (enemyController != null)
            {
                Transform modelRoot = GameObject.Find("EnemyDummy")?.transform;
                enemyController.Configure(enemy, archer, modelRoot);
            }

            CardObstacle[] cards = new[]
            {
                EnsureCardObstacle("CardObstacle_1"),
                EnsureCardObstacle("CardObstacle_2"),
                EnsureCardObstacle("CardObstacle_3"),
                EnsureCardObstacle("CardObstacle_4")
            };

            CardSpawnManager spawnManager = EnsureComponent<CardSpawnManager>("PlayfieldRoot");
            if (spawnManager != null)
                spawnManager.AssignPool(cards);
        }

        private static CharacterHealth EnsureCharacter(string objectName, float maxHealth, Vector3 barOffset, bool deactivateOnDeath)
        {
            GameObject target = GameObject.Find(objectName);
            if (target == null)
                return null;

            CharacterHealth health = target.GetComponent<CharacterHealth>();
            if (health == null)
                health = target.AddComponent<CharacterHealth>();

            health.SetMaxHealth(maxHealth);
            health.SetDeactivateOnDeath(deactivateOnDeath);

            HealthBarUI bar = target.GetComponentInChildren<HealthBarUI>();
            if (bar == null)
                bar = CreateHealthBar(target.transform, barOffset);

            if (bar != null)
                health.AssignHealthBar(bar);

            return health;
        }

        private static HealthBarUI CreateHealthBar(Transform parent, Vector3 offset)
        {
            Sprite backSprite = Resources.Load<Sprite>("Textures/HealthBack");
            Sprite fillSprite = Resources.Load<Sprite>("Textures/HealthFill");
            if (backSprite == null || fillSprite == null)
                return null;

            GameObject root = new GameObject("HealthBar");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = offset;

            SpriteRenderer backRenderer = root.AddComponent<SpriteRenderer>();
            backRenderer.sprite = backSprite;
            backRenderer.sortingOrder = 20;

            GameObject fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(root.transform, false);
            SpriteRenderer fillRenderer = fillObject.AddComponent<SpriteRenderer>();
            fillRenderer.sprite = fillSprite;
            fillRenderer.sortingOrder = 21;

            HealthBarUI bar = root.AddComponent<HealthBarUI>();
            bar.Initialize(fillRenderer.transform);
            return bar;
        }

        private static CardObstacle EnsureCardObstacle(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
                return null;

            CardObstacle obstacle = obj.GetComponent<CardObstacle>();
            if (obstacle == null)
                obstacle = obj.AddComponent<CardObstacle>();

            return obstacle;
        }

        private static T EnsureComponent<T>(string objectName) where T : Component
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
                return null;

            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();

            return component;
        }
    }
}
