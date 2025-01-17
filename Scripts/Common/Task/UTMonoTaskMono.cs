using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Runtime.InteropServices;
using System.Security;
#endif

namespace UTGame
{
    public class UTMonoTaskMono : MonoBehaviour
    {
       
        private Queue<System.Action> taskQueue = new Queue<System.Action>();
        private bool isProcessingTasks = false;

        // 添加任务到队列
        public void AddTask(System.Action task)
        {
            taskQueue.Enqueue(task);
            if (!isProcessingTasks)
            {
                StartCoroutine(ProcessTasks());
            }
        }

        // IEnumerator 方法，用于处理任务队列
        private IEnumerator ProcessTasks()
        {
            isProcessingTasks = true;

            while (taskQueue.Count > 0)
            {
                // 等待一帧
                yield return null;

                // 执行一个任务
                System.Action task = taskQueue.Dequeue();
                task();
            }

            isProcessingTasks = false;
        }
    }
}



