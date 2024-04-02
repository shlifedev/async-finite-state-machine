using System;
using LD.StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.shlifedev.framework.scripts.core.ai.fsm.AsyncFSM.Example
{
    public class ExampleView : MonoBehaviour
    {
        public AsyncStateMachine<int> fsm;
        public StateViewer ViewA, ViewB;
        public Button Change;
        private void Awake()
        {
            fsm = new AsyncStateMachine<int>(this);
            fsm.Add(0, new ExampleStateA(this));
            fsm.Add(1, new ExampleStateB(this));  
            fsm.InitializeAndStartLoopAsync(0);

            Change.onClick.AddListener(async () =>
            {
                if (fsm.CurState == 0)
                {
                    await fsm.ChangeStateAsync(1);
                }
                else
                {
                    await fsm.ChangeStateAsync(0);
                }
            });
        }
    }
}