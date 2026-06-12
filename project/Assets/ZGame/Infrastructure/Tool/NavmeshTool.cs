using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ZGame
{
    public class NavmeshTool
    {
        static Vector3 getRandomPoint(Vector3 center, float radius)
        {
            Vector2 ran = UnityEngine.Random.insideUnitCircle;
            return center + radius * new Vector3(ran.x, 0, ran.y);
        }

        public static bool TryGetRandomPointInNavmesh(Vector3 center, ref Vector3 outPoint, float radius = 9)
        {
            int tryCount = 20;
            Vector3 result = Vector3.zero; ;
            bool flag = false;
            while (flag == false && tryCount > 0)
            {
                var pos = getRandomPoint(center, radius);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(pos, out hit, 10, 1 << 0))
                {
                    flag = true;
                    outPoint = hit.position;
                    return true;
                }
                tryCount--;
            }
            Debug.LogError("未能找到可达点");
            return false;
        }

        public static bool IsReachable(NavMeshAgent agent, Vector3 targetPos)
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(targetPos, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                //可达
                return true;
            }
            return false;
        }
    }
}
