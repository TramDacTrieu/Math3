using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        SpriteShapeController spriteShapeController;
        public Transform target;
        bool TargetReached;
        float damping = 0.5f;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        GeneratorItems[] spawners;
        public bool needToScroll;
        Rect Bounds;
        Camera cam;
        Vector2[] path;
        private GameObject[] targetsArray;
        public bool useCenterX;
        public float xCenter;

        public static float direction = 1f;

        GameObject guide;

        // Use this for initialization
        private void CameraStart()
        {
            guide = new GameObject();
            Physics2D.autoSimulation = false;
            // spawners = GetSpawnerOnScreen();
            // if (spawner == null)
            // spawner = GameObject.Instantiate((Resources.Load("Prefabs/SpawnItems") as GameObject)).GetComponent<GeneratorItems>();
            cam = GetComponent<Camera>();
            if (spriteShapeController == null) spriteShapeController = GameObject.FindObjectOfType<SpriteShapeController>();
            Bounds = GetBounds(spriteShapeController);
            path = spriteShapeController.GetComponent<LevelSettings>().GetPathCollider();
            float yPivot = path.First().y + 10;
            var tar = path.Where(i => i.y <= yPivot).OrderByDescending(i => i.y).First();
            transform.position = new Vector3(tar.x, tar.y, transform.position.z);
            guide.transform.position = transform.position;
            Sprite back = FindObjectOfType<Background>().GetComponent<Image>().sprite;
            Camera.main.orthographicSize = back.bounds.size.x / Screen.width * Screen.height / 2;
            needToScroll = NeedToScroll();
            if (!needToScroll) transform.position = new Vector3(Bounds.center.x, Bounds.center.y, transform.position.z);
            else
            {
                StartCoroutine(UpdateDelayed());
            }
            StartCoroutine(FindTargetAndWait());

        }



        private void OnEnable()
        {
            LevelManager.OnEnterGame += CameraStart;
            LevelManager.OnNextMove += GenerateItems;
            // TouchBlocker.OnUnblocked += GenerateItems;
        }

        private void OnDisable()
        {
            LevelManager.OnEnterGame -= CameraStart;
            LevelManager.OnNextMove -= GenerateItems;
            // TouchBlocker.OnUnblocked -= GenerateItems;

        }

        private void GenerateItems()
        {
            if (LevelManager.THIS.moveID % 1 == 0 /* || !CheckMatches() */)
            {
                // int destroyCounter = GeneratorItems.destroyCounter;
                // if (destroyCounter > 0)
                {
                    // LevelManager.THIS.touchBlocker.blocked = true;
                    FindTarget();
                    GeneratorItems[] generatorItems = GetSpawner();
                    generatorItems.ForEachY(i => i.GenerateItems(generatorItems.Length));
                }
            }
        }

        private GeneratorItems[] GetSpawner()
        {
            var spawners = GameObject.FindObjectsOfType<GeneratorItems>();
            if (target != null)
            {
                GeneratorItems[] generatorItems = spawners.Where(i => i.worldRect.Contains(transform.position)).ToArray();
                if (generatorItems.Count() == 0) generatorItems = spawners.Where(i => i.destroyCounter > 0).ToArray();
                return generatorItems;
            }
            else
                return spawners.Take(1).ToArray();
        }

        Rect GetBounds(SpriteShapeController spriteShapeController)
        {
            Vector2[] vertices = spriteShapeController.GetComponent<EdgeCollider2D>().points;

            float minX = vertices.Min(i => i.x) * spriteShapeController.transform.localScale.x + spriteShapeController.transform.position.x;
            float minY = vertices.Min(i => i.y) * spriteShapeController.transform.localScale.y + spriteShapeController.transform.position.y;
            float maxX = vertices.Max(i => i.x) * spriteShapeController.transform.localScale.x + spriteShapeController.transform.position.x;
            float maxY = vertices.Max(i => i.y) * spriteShapeController.transform.localScale.y + spriteShapeController.transform.position.y;
            float width = (maxX - minX)/*  * spriteShapeController.transform.localScale.x */;
            float height = (maxY - minY)/*  * spriteShapeController.transform.localScale.y */;
            return new Rect(minX, minY, width, height);
        }

        bool NeedToScroll()
        {
            bool need = false;
            var levelBounds = GetBounds(spriteShapeController);

            var camRect = cam.GetCameraWorldRect();
            camRect = new Rect(camRect.x, camRect.y + 3, camRect.size.x, camRect.size.y - 6f);
            Debug.DrawLine(camRect.min, camRect.max);
            if (camRect.size.magnitude < levelBounds.size.magnitude)
                need = true;
            return need;
        }


        IEnumerator FindTargetAndWait()
        {
            FindTarget();
            if (!needToScroll && target != null)
            {
                StartCoroutine(StartFall());
            }
            else if (target == null)
            {
                yield return new WaitForSeconds(1);
                LevelManager.THIS.gameStatus = GameState.WaitForPopup;
                Physics2D.autoSimulation = true;
                yield return new WaitForSeconds(2);
                LevelManager.THIS.gameStatus = GameState.Tutorial;
            }
            if (target == null) TargetReached = true;
        }

        private void FindTarget()
        {
            // if (target == null)
            {
                targetsArray = GameObject.FindObjectsOfType<Diamond>().Where(i => !i.Collected).Select(i => i.gameObject).ToArray();
                if (targetsArray.Length == 0) targetsArray = FindObjectsOfType<NestedBig>().Select(i => i.gameObject).Concat(FindObjectsOfType<Nested>().Select(c => c.gameObject).Concat(FindObjectsOfType<KeyHolder>().Select(c => c.gameObject))).ToArray();

                CheckTargetPosition();
            }
        }

        private void CheckTargetPosition()
        {
            if (targetsArray != null && targetsArray.Length > 0)
            {
                if (direction > 0)
                    target = targetsArray.OrderByDescending(i => i.transform.position.y).First().transform;
                else
                    target = targetsArray.OrderBy(i => i.transform.position.y).First().transform;
                m_LastTargetPosition = target.position;
                m_OffsetZ = (transform.position - target.position).z;
                transform.parent = null;
            }
        }

        IEnumerator UpdateDelayed()
        {
            while (true)
            {
                CheckTargetPosition();
                if (target != null)
                {
                    float xMoveDelta = (target.position - m_LastTargetPosition).x;

                    bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

                    if (updateLookAheadTarget)
                    {
                        m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
                    }
                    else
                    {
                        m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
                    }

                    aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
                    float closestX = path.OrderBy(item => Math.Abs(guide.transform.position.y - item.y)).FirstOrDefault().x;
                    if (useCenterX) closestX = xCenter;
                    aheadTargetPos.x = closestX;
                }
                yield return new WaitForSeconds(.3f);
            }
        }

        // public float x;
        // public float y;
        private Vector3 aheadTargetPos;
        private Vector3 speedV;
        float speedX;
        float speedY;
        // Update is called once per frame
        private void Update()
        {
            // for (int i = 0; i < path.Length - 1; i++)
            // {
            //     Debug.DrawLine(path[i], path[i + 1]);
            // }
            if (target != null && (target.GetComponent<ICollectable>()?.IsCollected() ?? false))
                target = null;
            if (!needToScroll) return;
            if (target == null && needToScroll) FindTarget();
            if (target == null) return;
            // only update lookahead pos if accelerating or changed direction


            Vector3 newPos = Vector3.SmoothDamp(guide.transform.position, aheadTargetPos, ref m_CurrentVelocity, 0);
            // newPos.x = closestX;

            guide.transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

            m_LastTargetPosition = target.position;

            if (!TargetReached)
            {
                if (Vector2.Distance(guide.transform.position, target.position) < 10)
                {
                    StartCoroutine(StartFall());
                    TargetReached = true;
                }
            }
            if (needToScroll)
            {
                newPos = ApplyBounds(guide.transform.position);
                // if (Vector2.Distance(newPos, transform.position) > .2f)
                //     ItemPhysicsEditor.LowerDinamicTreshold = newPos + Vector3.down * 10 * direction;
            }
            var v = new Vector3(newPos.x, newPos.y, transform.position.z);
            // float x = Mathf.SmoothDamp(transform.position.x, v.x, ref speedX, 1f);
            // float y = Mathf.SmoothDamp(transform.position.y, v.y, ref speedY, .1f);
            // transform.position = new Vector3(x, y, -10);
            transform.position = Vector3.SmoothDamp(transform.position, v, ref speedV, 1f);
            // float v = transform.position.y + 12;
            // closestX = path.OrderBy(item => Math.Abs(v - item.y)).FirstOrDefault().x;
            // spawners.transform.position = new Vector2(closestX, v);
        }

        private IEnumerator StartFall()
        {
            LevelManager.THIS.gameStatus = GameState.WaitForPopup;
            yield return new WaitForSeconds(2);
            LevelManager.THIS.gameStatus = GameState.Tutorial;
            yield return new WaitUntil(() => LevelManager.THIS.gameStatus == GameState.Playing);
            Physics2D.autoSimulation = true;
            yield return new WaitForSeconds(2);
            target.GetComponent<Diamond>()?.StartFall();

        }

        private Vector2 ApplyBounds(Vector2 position)
        {
            float cameraHeight = Camera.main.orthographicSize * 2f;
            float cameraWidth = (Screen.width * 1f / Screen.height) * cameraHeight;
            // position.x = Mathf.Max(position.x, Bounds.min.x + cameraWidth / 2f);
            position.y = Mathf.Max(position.y, (Bounds.min.y - 4) + cameraHeight / 2f);
            // position.x = Mathf.Min(position.x, Bounds.max.x - cameraWidth / 3f);
            position.y = Mathf.Min(position.y, (Bounds.max.y + 4) - cameraHeight / 2f);
            return position;
        }

        public IColorableComponent[] GetVisibleItems()
        {
            return GetVisibleObjects().Select(i => i?.GetComponent<IColorableComponent>()).Where(i => i?.IDestroyableComponent).ToArray();
        }

        public GameObject[] GetVisibleObjects()
        {
            var camRect = cam.GetCameraWorldRect();
            camRect = new Rect(camRect.x, camRect.y + 3, camRect.size.x, camRect.size.y - 6.5f);
            return FindObjectsOfType<GameObject>().Where(i => camRect.Contains(i.transform.position)).ToArray();
        }

        bool IsObjectVisible(Vector2 position)
        {
            var camRect = cam.GetCameraWorldRect();
            camRect = new Rect(camRect.x, camRect.y + 3, camRect.size.x, camRect.size.y - 6.5f);
            return camRect.Contains(position);
        }

        bool CheckMatches()
        {
            var items = GetVisibleItems();
            foreach (var item in items)
            {
                int v = item.GetNearMatches().Count();
                if (v > 0) return true;
            }
            return false;
        }

    }
}
