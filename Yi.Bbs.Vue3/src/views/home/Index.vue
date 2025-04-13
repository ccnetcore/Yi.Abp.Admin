<template>
  <div class="home-box">
    <el-row :gutter="20" class="top-div">
      <el-col :span="17">
        <div class="chat-hub">
          <!--          <p @click="onClickToChatHub">点击前往-最新上线<span>《聊天室》 </span>，现已支持<span>Ai助手</span>，希望能帮助大家</p>-->

          <p v-if="isIcp"
             style="font-size: 25px;
    color: blue;
    background: red;
    /* height: 80px; */
    font-weight: 900;
    text-align: center;
margin: 10px auto;">
            本站点为个人内容分享，全部资料免费开源学习，所有数据为假数据
            <br/>
            不涉及企业、团体、论坛和经营销售等内容，只做简单的成果展示
            <br/>
            富强、‌民主、文明、‌和谐、‌自由、‌平等
            <br/>
            公正、‌法治、‌爱国、‌敬业、‌诚信、友善
          </p>
          <p v-else @click="onClickToWeChat">
            点击关注官方<span>意.Net微信小程序</span>与<span>意.Net公众号</span>
          </p>


        </div>
        <div class="scrollbar">
          <ScrollbarInfo/>
        </div>

        <el-row class="left-div">


          <el-col :span="8" v-for="i in plateList" :key="i.id" class="plate" :style="{
      'padding-left': i % 3 == 1 ? 0 : 0.2 + 'rem',
      'padding-right': i % 3 == 0 ? 0 : 0.2 + 'rem',
    }">
            <img v-if="isIcp" src="@/assets/login.png" style="height: 80px;width: 100%" alt=""/>
            <PlateCard v-else :name="i.name" :introduction="i.introduction" :id="i.id"
                       :isPublish="i.isDisableCreateDiscuss"/>
          </el-col>

          <div ref="scrollableDiv" class="scrollable-div" v-infinite-scroll="loadDiscuss" style="height: 2500px;width: 100%; overflow-y: auto;" infinite-scroll-distance="10">

            <el-col v-if="isDiscussFinished" :span="24" v-for="i in discussList" :key="i.id">
              <img v-if="isIcp" src="@/assets/login.png" style="height: 150px;width: 100%" alt=""/>
              <DisscussCard v-else :discuss="i" badge="置顶"/>
            </el-col>
            <Skeleton v-else :isBorder="true"/>
       

            <el-col :span="24" v-for="i in allDiscussList" :key="i.id">
              <img v-if="isIcp" src="@/assets/login.png" style="height: 150px;width: 100%" alt=""/>
  
              <DisscussCard v-else :discuss="i"/>
            </el-col>

            <Skeleton v-if="!isAllDiscussFinished"  :isBorder="true"/>
          </div>

 
        </el-row>
      </el-col>
      <el-col :span="7">
        <el-row class="right-div">
          <el-col :span="24">
            <el-carousel trigger="click" height="150px">
              <el-carousel-item v-for="item in bannerList" :key="item.id">
                <div class="carousel-font" :style="{ color: item.color }">
                  {{ item.name }}
                </div>
                <el-image style="width: 100%; height: 100%" :src="item.logo" fit="cover"/>
              </el-carousel-item>
            </el-carousel>
          </el-col>
          <div class="analyse">
            <div class="item">
              <div class="text">在线人数</div>
              <div class="content">
                <div class="name"></div>
                <div class="content-box top">
                  <div class="count">{{ onlineNumber }}</div>
                </div>
              </div>
            </div>
            <div class="item">
              <div class="text">注册人数</div>
              <div class="content">
                <div class="content-box top">
                  <div class="count">{{ userAnalyseInfo.registerNumber }}</div>
                </div>
              </div>
            </div>
            <div class="item">
              <div class="text">昨日新增</div>
              <div class="content">
                <div class="content-box">
                  <div class="count">
                    {{ userAnalyseInfo.yesterdayNewUser }}
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- 签到 -->
          <el-col v-if="!isIcp" :span="24">
            <InfoCard header="活动">
              <template #content>
                <div class="top">与AI同行，创造无限可能</div>
                <el-row class="active">

                  <el-col style="padding: 5px 0px;box-shadow:none" v-for="item in activeList" :span="6" @click="handleToRouter(item.path)">

                    <el-icon color="#70aafb" size="30px">
                      <component :is="item.icon"></component>
                    </el-icon>
                    <span> {{ item.name }}</span>
                  </el-col>
                </el-row>
              </template>
            </InfoCard>
          </el-col>

          <el-col :span="24">
            <InfoCard header="访问统计" class="VisitsLineChart" text="全站历史统计" @onClickText="onClickAccessLog">
              <template #content>
                <p class="switch-span" @click="onClickWeekSwitch">切换</p>
                <VisitsLineChart :option="statisOptions" class="statisChart"/>

              </template>
            </InfoCard>

          
          </el-col>
          <el-dialog v-model="accessLogDialogVisible" title="全站历史统计" width="1200px" center>
              <el-tabs v-model="accessLogTab">
                <el-tab-pane label="访问统计（近3月）" name="AccessLogChart"
                             style="display: flex;justify-content: center;">
                  <AccessLogChart :option="accessLogOptins" style="height: 600px;width: 1200px;"/>
                </el-tab-pane>
                <el-tab-pane label="注册统计（近3月）" name="RegisterChart"
                             style="display: flex;justify-content: center;">
                  <AccessLogChart :option="registerLogOptins" style="height: 600px;width: 1200px;"/>
                </el-tab-pane>

              </el-tabs>


            </el-dialog>
          <el-col :span="24">
            <InfoCard header="简介" text="">
              <template #content>
                <div class="introduce">
                  没有什么能够阻挡，人类对代码<span style="color: #1890ff">优雅</span>的追求
                </div>
              </template>
            </InfoCard>
          </el-col>

          <el-col v-if="!isIcp" :span="24">
            <template v-if="isPointFinished">
              <InfoCard :isPadding="false" :items="pointList" header="财富排行榜" text="查看我的位置" height="410"
                        style="padding:0 20px"
                        @onClickText="onClickMoneyTop">
                <template #item="temp">
                  <PointsRanking :pointsData="temp"/>
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard :isPadding="false" header="财富排行榜" text="查看我的位置">
                <template #content>
                  <Skeleton/>
                </template>
              </InfoCard>
            </template>
          </el-col>

          <el-col v-if="!isIcp" :span="24">
            <template v-if="isFriendFinished">
              <InfoCard :isPadding="false" :items="friendList" header="推荐好友" text="更多" height="400"
                        style="padding:0 20px">
                <template #item="temp">
                  <RecommendFriend :friendData="temp"/>
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard :isPadding="false" header="推荐好友" text="更多">
                <template #content>
                  <Skeleton/>
                </template>
              </InfoCard>
            </template>
          </el-col>
          <el-col v-if="!isIcp" :span="24">
            <template v-if="isThemeFinished">
              <InfoCard :isPadding="false" :items="themeList" header="推荐主题" text="更多" height="400"
                        style="padding:0 20px"
              >
                <template #item="temp">
                  <ThemeData :themeData="temp"/>
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard :isPadding="false" header="推荐主题" text="更多">
                <template #content>
                  <Skeleton/>
                </template>
              </InfoCard>
            </template>
          </el-col>

          <el-col :span="24" style="background-color: #ffffff;">
            <BottomInfo/>
          </el-col>
        </el-row>
      </el-col>
    </el-row>


    <el-dialog
        v-model="wechatDialogVisible"
        title="意社区官方"
        width="800"
    >
  
      <div style="display: flex;flex-direction: column;align-items: center;">
        <p style="margin: 10px;font-size: large">微信小程序：</p>
      <img style="width: 200px; height: 200px" src="@/assets/wechat/mini.jpg" alt=""/>
    <el-divider/>
        <p style="margin: 10px;font-size: large"> 微信公众号：</p>
        <img style="width: 585px; height: 186px" src="@/assets/wechat/share.png" alt=""/>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="wechatDialogVisible = false">
            已关注
          </el-button>
        </div>
      </template>
    </el-dialog>
    <el-backtop :right="100" :bottom="100" @click="clickBacktop" visibility-height="0"/>
  </div>
