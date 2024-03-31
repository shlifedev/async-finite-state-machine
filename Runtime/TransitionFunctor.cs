using System;
using Cysharp.Threading.Tasks;

namespace LD.AI.AsyncFSM
{
    /// <summary>
    /// ITransition의 콜백 클래스 구현
    /// </summary>
    public class TransitionFunctor : ITransition
    {
        public Func<UniTask<bool>> Functor;

        public TransitionFunctor(Func<UniTask<bool>> functor)
        {
            Functor = functor;
        }

        public async UniTask<bool> ShouldTransition() => await Functor(); 
    }
}