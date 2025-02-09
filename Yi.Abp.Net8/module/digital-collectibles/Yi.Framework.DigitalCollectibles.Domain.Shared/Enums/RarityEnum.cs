using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Attributes;

namespace Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;

/// <summary>
/// 稀有度枚举
/// </summary>
public enum RarityEnum
{
    [Display(Name = "普通")][Probability(0.6f,100f)] Ordinary = 0,
    [Display(Name = "高级")][Probability(0.25f,200f)] Senior = 1,
    [Display(Name = "稀有")][Probability(0.1f,300f)] Rare = 2,
    [Display(Name = "珍品")][Probability(0.04f,400f)] Gem = 3,
    [Display(Name = "传说")][Probability(0.01f,600f)] Legend = 4
}

public static class RarityEnumExtensions
{
    public static string GetRarityName(this RarityEnum enumValue)
    {
            // 获取枚举类型
            Type type = enumValue.GetType();
        
            // 获取当前枚举值的 FieldInfo
            FieldInfo fieldInfo = type.GetField(enumValue.ToString());

            // 获取 Display 特性
            DisplayAttribute displayAttribute = fieldInfo
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            // 返回名称，如果没有找到，则返回枚举值的名称
            return displayAttribute?.Name ?? enumValue.ToString();
    }

    private static T GetAttribute<T>(RarityEnum rarity) where T : Attribute
    {
        var fieldInfo = typeof(RarityEnum).GetField(rarity.ToString());
        var attribute = fieldInfo.GetCustomAttribute<T>();
        return attribute!;
    }

    public static decimal GetProbabilityValue(this RarityEnum rarity)
    {
        var attribute = GetAttribute<ProbabilityAttribute>(rarity);
        return attribute.Probability.To<decimal>();
    }

    public static double GetDefaultValue(this RarityEnum rarity)
    {
        var attribute = GetAttribute<ProbabilityAttribute>(rarity);
        return attribute.DefaultValue.To<double>();
    }
    
    public static string GetDisplayValue(this RarityEnum rarity)
    {
        var display = GetAttribute<DisplayAttribute>(rarity);
        return display.Name!;
    }
    
    public static decimal[] GetProbabilityArray()
    {
        List<decimal> probabilityList = new List<decimal>();

        foreach (var field in typeof(RarityEnum).GetFields())
        {
            // 获取特性
            var attribute = field.GetCustomAttribute<ProbabilityAttribute>();
            if (attribute != null)
            {
                // 将特性值添加到列表
                probabilityList.Add(attribute.Probability.To<decimal>());
            }
        }

        return probabilityList.ToArray();
    }
    
}