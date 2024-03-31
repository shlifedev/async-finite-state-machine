using Cysharp.Threading.Tasks; 
using UnityEngine;

namespace LD.AI.AsyncFSM
{
    public abstract class StateBase<TStateKey> : IState where TStateKey : struct
    {   
        /// <summary>
        /// State Machine
        /// </summary>
        public AsyncStateMachine<TStateKey> StateMachine { get; set; }  
        /// <summary>
        /// Enter State
        /// </summary>
        /// <returns></returns>
        public abstract UniTask OnStateEnter();
        /// <summary>
        /// Update State
        /// </summary>
        public abstract UniTask OnStateUpdate();
        /// <summary>
        /// Exit State
        /// </summary>
        /// <returns></returns>
        public abstract UniTask OnStateExit();
    }
    
    
}