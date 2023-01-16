using System.Collections.Generic;
using System.Linq;
using GameStates;
using Photon.Pun;
using UnityEngine;

namespace Entities.FogOfWar
{
    public class FogOfWarManager : MonoBehaviourPun
    {
        private static FogOfWarManager _instance;

        public static FogOfWarManager Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            cameraFog.orthographicSize = worldSize * 0.5f;
            cameraMinimap.orthographicSize = worldSize * 0.5f;
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        
        [SerializeField] private List<Entity> allViewables = new List<Entity>();
        [SerializeField] private List<Entity> allShowables = new List<Entity>();
        private IEnumerable<Entity> allyViewables => allViewables.Where(viewable => GameStates.GameStateMachine.Instance.GetPlayerTeam() == viewable.team);
        private IEnumerable<Entity> enemyShowables => allShowables.Where(showable => showable.isEnemyOfPlayer);
        
        [Header("Camera and Scene Setup")]
        public Camera cameraFog;
        public Camera cameraMinimap;

        public List<string> sceneToRenderFog;

        [Header("Fog Of War Parameter")] [Tooltip("Color for the area where the player can't see")]
        public Color fogColor = new Color(0.25f, 0.25f, 0.25f, 1f);

        public LayerMask layerTargetFogOfWar;

        [Tooltip("Material who is going to be render in the RenderPass")]
        public Material fogMat;

        [Tooltip("Define the size of the map to make the texture fit the RenderPass")]
        public int worldSize = 24;

        public bool worldSizeGizmos;

        //Parameter For Creating Field Of View Mesh
        public FOVSettings settingsFOV;
        
        public void AddFOWViewable(Entity viewable)
        {
            if (allViewables.Contains(viewable)) return;
            allViewables.Add(viewable);
            viewable.meshFilterFoV.gameObject.SetActive(true);
        }
        
        public void RemoveFOWViewable(Entity viewable)
        {
            if (!allViewables.Contains(viewable)) return;
            allViewables.Remove(viewable);
            viewable.meshFilterFoV.gameObject.SetActive(false);
        }
        
        public void AddFOWShowable(Entity viewable)
        {
            if (!allShowables.Contains(viewable)) allShowables.Add(viewable);
        }
        
        public void RemoveFOWShowable(Entity viewable)
        {
            if (allShowables.Contains(viewable)) allShowables.Remove(viewable);
        }


        private void Update()
        {
            TrySeeEnemies();
            RenderFOW();
            UpdateShowableElements();
        }

        private void TrySeeEnemies()
        {
            foreach (var viewable in allyViewables)
            {
                viewable.seenShowables.Clear();
                if(viewable == null) continue;
                var fromPosition = viewable.fogOfWarStartDetection.position;
                foreach (var enemy in enemyShowables.Where(enemy => Vector3.Distance(fromPosition,enemy.transform.position) <= viewable.viewRange))
                {
                    var toPosition = enemy.transform.position;
                    var direction = toPosition - fromPosition;
                    var hitwall = false;
                    if (Physics.Raycast(fromPosition, direction, out var hit, viewable.viewRange,
                        layerTargetFogOfWar))
                    {
                        if(hit.collider.gameObject.layer != 29) hitwall = true;
                    }
                    if(!hitwall) viewable.AddShowable(enemy);
                }
            }
        }
        
        private void RenderFOW()
        {
            foreach (var viewable in allyViewables.Where(ent => ent != null))
            {
                DrawFieldOfView(viewable);
            }
        }

        private void UpdateShowableElements()
        {
            foreach (var showable in enemyShowables)
            {
                showable.UpdateShow();
            }
        }

        private void InitMesh(MeshFilter viewMeshFilter)
        {
            var viewMesh = new Mesh();
            viewMeshFilter.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;
        }

        //Draw the Field of View for the Player.
        private void DrawFieldOfView(Entity entity)
        {
            int stepCount = Mathf.RoundToInt(entity.viewAngle * settingsFOV.meshResolution / 5);
            float stepAngleSize = entity.viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = entity.viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle, entity);

                if (i > 0)
                {
                    bool edgeDstThresholdExceeded =
                        Mathf.Abs(oldViewCast.dst - newViewCast.dst) > settingsFOV.edgeDstThreshold;
                    if (oldViewCast.hit != newViewCast.hit ||
                        (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast, entity);
                        if (edge.pointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointA);
                        }

                        if (edge.pointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }

                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = entity.transform.InverseTransformPoint(viewPoints[i]) +
                                  Vector3.forward * settingsFOV.maskCutawayDst;

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            Mesh viewMesh = entity.meshFilterFoV.mesh;
            if (viewMesh == null)
            {
                InitMesh(entity.meshFilterFoV);
            }

            viewMesh.Clear();

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, Entity entity)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < settingsFOV.edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle, entity);

                bool edgeDstThresholdExceeded =
                    Mathf.Abs(minViewCast.dst - newViewCast.dst) > settingsFOV.edgeDstThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        private ViewCastInfo ViewCast(float globalAngle, Entity entity)
        {
            Vector3 dir = DirFromAngle(globalAngle, true, entity);
            RaycastHit hit;
            if (!Physics.Raycast(entity.fogOfWarStartDetection.position, dir, out hit, entity.viewRange,
                layerTargetFogOfWar))
                return new ViewCastInfo(false, entity.transform.position + dir * entity.viewRange, entity.viewRange,
                    globalAngle);
            var candidateEntity = hit.collider.gameObject.GetComponent<Entity>();
            if (candidateEntity == null) return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            return new ViewCastInfo(false, entity.transform.position + dir * entity.viewRange, entity.viewRange,
                globalAngle);
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal, Entity entity)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += entity.transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dst;
            public float angle;

            public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
            {
                hit = _hit;
                point = _point;
                dst = _dst;
                angle = _angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
            {
                pointA = _pointA;
                pointB = _pointB;
            }
        }

        private void OnDrawGizmos()
        {
            if (!worldSizeGizmos) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(worldSize * 0.9f, 10, worldSize * 0.9f));
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(worldSize, 10, worldSize));
        }
    }
}

[System.Serializable]
public class FOVSettings
{
    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;
    public float maskCutawayDst = .1f;
}