</template>

<script setup>
import {onMounted, ref, reactive, computed, nextTick, watch} from "vue";
import {useRouter} from "vue-router";
import DisscussCard from "@/components/DisscussCard.vue";
import InfoCard from "@/components/InfoCard.vue";
import ThemeData from "@/views/home/components/RecommendTheme/index.vue";
import PlateCard from "@/components/PlateCard.vue";
import ScrollbarInfo from "@/components/ScrollbarInfo.vue";
import BottomInfo from "@/components/BottomInfo.vue";
import VisitsLineChart from "./components/VisitsLineChart/index.vue";
import AccessLogChart from "./components/AccessLogChart/Index.vue"
import {access, getAccessList} from "@/apis/accessApi.js";
import {getList} from "@/apis/plateApi.js";
import {getList as bannerGetList} from "@/apis/bannerApi.js";
import {getHomeDiscuss} from "@/apis/discussApi.js";
import {getWeek} from "@/apis/accessApi.js";
import {
  getRecommendedTopic,
  getRecommendedFriend,
  getMoneyTop,
  getUserAnalyse,
  getRegisterAnalyse
} from "@/apis/analyseApi.js";
import {getList as getAllDiscussList} from "@/apis/discussApi.js";
import PointsRanking from "./components/PointsRanking/index.vue";
import RecommendFriend from "./components/RecommendFriend/index.vue";
import Skeleton from "@/components/Skeleton/index.vue";
import useSocketStore from "@/stores/socket";

