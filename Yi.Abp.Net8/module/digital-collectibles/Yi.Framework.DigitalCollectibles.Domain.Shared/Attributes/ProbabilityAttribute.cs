using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;

namespace Yi.Framework.DigitalCollectibles.Domain.Shared.Attributes;


public class ProbabilityAttribute : Attribute
{
    /// <summary>
    /// 概率
    /// </summary>
    public double Probability { get; set; }

    /// <summary>
    /// 默认价值
    /// </summary>
    public double DefaultValue { get; set; }

    public ProbabilityAttribute(double probability,double defaultValue)
    {
        this.Probability = probability;
        this.DefaultValue = defaultValue;
    }
}

