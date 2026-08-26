#if !NOT_UNITY3D

#pragma warning disable 649

using ModestTree;
using UnityEngine;

namespace Zenject
{
    public abstract class MonoKernel : MonoBehaviour
    {
        [InjectLocal]
        InitializableManager _initializableManager = null;

        [InjectLocal]
        DisposableManager _disposablesManager = null;

        [InjectOptional] 
        protected IDecoratableMonoKernel decoratableMonoKernel;

        bool _hasInitialized;
        bool _isDestroyed;

        protected bool IsDestroyed
        {
            get { return _isDestroyed; }
        }

        public virtual void Start()
        {
            if (decoratableMonoKernel?.ShouldInitializeOnStart()??true)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            // We don't put this in start in case Start is overridden
            if (!_hasInitialized)
            {
                _hasInitialized = true;

                if (decoratableMonoKernel != null)
                {
                    decoratableMonoKernel.Initialize();
                }
                else
                {
                    _initializableManager.Initialize();
                }
            }
        }

        public virtual void OnDestroy()
        {
            // _disposablesManager can be null if we get destroyed before the Start event
            if (_disposablesManager != null)
            {
                Assert.That(!_isDestroyed);
                _isDestroyed = true;

                if (decoratableMonoKernel != null)
                {
                    decoratableMonoKernel.Dispose();
                    decoratableMonoKernel.LateDispose();
                }
                else
                {
                    _disposablesManager.Dispose();
                    _disposablesManager.LateDispose();
                }
            }
        }
    }
}

#endif
