using UnityEngine;
using System;

namespace Utility {
	public enum TransitionType {
		LINEAR,
		PARABOLIC_ACC,
		PARABOLIC_DEC,
		HYPERBOLIC_ACC,
		HYPERBOLIC_DEC,
		SIN_IN_PI2_RANGE,
		SIN_IN_PI_RANGE,
		SIN_IN_2PI_RANGE,
		COS_IN_PI2_RANGE,
		COS_IN_PI_RANGE,
		COS_IN_2PI_RANGE,
		ASYMETRIC_NORMALISED,
		ASYMETRIC_INFLECTED,
		SYMMETRIC_INFLECTED,
	};

	public static class Maths {
		public const float M_2PI = 2 * Mathf.PI;
		public const float M_PI2 = Mathf.PI / 2;
		public const float M_PI = Mathf.PI;

		public static float PowF(float a, int n) {
			float z = 1.0f;

			for (int i = 0; i < Math.Abs(n); i++) {
				z *= a;
			}
			if (n < 0) z = 1 / z;
			return z;
		}


		public static int PowI(int a, int n) {
			int z = 1;
			for (int i = 0; i < Math.Abs(n); i++) {
				z *= a;
			}
			return z;
		}



		public static float GetTransitionAsymPolynom32(float timePassed, float duration, int n, int m) {
			float x = timePassed / duration;
			return -2 * PowF(x, n) + 3 * PowF(x, m);
		}

		public static float GetTransitionAsymNormalised(float timePassed, float duration, float inflectionPointNormalized, int n, int m) {
			float x0 = inflectionPointNormalized * duration;
			float denomPart = m * x0 - n * (x0 - duration);
			float denom, shift;

			//    float a = m/(PowF(x0, n-1)*denomPart);
			//    float b = n/(PowF(x0-duration, m-1)*denomPart);
			//    printf("n=%i m=%i x0=%f duration=%f a=%.10f b=%.10f\n",n,m,x0,duration,a,b);

			if (timePassed < x0)
			{
				denom = PowF(x0, n - 1) * denomPart;
				shift = m * PowF(timePassed, n) / denom;
			}
			else
			{
				denom = PowF(x0 - duration, m - 1) * denomPart;
				shift = n * PowF(timePassed - duration, m) / denom + 1;
			}
			return shift;
		}

		public static float GetTransitionSymmInflected(float timePassed, float duration, float inflPtNorm, int n, int m) {
			float halfDuration = duration * 0.5f;
			float shift;
			if (timePassed < halfDuration)
			{
				shift = GetTransitionAsymNormalised(timePassed, halfDuration, inflPtNorm, n, m);
			}
			else
			{
				shift = (1.0f - GetTransitionAsymNormalised(timePassed - halfDuration, halfDuration, 1.0f - inflPtNorm, m, n));
			}
			return shift;
		}

		public static float GetTransitionAsymInflected(float timePassed, float duration,
													   int N, int M, int n, int m, float xMax, float yMax,
													   float inflPtNorm0, float inflPtNorm1) {
			//			N=2;
			//			M=2;
			//			n=2;
			//			m=2;
			//			xMax = 0.6f;
			//			yMax = 1.4f;
			//			inflPtNorm0 = inflPtNorm1 = 0.5f;
			float shift;
			float t0 = duration * xMax;
			if (timePassed < t0) {
				shift = yMax * GetTransitionAsymNormalised(timePassed, t0, inflPtNorm0, N, M);
			} else {
				shift = 1.0f + (yMax - 1.0f) * (1.0f - GetTransitionAsymNormalised(timePassed - t0, duration * (1.0f - xMax), inflPtNorm1, n, m));
			}
			return shift;
		}



		public static float GetTransition(TransitionType transitionType, float timePassed, float duration, int power) {
			float shift;
			switch (transitionType) {
				case TransitionType.LINEAR:
				shift = timePassed / duration;
				break;
				case TransitionType.PARABOLIC_ACC:
				shift = PowF(Mathf.Abs(timePassed / duration), power);
				break;
				case TransitionType.PARABOLIC_DEC:
				shift = -PowF(Mathf.Abs(timePassed - duration) / duration, power) + 1;
				break;
				case TransitionType.HYPERBOLIC_ACC:
				shift = -1 / (0.5f * PowF(Mathf.Abs(timePassed / duration), power) - 1) - 1;
				break;
				case TransitionType.HYPERBOLIC_DEC:
				shift = 1 / (0.5f * PowF(Mathf.Abs(timePassed / duration - 1), power) - 1) + 2;
				break;
				case TransitionType.SIN_IN_PI2_RANGE: {

					float omegaT = timePassed * M_PI2 / duration;
					float sinOmegaT = Mathf.Sin(omegaT);
					shift = sinOmegaT;
				}
				break;
				case TransitionType.SIN_IN_PI_RANGE: {
					float omegaT = timePassed * M_PI / duration;
					float sinOmegaT = Mathf.Sin(omegaT);
					shift = sinOmegaT;
				}
				break;
				case TransitionType.SIN_IN_2PI_RANGE: {
					float omegaT = timePassed * 2 * M_PI / duration;
					float sinOmegaT = Mathf.Sin(omegaT);
					shift = sinOmegaT;
				}
				break;
				case TransitionType.COS_IN_PI2_RANGE: {
					float omegaT = timePassed * M_PI2 / duration;
					float cosOmegaT = Mathf.Cos(omegaT);
					shift = -cosOmegaT + 1;
				}
				break;

				case TransitionType.COS_IN_PI_RANGE: {
					float omegaT = timePassed * M_PI / duration;
					float cosOmegaT = Mathf.Cos(omegaT);
					shift = (-cosOmegaT + 1.0f) / 2;
				}
				break;
				case TransitionType.COS_IN_2PI_RANGE: {
					//            float omegaT=timePassed*M_PI/duration;
					//            float cosOmegaT=cosf(omegaT);
					//            // cos(2x)=2*cos(x)^2-1
					//            shift = (-PowF(cosOmegaT,2)+1.f);
					float omegaT = timePassed * 2 * M_PI / duration;
					float cosOmegaT = Mathf.Cos(omegaT);
					shift = (-cosOmegaT + 1.0f) / 2;
				}
				break;
				default:
				shift = 0;
				break;
			}

			return shift;
		}

	}
}

