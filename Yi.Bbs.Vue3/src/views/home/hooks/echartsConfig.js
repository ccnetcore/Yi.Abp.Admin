export const statisticsEcharts = {
  grid: {
    top: "10%",
    left: "4%",
    right: "4%",
    bottom: "5%",
    containLabel: true,
  },
  tooltip: {
    trigger: "axis",
  },
  xAxis: {
    show: true,
    type: "category",
    data: [],
    axisLine: {
      lineStyle: {
        color: "#999",
      },
    },
  },
  yAxis: [
    {
      type: "value",
      splitNumber: 4,
      splitLine: {
        lineStyle: {
          type: "dashed",
          color: "#DDD",
        },
      },
      axisLine: {
        show: false,
        lineStyle: {
          color: "#333",
        },
      },
      nameTextStyle: {
        color: "#999",
      },
      splitArea: {
        show: false,
      },
    },
  ],
  series: {
    name: "访问量",
    type: "line",
    data: [],
    lineStyle: {
      normal: {
        width: 5,
        color: {
          type: "linear",
          colorStops: [
            {
              offset: 0,
              color: "#a0cfff", // 浅蓝色，0% 处的颜色
            },
            {
              offset: 1,
              color: "#0047AB", // 深蓝色，100% 处的颜色
            },
          ],
          globalCoord: false, // 缺省为 false
        },
        shadowColor: "rgba(72,216,191, 0.3)",
        shadowBlur: 10,
        shadowOffsetY: 20,
      },
    },
    itemStyle: {
      normal: {
        color: "#fff",
        borderWidth: 10,
        borderColor: "#A9F387",
      },
    },
    smooth: true,
  },
};