const accessLogDialogVisible = ref(false)
const router = useRouter();

const accessAllList = ref([]);
const registerAllList = ref([]);

const plateList = ref([]);
const discussList = ref([]);
const isDiscussFinished = ref(false);
const bannerList = ref([]);
const weekList = ref([]);
const pointList = ref([]);
const isPointFinished = ref(false);
const friendList = ref([]);
const isFriendFinished = ref(false);
const themeList = ref([]);
const isThemeFinished = ref(false);
const allDiscussList = ref([]);
const isAllDiscussFinished = ref(false);
const userAnalyseInfo = ref({});
const onlineNumber = ref(0);
const accessLogTab = ref()

const currentDiscussPageIndex = ref(1);
const activeList = [
  {name: "签到", path: "/activity/sign", icon: "Present"},
  {name: "等级", path: "/activity/level", icon: "Ticket"},
  {name: "大转盘", path: "/activity/lucky", icon: "Sunny"},
  {name: "银行", path: "/activity/bank", icon: "CreditCard"},

  {name: "任务", path: "/activity/assignment", icon: "Memo"},
  {name: "排行榜", path: "/money", icon: "Money"},
  {name: "开始", path: "/start", icon: "Position"},
  {name: "聊天室", path: "/chat", icon: "ChatRound"},

  {name: "商城", path: "/shop", icon: "ShoppingCart"},
  {name: "数字藏品", path: "/dc", icon: "Trophy"},
  {name: "面试宝典", path: "/book", icon: "Memo"},
  {name: "AI炒股", path: "/stock", icon: "TrendCharts"},
  // {name: "小程序", path: "/", icon: "Position"},
  // {name: "公众号", path: "/", icon: "ChatRound"},
];
const isIcp = import.meta.env.VITE_APP_ICP === "true";

const weekQuery = reactive({accessLogType: "Request"});

