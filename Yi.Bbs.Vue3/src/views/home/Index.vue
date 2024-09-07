<template>
  <div class="home-box">
    <el-row :gutter="20" class="top-div">
      <el-col :span="17">
        <div class="chat-hub">
          <p @click="onClickToChatHub">点击前往-最新上线<span>《聊天室》 </span>，现已支持<span>Ai助手</span>，希望能帮助大家
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
            <PlateCard :name="i.name" :introduction="i.introduction" :id="i.id" :isPublish="i.isDisableCreateDiscuss"/>
          </el-col>
          <template v-if="isDiscussFinished">
            <el-col :span="24" v-for="i in discussList" :key="i.id">
              <DisscussCard :discuss="i"/>
            </el-col>
          </template>
          <template v-else>
            <Skeleton :isBorder="true"/>
          </template>
          <template v-if="isAllDiscussFinished">
            <el-col :span="24" v-for="i in allDiscussList" :key="i.id">
              <DisscussCard :discuss="i"/>
            </el-col>
          </template>
          <template v-else>
            <Skeleton :isBorder="true"/>
          </template>
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
          <el-col :span="24">
            <InfoCard header="活动">
              <template #content>
                <div class="top">你好，很高兴今天又遇到你呀~</div>
                <el-row class="active">

                  <el-col v-for="item in activeList" :span="6" @click="handleToRouter(item.path)">

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
          </el-col>

          <el-col :span="24">
            <InfoCard header="简介" text="">
              <template #content>
                <div class="introduce">
                  没有什么能够阻挡，人类对代码<span style="color: #1890ff">优雅</span>的追求
                </div>
              </template>
            </InfoCard>
          </el-col>

          <el-col :span="24">
            <template v-if="isPointFinished">
              <InfoCard :items="pointList" header="财富排行榜" text="查看我的位置" height="400"
                        @onClickText="onClickMoneyTop">
                <template #item="temp">
                  <PointsRanking :pointsData="temp"/>
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard header="本月排行" text="更多">
                <template #content>
                  <Skeleton/>
                </template>
              </InfoCard>
            </template>
          </el-col>

          <el-col :span="24">
            <template v-if="isFriendFinished">
              <InfoCard :items="friendList" header="推荐好友" text="更多" height="400">
                <template #item="temp">
                  <RecommendFriend :friendData="temp"/>
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard header="推荐好友" text="更多">
                <template #content>
                  <Skeleton/>
                </template>
              </InfoCard>
            </template>
          </el-col>
          <el-col :span="24">
            <template v-if="isThemeFinished">
              <InfoCard :items="themeList" header="推荐主题" text="更多" height="400">
                <template #item="temp">
                  <ThemeData :themeData="temp"/>
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard header="推荐主题" text="更多">
                <template #content>
                  <Skeleton/>
                </template>
              </InfoCard>
            </template>
          </el-col>

          <el-col :span="24" style="background: transparent">
            <BottomInfo/>
          </el-col>
        </el-row>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import {onMounted, ref, reactive, computed, nextTick, watch} from "vue";
import {useRouter} from "vue-router";
import DisscussCard from "@/components/DisscussCard.vue";
import InfoCard from "@/components/InfoCard.vue";
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
  getRankingPoints,
  getUserAnalyse,
  getRegisterAnalyse
} from "@/apis/analyseApi.js";
import {getList as getAllDiscussList} from "@/apis/discussApi.js";
import PointsRanking from "./components/PointsRanking/index.vue";
import RecommendFriend from "./components/RecommendFriend/index.vue";
import ThemeData from "./components/RecommendTheme/index.vue";
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
const activeList = [
  {name: "签到", path: "/activity/sign", icon: "Present"},
  {name: "等级", path: "/activity/level", icon: "Ticket"},
  {name: "大转盘", path: "/activity/lucky", icon: "Sunny"},
  {name: "银行", path: "/activity/bank", icon: "CreditCard"},

  {name: "任务", path: "/activity/assignment", icon: "Memo"},
  {name: "排行榜", path: "/money", icon: "Money"},
  {name: "开始", path: "/start", icon: "Position"},
  {name: "聊天室", path: "/chat", icon: "ChatRound"},
];

//主题查询参数
const query = reactive({
  skipCount: 1,
  maxResultCount: 10,
  isTop: true,
});

const weekQuery = reactive({accessLogType: "Request"});
//初始化
onMounted(async () => {
  access();
  const {data: plateData} = await getList();
  plateList.value = plateData.items;
  const {data: discussData, config: discussConfig} = await getHomeDiscuss();
  discussList.value = discussData;
  isDiscussFinished.value = discussConfig.isFinish;
  const {data: bannerData} = await bannerGetList();
  bannerList.value = bannerData.items;
  const {data: weekData} = await getWeek(weekQuery);
  weekList.value = weekData;
  const {data: pointData, config: pointConfig} = await getRankingPoints();
  pointList.value = pointData.items;
  isPointFinished.value = pointConfig.isFinish;
  const {data: friendData, config: friendConfig} =
      await getRecommendedFriend();
  friendList.value = friendData;
  isFriendFinished.value = friendConfig.isFinish;
  const {data: themeData, config: themeConfig} = await getRecommendedTopic();
  themeList.value = themeData;
  isThemeFinished.value = themeConfig.isFinish;
  const {data: allDiscussData, config: allDiscussConfig} =
      await getAllDiscussList({
        Type: 0,
        skipCount: 1,
        maxResultCount: 30,
      });
  isAllDiscussFinished.value = allDiscussConfig.isFinish;
  allDiscussList.value = allDiscussData.items;
  const {data: userAnalyseInfoData} = await getUserAnalyse();
  onlineNumber.value = userAnalyseInfoData.onlineNumber;
  userAnalyseInfo.value = userAnalyseInfoData;
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

//切换统计开关
const onClickWeekSwitch = async () => {
  if (weekQuery.accessLogType === "HomeClick") {
    weekQuery.accessLogType= "Request";
  } else if (weekQuery.accessLogType === "Request") {
    weekQuery.accessLogType = "HomeClick";
  }

  const {data: weekData} = await getWeek(weekQuery);
  weekList.value = weekData;
}
</script>
<style scoped lang="scss">
.home-box {
  width: 1300px;
  height: 100%;

  .introduce {
    color: rgba(0, 0, 0, 0.45);
    font-size: small;
  }

  .plate {
    background: transparent !important;
  }

  .left-div .el-col {
    background-color: #ffffff;

    margin-bottom: 1rem;
  }

  .right-div .el-col {
    background-color: #ffffff;
    margin-bottom: 1rem;
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
    justify-content: space-between;
    align-items: center;
    color: #8a919f;


    .el-col {
      flex-direction: column;
      align-items: center;
      display: flex;
      cursor: pointer;
      padding: 10px 0px;
    }

    .el-col:hover {
      background-color: #cce1ff;
      /* 悬浮时背景色变化 */
      color: #70aafb;
      /* 悬浮时文字颜色变化 */
    }

    &-btn {
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      width: 74px;
      height: 36px;
      border-radius: 4px;
      border: 1px solid rgba(30, 128, 255, 0.3);
      background-color: rgba(30, 128, 255, 0.1);
      color: #1e80ff;
    }
  }

  .VisitsLineChart > > > .el-card__body {
    padding: 0.5rem;
  }
  
.VisitsLineChart p{
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
  height: 30px;

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
</style>
