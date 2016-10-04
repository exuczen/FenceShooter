using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CT {
	/// <summary>Irwin–Hall distribution, an approximation of normal (Gaussian) distribution. https://en.wikipedia.org/wiki/Irwin%E2%80%93Hall_distribution
	/// <para>This class encapsulates parameters of generalized scaled Irwin–Hall distribution. Instead of approximating true normal distribution by taking
	/// 12*U(0,1)-6, we take sum of N values and scale the result to fit [mean-scatter,mean+scatter] range, where N, "mean" and "scatter" are user-defined.
	/// The resulting distribution is narrowed into desired range, and thus the standard deviation is 1/6*scatter instead of 1*scatter.</para>
	/// <para>https://en.wikipedia.org/wiki/Normal_distribution "An easy to program approximate approach, that relies on the central limit theorem,
	/// is as follows: generate 12 uniform U(0,1) deviates, add them all up, and subtract 6 – the resulting random variable will have approximately standard normal distribution.
	/// In truth, the distribution will be Irwin–Hall, which is a 12-section eleventh-order polynomial approximation to the normal distribution.
	/// This random deviate will have a limited range of (−6, 6) cit. Johnson, Norman L.; Kotz, Samuel; Balakrishnan, Narayanaswamy (1995). Continuous Univariate Distributions, Volume 2. Wiley. ISBN 0-471-58494-0."</para>
	/// <para>About 68% of values drawn from a normal distribution are within one standard deviation σ away from the mean
	/// about 95% of the values lie within two standard deviations; and about 99.7% are within three standard deviations.
	/// This fact is known as the 68-95-99.7 (empirical) rule, or the 3-sigma rule.http://en.wikipedia.org/wiki/Normal_distribution</para>
	/// </summary>
	[System.Serializable]
	public class IrwinHallDistribution {
		[Tooltip("The more samples, the smoother distribution. One sample is Uniform. Six gives pretty nice Gaussian Distribution.")]
		public int samples;
		[Tooltip("The values will be contained in [-1,1] range if mean is 0 and scatter is 1, generally [mean-scatter,mean+scatter]")]
		public float mean;
		[Tooltip("The standard deviation is scatter/6")]
		public float scatter;
		public IrwinHallDistribution(int samples, float mean, float scatter) {
			this.samples = samples;
			this.mean = mean;
			this.scatter = scatter;
		}
		public float value {
			get {
				if (samples == 0)
					return 0f;
				float result = 0f;
				for (int i = 0; i < samples; i++)
					result += Random.value;
				var samplesHalf = samples * 0.5f;
				result -= samplesHalf; // center on 0
				result /= samplesHalf; // normalize to -1,1
				return result * scatter + mean; // scale it and position to be in [mean-scatter,mean+scatter] range
			}
		}
	}
}