const init = async () => {

  //分阶段优化
  await Promise.all([
    (async () => {
      const {data: allDiscussData, config: allDiscussConfig} =
          await getAllDiscussList({Type: 0, skipCount: currentDiscussPageIndex.value, maxResultCount: 10});
      isAllDiscussFinished.value = allDiscussConfig.isFinish;
      allDiscussList.value = allDiscussData.items;
    })(),
    (async () => {
      const {data: plateData} = await getList();
      plateList.value = plateData.items;
    })(),
    (async () => {
      const {data: discussData, config: discussConfig} = await getHomeDiscuss();
      discussList.value = discussData;
      isDiscussFinished.value = discussConfig.isFinish;
    })(),
    (async () => {
      const {data: bannerData} = await bannerGetList();
      bannerList.value = bannerData.items;
    })(),
    (async () => {
      const {data: weekData} = await getWeek(weekQuery);
      weekList.value = weekData;
    })(),
    (async () => {
      const {data: pointData, config: pointConfig} = await getMoneyTop();
      pointList.value = pointData.items;
      isPointFinished.value = pointConfig.isFinish;
    })(),
    (async () => {
      const {data: userAnalyseInfoData} = await getUserAnalyse();
      onlineNumber.value = userAnalyseInfoData.onlineNumber;
      userAnalyseInfo.value = userAnalyseInfoData;
    })(),
  ]);


  //不重要的请求滞后
  const {data: friendData, config: friendConfig} = await getRecommendedFriend();
  friendList.value = friendData;
  isFriendFinished.value = friendConfig.isFinish;
  const {data: themeData, config: themeConfig} = await getRecommendedTopic();
  themeList.value = themeData;
  isThemeFinished.value = themeConfig.isFinish;
  await access();
}
//初始化
onMounted(async () => {
  await init();
});

const weekXAxis = ["周一", "周二", "周三", "周四", "周五", "周六", "周日"];
// 访问统计
const statisOptions = computed(() => {
  return {
    xAxis: {
      data: weekList.value.map((item, index) => {
        return weekXAxis.filter((v, vIndex) => {
          return vIndex === index;
        })[0];
      }),
    },
    series: {
      data: weekList.value.map((item) => item.number),
    },
  };
});
//历史全部访问统计
const accessLogOptins = computed(() => {
  return {
    xAxis: {
      data: accessAllList.value?.map((item, index) => {
        return item.creationTime.slice(0, 10);

      })
    },
    series: [
      {
        data: accessAllList.value?.map((item, index) => {
          return item.number;
        })
      }
    ]
  }
});

//历史注册人员全部访问统计
const registerLogOptins = computed(() => {
  return {
    xAxis: {
      data: registerAllList.value?.map((item, index) => {
        return item.time.slice(0, 10);

      })
    },
    series: [
      {
        data: registerAllList.value?.map((item, index) => {
          return item.number;
        })
      }
    ]
  }
});

const onClickMoneyTop = () => {
  router.push("/money");
};

const onClickToChatHub = () => {
  router.push("/chat");
};

const handleToRouter = (path) => {
  router.push(path);
};

// 推送的实时人数获取
const currentOnlineNum = computed(() => useSocketStore().getOnlineNum());
watch(
    () => currentOnlineNum.value,
    (val) => {
      onlineNumber.value = val;
    },
    {deep: true}
);
watch(
    () => accessLogTab.value,
    async (value) => {
      switch (value) {
        case "AccessLogChart":
          const {data} = await getAccessList(weekQuery);
          accessAllList.value = data;

          break;
        case "RegisterChart":
          const {data: registerUserListData} = await getRegisterAnalyse();
          registerAllList.value = registerUserListData;

          break;
      }
    }
)
const onClickAccessLog = async () => {
  accessLogDialogVisible.value = true;
  accessLogTab.value = "AccessLogChart";

}

