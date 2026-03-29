using System;
using NnUnityEasings;

namespace NnUtils.Scripts.UI.Slideshow
{
    [Serializable]
    public class SlideshowTransition
    {
        public SlideshowTransitionType TransitionType;
        public float Duration;
        public Easing Easing;

        public SlideshowTransition() : this(SlideshowTransitionType.None, 0, Easing.Linear) { }

        public SlideshowTransition(SlideshowTransitionType transitionType, float duration, Easing easing)
        {
            TransitionType = transitionType;
            Duration = duration;
            Easing = easing;
        }


    }
}
