<template>
  <div class="home-box">
    <el-row :gutter="20" class="top-div">
      <el-col :span="17">
        <div class="scrollbar">
          <ScrollbarInfo />
        </div>
        <el-row class="left-div">
          <el-col
            :span="8"
            v-for="i in plateList"
            class="plate"
            :style="{
              'padding-left': i % 3 == 1 ? 0 : 0.2 + 'rem',
              'padding-right': i % 3 == 0 ? 0 : 0.2 + 'rem',
            }"
          >
            <PlateCard
              :name="i.name"
              :introduction="i.introduction"
              :id="i.id"
              :isPublish="i.isDisableCreateDiscuss"
            />
          </el-col>
          <template v-if="isDiscussFinished">
            <el-col :span="24" v-for="i in discussList">
              <DisscussCard :discuss="i" />
            </el-col>
          </template>
          <template v-else>
            <Skeleton :isBorder="true" />
          </template>
          <template v-if="isAllDiscussFinished">
            <el-col :span="24" v-for="i in allDiscussList">
              <DisscussCard :discuss="i" />
            </el-col>
          </template>
          <template v-else>
            <Skeleton :isBorder="true" />
          </template>
        </el-row>
      </el-col>
      <el-col :span="7">
        <el-row class="right-div">
          <el-col :span="24">
            <el-carousel trigger="click" height="150px">
              <el-carousel-item v-for="item in bannerList">
                <div class="carousel-font" :style="{ color: item.color }">
                  {{ item.name }}
                </div>
                <el-image
                  style="width: 100%; height: 100%"
                  :src="item.logo"
                  fit="cover"
                />
              </el-carousel-item>
            </el-carousel>
          </el-col>
          <div class="analyse">
            <div class="item">
              <div class="text">在线人数</div>
              <div class="content">
                <div class="name"></div>
                <div class="content-box top">
                  <div class="count">{{ userAnalyseInfo.onlineNumber }}</div>
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

          <el-col :span="24">
            <InfoCard header="访问统计" class="VisitsLineChart" text="详情">
              <template #content>
                <VisitsLineChart :option="statisOptions" class="statisChart" />
              </template>
            </InfoCard>
          </el-col>

          <el-col :span="24">
            <InfoCard header="简介" text="详情">
              <template #content>
                <div class="introduce">
                  没有什么能够阻挡，人类对代码<span style="color: #1890ff"
                    >优雅</span
                  >的追求
                </div>
              </template>
            </InfoCard>
          </el-col>

          <el-col :span="24">
            <template v-if="isPointFinished">
              <InfoCard
                :items="pointList"
                header="本月排行"
                text="更多"
                height="400"
              >
                <template #item="temp">
                  <PointsRanking :pointsData="temp" />
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard header="本月排行" text="更多">
                <template #content> <Skeleton /></template>
              </InfoCard>
            </template>
          </el-col>

          <el-col :span="24">
            <template v-if="isFriendFinished">
              <InfoCard
                :items="friendList"
                header="推荐好友"
                text="更多"
                height="400"
              >
                <template #item="temp">
                  <RecommendFriend :friendData="temp" />
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard header="推荐好友" text="更多">
                <template #content> <Skeleton /></template>
              </InfoCard>
            </template>
          </el-col>
          <el-col :span="24">
            <template v-if="isThemeFinished">
              <InfoCard
                :items="themeList"
                header="推荐主题"
                text="更多"
                height="400"
              >
                <template #item="temp">
                  <ThemeData :themeData="temp" />
                </template>
              </InfoCard>
            </template>
            <template v-else>
              <InfoCard header="推荐主题" text="更多">
                <template #content> <Skeleton /></template>
              </InfoCard>
            </template>
          </el-col>

          <el-col :span="24" style="background: transparent">
            <BottomInfo />
          </el-col>
        </el-row>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { onMounted, ref, reactive, computed, nextTick, watch } from "vue";
import DisscussCard from "@/components/DisscussCard.vue";
import InfoCard from "@/components/InfoCard.vue";
import PlateCard from "@/components/PlateCard.vue";
import ScrollbarInfo from "@/components/ScrollbarInfo.vue";
import BottomInfo from "@/components/BottomInfo.vue";
import VisitsLineChart from "./components/VisitsLineChart/index.vue";
import { access } from "@/apis/accessApi.js";
import { getList } from "@/apis/plateApi.js";
import { getList as bannerGetList } from "@/apis/bannerApi.js";
import { getHomeDiscuss } from "@/apis/discussApi.js";
import { getWeek } from "@/apis/accessApi.js";
import {
  getRecommendedTopic,
  getRecommendedFriend,
  getRankingPoints,
  getUserAnalyse,
} from "@/apis/analyseApi.js";
import { getList as getAllDiscussList } from "@/apis/discussApi.js";
import PointsRanking from "./components/PointsRanking/index.vue";
import RecommendFriend from "./components/RecommendFriend/index.vue";
import ThemeData from "./components/RecommendTheme/index.vue";
import Skeleton from "@/components/Skeleton/index.vue";
import useUserStore from "@/stores/user";
import { storeToRefs } from "pinia";
import signalR from "@/utils/signalR";

const { token } = storeToRefs(useUserStore());

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
const isThemeFinished = ref([]);
const allDiscussList = ref([]);
const isAllDiscussFinished = ref(false);
const userAnalyseInfo = ref({});

const items = [{ user: "用户1" }, { user: "用户2" }, { user: "用户3" }];
//主题查询参数
const query = reactive({
  skipCount: 1,
  maxResultCount: 10,
  isTop: true,
});

//初始化
onMounted(async () => {
  access();
  const { data: plateData } = await getList();
  plateList.value = plateData.items;
  const { data: discussData, config: discussConfig } = await getHomeDiscuss();
  discussList.value = discussData;
  isDiscussFinished.value = discussConfig.isFinish;
  const { data: bannerData } = await bannerGetList();
  bannerList.value = bannerData.items;
  const { data: weekData } = await getWeek();
  weekList.value = weekData;
  const { data: pointData, config: pointConfig } = await getRankingPoints();
  pointList.value = pointData;
  isPointFinished.value = pointConfig.isFinish;
  const { data: friendData, config: friendConfig } =
    await getRecommendedFriend();
  friendList.value = friendData;
  isFriendFinished.value = friendConfig.isFinish;
  const { data: themeData, config: themeConfig } = await getRecommendedTopic();
  themeList.value = themeData;
  isThemeFinished.value = themeConfig.isFinish;
  const { data: allDiscussData, config: allDiscussConfig } =
    await getAllDiscussList({
      Type: 0,
      skipCount: 1,
      maxResultCount: 5,
    });
  isAllDiscussFinished.value = allDiscussConfig.isFinish;
  allDiscussList.value = allDiscussData.items;
  const { data: userAnalyseInfoData } = await getUserAnalyse();
  userAnalyseInfo.value = userAnalyseInfoData;
  // 实时人数
  // await signalR.init(`main`);
  // nextTick(() => {
  //   // 初始化主题样式
  //   handleThemeStyle(useSettingsStore().theme);
  // });
});

//这里还需要监视token的变化，重新进行signalr连接
watch();
// () => token.value,
// async (newValue, oldValue) => {
//   await signalR.init(`main`);
// }

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
        padding: 0 5px;
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

  .VisitsLineChart >>> .el-card__body {
    padding: 0.5rem;
  }

  .statisChart {
    width: 100%;
    height: 300px;
  }
}
</style>
