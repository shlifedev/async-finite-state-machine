using Cysharp.Threading.Tasks;
using LD.StateMachine;
using UnityEngine;

namespace com.shlifedev.framework.scripts.core.ai.fsm.AsyncFSM.Example
{
    public abstract class ExampleState : StateBase<int> 
    {
        protected readonly ExampleView View;

        public ExampleState(ExampleView view)
        {
            View = view;
        }
        public override UniTask OnStateEnter()
        {
            return UniTask.CompletedTask; 
        }

        public override UniTask OnStateUpdate()
        {
            return UniTask.CompletedTask; 
        }

        public override UniTask OnStateExit()
        {
            return UniTask.CompletedTask; 
        }
    }

    public class ExampleStateA : ExampleState
    {
        public override async UniTask OnStateEnter()
        { 
            
            
            View.ViewA.Img.color = Color.green; 
            View.ViewA.State.text = $"Enter!";
            
            await UniTask.Delay(1000);
        }

        public override UniTask OnStateUpdate()
        {
            View.ViewA.Img.color = Color.yellow;
            View.ViewA.State.text = $"Update . . .";
            return UniTask.CompletedTask;;
        }

        public override async UniTask OnStateExit()
        { 
            View.ViewA.Img.color = Color.black;
            for (int i = 100; i >= 0; i--)
            {
                await  UniTask.Delay(1);
                View.ViewA.State.text = $"Exiting, Please Wait.. {i}";
            } 
            
            View.ViewA.Img.color = Color.red;

        }

        public ExampleStateA(ExampleView view) : base(view)
        {
        }
    }
  
    public class ExampleStateB : ExampleState
    {
        public override async UniTask OnStateEnter()
        {   
            View.ViewB.Img.color = Color.green; 
            View.ViewB.State.text = $"Enter!";
            await UniTask.Delay(1000);
        }

        public override UniTask OnStateUpdate()
        {
             
            View.ViewB.Img.color = Color.yellow;
            View.ViewB.State.text = $"Update . . .";
            Debug.Log("KK");
            return UniTask.CompletedTask;;
        }

        public override async UniTask OnStateExit()
        { 
            View.ViewB.Img.color = Color.black;
            for (int i = 100; i >= 0; i--)
            {
                await  UniTask.Delay(1);
                View.ViewB.State.text = $"Exiting, Please Wait.. {i}";
            } 
            View.ViewB.Img.color = Color.red;
        }

        public ExampleStateB(ExampleView view) : base(view)
        {
        }
    }
    
}