let loadingDiscuss = false;
//加载滚动文章
const loadDiscuss = async () => {
  
  if (loadingDiscuss === false) {
    loadingDiscuss = true;


    currentDiscussPageIndex.value += 1;

    isAllDiscussFinished.value = false;
    const {data: allDiscussData, config: allDiscussConfig} =
        await getAllDiscussList({Type: 0, skipCount: currentDiscussPageIndex.value, maxResultCount: 10});
    isAllDiscussFinished.value = allDiscussConfig.isFinish;

    //在列表后新增
    allDiscussList.value.push(...allDiscussData.items);


    loadingDiscuss = false;
  }


}
const wechatDialogVisible = ref(false)
//切换统计开关
const onClickWeekSwitch = async () => {
  if (weekQuery.accessLogType === "HomeClick") {
    weekQuery.accessLogType = "Request";
  } else if (weekQuery.accessLogType === "Request") {
    weekQuery.accessLogType = "HomeClick";
  }

  const {data: weekData} = await getWeek(weekQuery);
  weekList.value = weekData;
}
const scrollableDiv = ref(null);
//回到顶部
const clickBacktop=()=>{
  if (scrollableDiv.value) {
    scrollableDiv.value.scrollTop = 0; // 设置滚动条到顶部
  }
}

//打开微信公众号弹窗
const onClickToWeChat = () => {
  wechatDialogVisible.value = true;
};
</script>
<style scoped lang="scss">
.home-box {
  width: 100%;  /* 改为100%使其更具响应性 */
  max-width: 1300px;  /* 保持最大宽度限制 */
  margin: 0 auto;  /* 居中显示 */
  
  .left-div .el-col,
  .right-div .el-col {
    background-color: #ffffff;
    margin-bottom: 1rem;
    border-radius: 8px;  /* 增加圆角 */
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.05);  /* 添加微妙阴影 */
    transition: all 0.3s ease;  /* 过渡效果 */
    
    &:hover {
      transform: translateY(-5px);  /* 悬停时微抬升 */
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);  /* 悬停时增强阴影 */
    }
  }
  
  /* 简介卡片样式特别处理 */
  .introduce {
    color: rgba(0, 0, 0, 0.65);  /* 更深的颜色提高对比度 */
    font-size: 14px;  /* 稍微增大字体 */
    line-height: 1.6;  /* 增加行高 */
    padding: 15px 5px;  /* 增加内边距 */
    letter-spacing: 0.5px;  /* 字间距 */
    
    span {
      color: #1890ff;
      font-weight: 600;  /* 加粗 */
      padding: 0 2px;  /* 增加内边距 */
    }
  }

  .plate {
    background: transparent !important;
  }

  .carousel-font {
    position: absolute;
    z-index: 1;
    top: 10%;
    left: 10%;
  }

  .top-div {
    padding-top: 0.5rem;
  }

  .scrollbar {
    display: block;
    margin-bottom: 0.5rem;
  }

  .analyse {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    height: 100px;
    margin-bottom: 10px;

    .item {
      width: 30%;
      height: 100%;
      position: relative;
      background: url("@/assets/box/online_bg.svg") no-repeat;
      background-color: #fff;
      background-position: 0 30px;
      background-size: 150% 100%;
      border: 1px solid #409eff;
      border-radius: 5px;
      color: #409eff;

      .content {
        width: 100%;
        height: 100%;
        display: flex;
        justify-content: center;

        &-box {
          width: 100%;
          display: flex;
          align-items: center;
          justify-content: flex-start;
          justify-content: center;
          margin-bottom: 10px;

          .name {
            font-size: 14px;
          }

          .count {
            font-size: 20px;
            font-weight: bold;
          }
        }
      }

      .text {
        width: 60px;
        position: absolute;
        top: -10px;
        left: 50%;
        transform: translateX(-50%);
        font-size: 12px;
        text-align: center;
        border: 1px solid #d9ecff;
        border-radius: 5px;
        color: #409eff;
        background-color: #ecf5ff;
      }
    }
  }

  .top {
    text-align: center;
    margin-bottom: 20px;
  }

  .active {
    display: flex;
    flex-wrap: wrap;
    justify-content: flex-start;
    /* gap: 10px; */
    /* padding: 15px; */
    /* background-color: #f9f9f9; */
    border-radius: 8px;
    
    .el-col {
      flex-direction: column;
      align-items: center;
      display: flex;
      cursor: pointer;
      padding: 15px 0;  /* 增加内边距 */
      border-radius: 6px;  /* 圆角 */
      transition: all 0.3s ease;
      
      .el-icon {
        font-size: 24px;  /* 增大图标 */
        margin-bottom: 8px;  /* 增加与文字间距 */
        color: #606266;  /* 初始颜色 */
      }
      
      &:hover {
        background-color: #ebf5ff;  /* 更柔和的悬停色 */
        color: #409eff;  /* 文字颜色变化 */
        
        .el-icon {
          color: #409eff;  /* 图标颜色跟随变化 */
          transform: scale(1.1);  /* 图标微放大 */
        }
      }
    }
  }


  .VisitsLineChart p {
    display: flex;
    justify-content: flex-end;
    color: #409eff;
    cursor: pointer;
    margin-top: 8px;
  }

  .statisChart {
    width: 100%;
    height: 300px;
  }

  .accessLogChart {
    width: 1100px;
    height: 500px;
  }
}


