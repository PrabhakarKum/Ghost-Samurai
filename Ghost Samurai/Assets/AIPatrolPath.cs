using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrolPath : MonoBehaviour
{
   public int patrolPathID = 0;
   public List<Vector3> patrolPoints = new List<Vector3>();

   private void Awake()
   {
      for (int i = 0; i < transform.childCount; i++)
      {
         patrolPoints.Add(transform.GetChild(i).position);
      }
   }

   private void Start()
   {
      if (WorldAIManager.instance != null)
      {
         WorldAIManager.instance.AddPatrolPathToList(this);
      }
   }
}
