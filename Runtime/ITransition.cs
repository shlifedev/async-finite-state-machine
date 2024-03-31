using Cysharp.Threading.Tasks;

namespace Macovill.LuckyByte
{
    public interface ITransition
    {  
        /// <summary>
        /// 트랜지션 가능한가?
        /// </summary> 
        UniTask<bool> ShouldTransition(); 
    }
}