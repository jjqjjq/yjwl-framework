using UnityEngine;

namespace JQFramework.tTimeCtrl.Clock
{
    /// <summary>
    /// Determines how a clock combines its time scale with that of others.
    /// </summary>
    public enum ClockBlend
    {
        /// <summary>
        /// The clock's time scale is multiplied with that of others.
        /// </summary>
        Multiplicative = 0,

        /// <summary>
        /// The clock's time scale is added to that of others.
        /// </summary>
        Additive = 1
    }


    public abstract class TC_Clock : MonoBehaviour
    {
        #region Properties

        [SerializeField]
        private float _localTimeScale = 1;

        /// <summary>
        /// The scale at which the time is passing for the clock. This can be used for slow motion, acceleration, pause or even rewind effects. 
        /// </summary>
        public float localTimeScale
        {
            get { return _localTimeScale; }
            set { _localTimeScale = value; }
        }
        /// <summary>
        /// The computed time scale of the clock. This value takes into consideration all of the clock's parameters (parent, paused, etc.) and multiplies their effect accordingly. 
        /// </summary>
        public float timeScale
        {
            get
            {
                if (paused)
                {
                    return 0;
                }
                else
                {
                    return localTimeScale;
                }
            }
        }


        [SerializeField]
        private bool _paused;

        /// <summary>
        /// Determines whether the clock is paused. This toggle is especially useful if you want to pause a clock without having to worry about storing its previous time scale to restore it afterwards. 
        /// </summary>
        public bool paused
        {
            get { return _paused; }
            set { _paused = value; }
        }

        [SerializeField]
        private ClockBlend _parentBlend = ClockBlend.Multiplicative;

        /// <summary>
        /// Determines how the clock combines its time scale with that of its parent.
        /// </summary>
        public ClockBlend parentBlend
        {
            get { return _parentBlend; }
            set { _parentBlend = value; }
        }

        #endregion
        
    }
}
