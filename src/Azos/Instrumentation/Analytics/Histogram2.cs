/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System.Collections.Generic;

namespace Azos.Instrumentation.Analytics
{
  /// <summary>
  /// Two-dimensional histogram for storing number of samples for given
  /// dimension keys
  /// </summary>
  public class Histogram<TData1, TData2> : Histogram
  {
    /// <summary>
    /// Constructs a histogram from a given array of dimensions
    /// </summary>
    /// <param name="title">Histogram title used for displaying result</param>
    /// <param name="dimension1">Dimension of the 1st histogram dimension</param>
    /// <param name="dimension2">Dimension of the 2nd histogram dimension</param>
    public Histogram(string title,
        Dimension<TData1> dimension1,
        Dimension<TData2> dimension2)
        : base(title, 1, dimension1.PartitionCount + dimension2.PartitionCount)
    {
      m_Dimension1 = dimension1;
      m_Dimension2 = dimension2;
      m_Dimension1.SetIndex(0);
      m_Dimension2.SetIndex(1);
    }

    #region Public

    /// <summary>
    /// Number of dimensions in this histogram
    /// </summary>
    public override int DimensionCount => 2;

    /// <summary>
    /// Return the sample count associated with given histogram keys
    /// </summary>
    public int this[int k1, int k2] => this[new HistogramKeys(k1, k2)];

    /// <summary>
    /// Try to get the sample count associated with the given histogram keys.
    /// If the keys are not present in the histogram dictionary return false
    /// </summary>
    public bool TryGet(int k1, int k2, out int count)
    => TryGet(new HistogramKeys(k1, k2), out count);

    /// <summary>
    /// Increment histogram statistics for a given pair of dimension values
    /// </summary>
    public virtual void Sample(TData1 value1, TData2 value2)
      => DoSample(Keys(value1, value2));

    /// <summary>
    /// Convert values to HistogramKeys struct
    /// </summary>
    public HistogramKeys Keys(TData1 value1, TData2 value2)
    => new HistogramKeys(m_Dimension1[value1], m_Dimension2[value2]);

    /// <summary>
    /// Returns number of samples collected for a given key.
    /// The key is obtained by mapping the given values into the dimensions' partitions.
    /// Return value of 0 indicates that key is not present in the histogram
    /// </summary>
    public int Value(TData1 value1, TData2 value2)
    {
      var keys = Keys(value1, value2);
      return TryGet(keys, out int count) ? count : 0;
    }

    public override IEnumerable<Dimension> Dimensions
    {
      get
      {
        yield return m_Dimension1;
        yield return m_Dimension2;
      }
    }

    #endregion

    #region Fields

    protected Dimension<TData1> m_Dimension1;
    protected Dimension<TData2> m_Dimension2;

    #endregion

  }
}