//走马灯，聊天室链接
.chat-hub {
  background-color: #E6A23C;
  color: #ffffff;
  margin-bottom: 10px;
  width: 100%;
  overflow: hidden;
  white-space: nowrap;
  box-sizing: border-box;

  span {
    color: red;
  }

  display: flex;
  align-content: center;
  flex-wrap: wrap;
  min-height: 30px;

  p {
    margin: 0 auto;
    cursor: pointer;
  }
}

@keyframes marquee {
  0% {
    transform: translateX(0);
  }

  100% {
    transform: translateX(-100%);
  }
}


/* 美化滚动条样式 */
.scrollable-div::-webkit-scrollbar {
  width: 6px;  /* 稍微加宽 */
}

.scrollable-div::-webkit-scrollbar-track {
  background: #f5f5f5; 
  border-radius: 10px;
}

.scrollable-div::-webkit-scrollbar-thumb {
  background: linear-gradient(to bottom, #e0e0e0, #bdbdbd);  /* 渐变色滚动条 */
  border-radius: 10px;
  border: 2px solid transparent;
  background-clip: content-box;
}

.scrollable-div::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(to bottom, #bdbdbd, #9e9e9e);  /* 悬停时颜色变深 */
}

/* 优化切换按钮 */
.switch-span {
  display: inline-block;
  padding: 5px 12px;
  background-color: #ecf5ff;
  color: #409eff;
  border-radius: 20px;  /* 更圆润的形状 */
  font-size: 13px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 6px rgba(64, 158, 255, 0.15);  /* 添加微妙阴影 */
  position: relative;
  overflow: hidden;
  
  &:hover {
    background-color: #409eff;
    color: #fff !important;/* 悬浮时文字变为白色 */
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(64, 158, 255, 0.25);
  }
  
  &:active {
    transform: translateY(0);  /* 点击时回到原位 */
    box-shadow: 0 1px 3px rgba(64, 158, 255, 0.2);  /* 点击时减弱阴影 */
  }
  
  /* 添加图标指示切换功能 */
  &::after {
    content: "⟳";  /* 添加旋转图标 */
    margin-left: 5px;
    display: inline-block;
    transition: transform 0.3s ease;
  }
  
  &:hover::after {
    transform: rotate(180deg);  /* 悬停时图标旋转 */
  }
}

// /* 媒体查询添加对不同屏幕尺寸的适应 */
// @media (max-width: 1400px) {
//   .home-box {
//     width: 95%;
//   }
// }

// @media (max-width: 768px) {
//   .home-box {
//     width: 100%;
    
//     .analyse {
//       flex-direction: column;
//       height: auto;
      
//       .item {
//         width: 90%;
//         margin-bottom: 15px;
//       }
//     }
    
//     .active {
//       .el-col {
//         width: 25%;  /* 小屏幕时每行显示4个 */
//       }
//     }
//   }
// }
</style>
