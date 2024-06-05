using System;
using System.Collections;
using Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
    {
        private Animator _animator;
        private float _animationDuration = 2f;

        private int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;
        
        private static readonly int SwipeIn = Animator.StringToHash("SwipeIn");
        private static readonly int SwipeOut = Animator.StringToHash("SwipeOut");

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            
            if (CurrentSceneIndex != 0)
                _animator.SetTrigger(SwipeOut);
        }
        
        public void LoadScene(int buildIndex)
        {
            StartCoroutine(TransitionToScene(buildIndex));
        }

        private IEnumerator TransitionToScene(int buildIndex)
        {
            _animator.SetTrigger(SwipeIn);
            
            yield return new WaitForSeconds(_animationDuration);
            
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }
    }
}
