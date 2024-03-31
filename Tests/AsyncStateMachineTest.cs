using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LD.AI.AsyncFSM;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

public class AsyncStateMachineTest
{
    public static int testValue = 0;
    public class StateTest : StateBase<int>
    {
        public override async UniTask OnStateEnter()
        {
            
            Debug.Log($"{nameof(OnStateEnter)} {nameof(StateTest)} {Time.frameCount}");
        }

        public override async UniTask OnStateUpdate()
        {
            Debug.Log($"{nameof(OnStateUpdate)} {nameof(StateTest)} {Time.frameCount}");
        }

        public override async UniTask OnStateExit()
        {
            Debug.Log($"{nameof(OnStateExit)} {nameof(StateTest)} {Time.frameCount}");
        }
    }
    
    public class StateTestB : StateBase<int>
    {
        public override async UniTask OnStateEnter()
        {
            Debug.Log($"{nameof(OnStateEnter)} {nameof(StateTestB)} {Time.frameCount}");
        }

        public override async UniTask OnStateUpdate()
        {
            Debug.Log($"{nameof(OnStateUpdate)} {nameof(StateTestB)} {Time.frameCount}");
        }

        public override async UniTask OnStateExit()
        {
            Debug.Log($"{nameof(OnStateExit)} {nameof(StateTestB)} {Time.frameCount}");
        }
    }
    
    public class NonAsyncTaskState : StateBase<int>
    {
        public override UniTask OnStateEnter()
        {
            Debug.Log($"{nameof(OnStateEnter)} {nameof(NonAsyncTaskState)} {Time.frameCount}");
            return UniTask.CompletedTask;
        }

        public override UniTask OnStateUpdate()
        {
            Debug.Log($"{nameof(OnStateUpdate)} {nameof(NonAsyncTaskState)} {Time.frameCount}");
            return UniTask.CompletedTask;
        }

        public override UniTask OnStateExit()
        {
            Debug.Log($"{nameof(OnStateExit)} {nameof(NonAsyncTaskState)} {Time.frameCount}");
            return UniTask.CompletedTask;
        }
    }
    
    CancellationTokenSource source = new CancellationTokenSource();
    private AsyncStateMachine<int> fsm = null;
    
    
    [UnityTest] public IEnumerator NullDefaultState()
    { 
        fsm = new AsyncStateMachine<int>(source.Token); 
        fsm.Add(1, new StateTest());
        fsm.Add(2, new StateTestB()); 
        yield return CreateTest();    
        async UniTask CreateTest()
        { 
            Assert.IsTrue(fsm.CurState == null); 
        }
        source.Cancel();
    }
    
    
    [UnityTest] public IEnumerator NotNull()
    { 
        fsm = new AsyncStateMachine<int>(source.Token); 
        fsm.Add(1, new StateTest());
        fsm.Add(2, new StateTestB());
        fsm.InitializeAndStartLoopAsync(1); 
        yield return CreateTest();   
        async UniTask CreateTest()
        { 
            Assert.IsTrue(fsm.CurState != null); 
        } 
        source.Cancel();
    }
    
    [UnityTest] public IEnumerator ChangeStateSafe()
    { 
        fsm = new AsyncStateMachine<int>(source.Token); 
        fsm.Add(1, new StateTest());
        fsm.Add(2, new StateTestB()); 
        fsm.Add(3, new NonAsyncTaskState()); 
        yield return CreateTest();   
        async UniTask CreateTest()
        {  
            await fsm.InitializeAndStartLoopAsync(1);
            Assert.IsTrue(fsm.CurState == 1);
            UniTask.Delay(1000);
            await fsm.ChangeStateAsync(2);
            Assert.IsTrue(fsm.CurState == 2);
            UniTask.Delay(1000); 
            await fsm.ChangeStateAsync(3); 
            Assert.IsTrue(fsm.CurState == 3);
            UniTask.Delay(1000); 
            await fsm.ChangeStateAsync(1); 
        }
        source.Cancel();
    }
}
