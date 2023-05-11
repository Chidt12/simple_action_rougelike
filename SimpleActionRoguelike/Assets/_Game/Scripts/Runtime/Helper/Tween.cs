using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Helper
{
    public class TweenHelper : MonoBehaviour
    {
        /// <summary>
        /// A list of all the possible curves you can tween a value along
        /// </summary>
        public enum TweenCurve
        {
            LinearTween,
            EaseInQuadratic, EaseOutQuadratic, EaseInOutQuadratic,
            EaseInCubic, EaseOutCubic, EaseInOutCubic,
            EaseInQuartic, EaseOutQuartic, EaseInOutQuartic,
            EaseInQuintic, EaseOutQuintic, EaseInOutQuintic,
            EaseInSinusoidal, EaseOutSinusoidal, EaseInOutSinusoidal,
            EaseInBounce, EaseOutBounce, EaseInOutBounce,
            EaseInOverhead, EaseOutOverhead, EaseInOutOverhead,
            EaseInExponential, EaseOutExponential, EaseInOutExponential,
            EaseInElastic, EaseOutElastic, EaseInOutElastic,
            EaseInCircular, EaseOutCircular, EaseInOutCircular,
            AntiLinearTween, AlmostIdentity
        }

        // Core methods ---------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Moves a value between a startValue and an endValue based on a currentTime, along the specified tween curve
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="initialTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, TweenCurve curve)
        {
            currentTime = MathHelper.Remap(currentTime, initialTime, endTime, 0f, 1f);
            switch (curve)
            {
                case TweenCurve.LinearTween: currentTime = TweenDefinitions.Linear_Tween(currentTime); break;
                case TweenCurve.AntiLinearTween: currentTime = TweenDefinitions.LinearAnti_Tween(currentTime); break;

                case TweenCurve.EaseInQuadratic: currentTime = TweenDefinitions.EaseIn_Quadratic(currentTime); break;
                case TweenCurve.EaseOutQuadratic: currentTime = TweenDefinitions.EaseOut_Quadratic(currentTime); break;
                case TweenCurve.EaseInOutQuadratic: currentTime = TweenDefinitions.EaseInOut_Quadratic(currentTime); break;

                case TweenCurve.EaseInCubic: currentTime = TweenDefinitions.EaseIn_Cubic(currentTime); break;
                case TweenCurve.EaseOutCubic: currentTime = TweenDefinitions.EaseOut_Cubic(currentTime); break;
                case TweenCurve.EaseInOutCubic: currentTime = TweenDefinitions.EaseInOut_Cubic(currentTime); break;

                case TweenCurve.EaseInQuartic: currentTime = TweenDefinitions.EaseIn_Quartic(currentTime); break;
                case TweenCurve.EaseOutQuartic: currentTime = TweenDefinitions.EaseOut_Quartic(currentTime); break;
                case TweenCurve.EaseInOutQuartic: currentTime = TweenDefinitions.EaseInOut_Quartic(currentTime); break;

                case TweenCurve.EaseInQuintic: currentTime = TweenDefinitions.EaseIn_Quintic(currentTime); break;
                case TweenCurve.EaseOutQuintic: currentTime = TweenDefinitions.EaseOut_Quintic(currentTime); break;
                case TweenCurve.EaseInOutQuintic: currentTime = TweenDefinitions.EaseInOut_Quintic(currentTime); break;

                case TweenCurve.EaseInSinusoidal: currentTime = TweenDefinitions.EaseIn_Sinusoidal(currentTime); break;
                case TweenCurve.EaseOutSinusoidal: currentTime = TweenDefinitions.EaseOut_Sinusoidal(currentTime); break;
                case TweenCurve.EaseInOutSinusoidal: currentTime = TweenDefinitions.EaseInOut_Sinusoidal(currentTime); break;

                case TweenCurve.EaseInBounce: currentTime = TweenDefinitions.EaseIn_Bounce(currentTime); break;
                case TweenCurve.EaseOutBounce: currentTime = TweenDefinitions.EaseOut_Bounce(currentTime); break;
                case TweenCurve.EaseInOutBounce: currentTime = TweenDefinitions.EaseInOut_Bounce(currentTime); break;

                case TweenCurve.EaseInOverhead: currentTime = TweenDefinitions.EaseIn_Overhead(currentTime); break;
                case TweenCurve.EaseOutOverhead: currentTime = TweenDefinitions.EaseOut_Overhead(currentTime); break;
                case TweenCurve.EaseInOutOverhead: currentTime = TweenDefinitions.EaseInOut_Overhead(currentTime); break;

                case TweenCurve.EaseInExponential: currentTime = TweenDefinitions.EaseIn_Exponential(currentTime); break;
                case TweenCurve.EaseOutExponential: currentTime = TweenDefinitions.EaseOut_Exponential(currentTime); break;
                case TweenCurve.EaseInOutExponential: currentTime = TweenDefinitions.EaseInOut_Exponential(currentTime); break;

                case TweenCurve.EaseInElastic: currentTime = TweenDefinitions.EaseIn_Elastic(currentTime); break;
                case TweenCurve.EaseOutElastic: currentTime = TweenDefinitions.EaseOut_Elastic(currentTime); break;
                case TweenCurve.EaseInOutElastic: currentTime = TweenDefinitions.EaseInOut_Elastic(currentTime); break;

                case TweenCurve.EaseInCircular: currentTime = TweenDefinitions.EaseIn_Circular(currentTime); break;
                case TweenCurve.EaseOutCircular: currentTime = TweenDefinitions.EaseOut_Circular(currentTime); break;
                case TweenCurve.EaseInOutCircular: currentTime = TweenDefinitions.EaseInOut_Circular(currentTime); break;

                case TweenCurve.AlmostIdentity: currentTime = TweenDefinitions.AlmostIdentity(currentTime); break;

            }
            return startValue + currentTime * (endValue - startValue);
        }

        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, TweenCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, TweenCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, TweenCurve curve)
        {
            float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Animation curve methods --------------------------------------------------------------------------------------------------------------

        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, AnimationCurve curve)
        {
            currentTime = MathHelper.Remap(currentTime, initialTime, endTime, 0f, 1f);
            currentTime = curve.Evaluate(currentTime);
            return startValue + currentTime * (endValue - startValue);
        }

        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, AnimationCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, AnimationCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, AnimationCurve curve)
        {
            float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Tween type methods ------------------------------------------------------------------------------------------------------------------------

        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, TweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return 0f;
        }
        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, TweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector2.zero;
        }
        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, TweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector3.zero;
        }
        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, TweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Quaternion.identity;
        }

        // MOVE METHODS ---------------------------------------------------------------------------------------------------------
        public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Vector3 origin, Vector3 destination,
            WaitForSeconds delay, float delayDuration, float duration, TweenHelper.TweenCurve curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        public static Coroutine MoveRectTransform(MonoBehaviour mono, RectTransform targetTransform, Vector3 origin, Vector3 destination,
            WaitForSeconds delay, float delayDuration, float duration, TweenHelper.TweenCurve curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveRectTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration,
            TweenHelper.TweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, updatePosition, updateRotation, ignoreTimescale));
        }

        public static Coroutine RotateTransformAround(MonoBehaviour mono, Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration,
            float duration, TweenHelper.TweenCurve curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(RotateTransformAroundCo(targetTransform, center, destination, angle, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        protected static IEnumerator MoveRectTransformCo(RectTransform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
            float delayDuration, float duration, TweenHelper.TweenCurve curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                targetTransform.localPosition = TweenHelper.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.localPosition = destination;
        }

        protected static IEnumerator MoveTransformCo(Transform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
            float delayDuration, float duration, TweenHelper.TweenCurve curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                targetTransform.transform.position = TweenHelper.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.transform.position = destination;
        }

        protected static IEnumerator MoveTransformCo(Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration,
            TweenHelper.TweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                if (updatePosition)
                {
                    targetTransform.transform.position = TweenHelper.Tween(duration - timeLeft, 0f, duration, origin.position, destination.position, curve);
                }
                if (updateRotation)
                {
                    targetTransform.transform.rotation = TweenHelper.Tween(duration - timeLeft, 0f, duration, origin.rotation, destination.rotation, curve);
                }
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            if (updatePosition) { targetTransform.transform.position = destination.position; }
            if (updateRotation) { targetTransform.transform.localEulerAngles = destination.localEulerAngles; }
        }

        protected static IEnumerator RotateTransformAroundCo(Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration,
            TweenHelper.TweenCurve curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }

            Vector3 initialRotationPosition = targetTransform.transform.position;
            Quaternion initialRotationRotation = targetTransform.transform.rotation;

            float rate = 1f / duration;

            float timeSpent = 0f;
            while (timeSpent < duration)
            {

                float newAngle = TweenHelper.Tween(timeSpent, 0f, duration, 0f, angle, curve);

                targetTransform.transform.position = initialRotationPosition;
                initialRotationRotation = targetTransform.transform.rotation;
                targetTransform.RotateAround(center.transform.position, center.transform.up, newAngle);
                targetTransform.transform.rotation = initialRotationRotation;

                timeSpent += ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.transform.position = destination.position;
        }
    }

}
