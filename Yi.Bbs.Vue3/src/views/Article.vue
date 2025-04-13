<template>
  <div class="article-box">
    <el-row :gutter="20" class="top-div">
      <el-col :span="5">
        <el-row class="art-info-left">
          <el-col :span="24"  v-if="discuss.discussType=='Article'">
            <InfoCard header="文章信息" text="展开" hideDivider="true" :isPadding="false" style="padding: 10px">
              <template #content>
                <el-button
                    style="width: 100%; margin-bottom: 0.8rem"
                    @click="loadDiscuss(true)"
                >主题首页
                </el-button
                >
                <el-button
                    v-if="isAddArticle && isArticleUser"
                    @click="addArticle('00000000-0000-0000-0000-000000000000')"
                    type="primary"
                    style="width: 100%; margin-bottom: 0.8rem; margin-left: 0"
                >添加子文章
                </el-button
                >
                <!--目录在这里 -->
                <el-scrollbar style="height:600px;overflow-y: auto;">
                  <TreeArticleInfo
                      :data="articleData"
                      @remove="delArticle"
                      @update="updateArticle"
                      @create="addNextArticle"
                      @handleNodeClick="handleNodeClick"
                      :currentNodeKey="currentNodeKey"
                      :isArticleUser="isArticleUser"
                  />
                </el-scrollbar>
              </template>
            </InfoCard>
          </el-col>

          <el-col :span="24"  v-if="discuss.discussType=='Reward'">
            <InfoCard header="悬赏信息" text="有偿" hideDivider="true" :isPadding="true" style="padding: 10px">
              <template #content>
                <p>
                  当前状态：
                <el-tag
                    v-if="discuss.rewardData.isResolved"
                    type="success"
                    effect="dark"
                    round
                >已解决</el-tag>

                <el-tag
                    v-else
                    type="danger"
                    effect="dark"
                    round
                >未解决</el-tag>
                </p>
                <el-divider/>
                <p>参考价格：{{discuss.rewardData.minValue}}~{{discuss.rewardData.maxValue}}RMB</p>
                <el-divider/>
                <p style="color: #AB9AAA">联系作者：</p>
                <p>{{discuss.rewardData.contact}}</p>
              </template>
            </InfoCard>
          </el-col>
          
          <el-col :span="24">
            <InfoCard :items="authorList" :isPadding="false" header="作者分享" height="410" text="更多" style="padding:0 20px">
              <template #item="temp">
                <ThemeData :themeData="temp"/>
              </template>
            </InfoCard>
          </el-col>
        </el-row>
      </el-col>

      <el-col :span="14">
        <el-row class="left-div">
          <el-col :span="24">
            <Breadcrumb :breadcrumbsList="breadcrumbsList" class="breadcrumb"/>

            <AvatarInfo
                :size="50"
                :showWatching="true"
                :time="discuss.creationTime"
                :userInfo="discuss.user"
            ></AvatarInfo>
            
            <h2> <DiscussTypeLable style="margin-right: 8px;height: 35px;" v-model="discuss.discussType" />  {{ discuss.title }}</h2>
            <h5 class="subtitle">{{ discuss.introduction }}</h5>
            <el-image
                :preview-src-list="[getUrl(discuss.cover)]"
                v-if="discuss.cover"
                :src="getUrl(discuss.cover)"
                style="width: 150px; height: 150px"
            />

            <div v-if="discuss.discussType=='Reward'">
              <el-divider/>
            <p style="color: red">通过”悬赏主题“发布问题，双方达成一致并解决问题后，建议有偿提供RMB给解决人员</p>
            <p style="color: red">社区只提供解决问题平台，不参与任何交易，请自行联系</p>
            </div>
            <el-divider/>
            <el-skeleton :rows="10" animated v-if="discuss.content==undefined" />
            <ArticleContentInfo
                :code="discuss.content ?? ''"
            ></ArticleContentInfo>

            <el-divider class="tab-divider"/>

            <el-space :size="10" :spacer="spacer">
              <AgreeInfo :data="discuss"/>
              <el-button icon="Star" text> 0</el-button>
              <el-button icon="Share" text> 分享</el-button>
              <el-button icon="Operation" text> 操作</el-button>
            </el-space>
          </el-col>

          <el-col :span="24" class="comment">
            <CommentInfo :isComment="isDisabledCreateComment"/>
          </el-col>
        </el-row>
        <BottomInfo/>
      </el-col>

      <el-col :span="5">
        <el-row class="right-div">
          <el-col :span="24">
            <InfoCard
                class="art-info-right"
                header="主题信息"
                text="更多"
                hideDivider="true"
            >
              <template #content>
                <div>
                  <ul class="art-info-ul">
                    <li>
                      <el-button
                          type="primary"
                          size="default"
                          v-if="isEditTheme && isArticleUser&&discuss.discussType=='Article'"
                          @click="updateHander(route.params.discussId)"
                      >编辑
                      </el-button
                      >
                      <el-button
                          type="warning"
                          size="default"
                          v-if="isEditTheme && isArticleUser&&discuss.discussType=='Reward'&&discuss.rewardData.isResolved==false"
                          @click="updateSolveHander(route.params.discussId)"
                      >设置已解决
                      </el-button
                      >
                      
                      
                      <el-button
                          style="margin-left: 1rem"
                          type="danger"
                          v-if="isRemoveTheme && isArticleUser"
                          @click="delHander(route.params.discussId)"
                      >删除
                      </el-button
                      >
                    </li>
                    <li>分类： <span>主题</span></li>
                    标签：
                    <el-tag type="success">主题</el-tag>
                    <el-tag type="info">资源</el-tag>
                  </ul>
                </div>
              </template>
            </InfoCard>
          </el-col>
          <el-col :span="24">
            <InfoCard class="art-info-right" header="目录" hideDivider="true">
              <template #content>
                <div>
                  <el-empty
                      :image-size="100"
                      style="padding: 20px 0"
                      v-if="catalogueData.length == 0"
                      description="无目录"
                  />
                  <ul v-else class="art-info-ul">
                    <li v-for="(item, i) in catalogueData" :key="i">
                      <el-button
                          style="width: 100%; justify-content: left"
                          type="primary"
                          text
                      >{{ `${i + 1} ： ${item}` }}
                      </el-button
                      >
                    </li>
                  </ul>
                </div>
              </template>
            </InfoCard>
          </el-col>
          <el-col :span="24">
            <InfoCard :items="themeList" :isPadding="false" header="推荐主题" text="更多" height="500" style="padding:0 20px">
              <template #item="temp">
                <ThemeData :themeData="temp"/>
              </template>
            </InfoCard>
          </el-col>
        </el-row>
      </el-col>
    </el-row>
  </div>
</template>
<script setup>
import {h, ref, onMounted, watch, computed} from "vue";
import AvatarInfo from "@/components/AvatarInfo.vue";
import InfoCard from "@/components/InfoCard.vue";
import ArticleContentInfo from "@/components/ArticleContentInfo.vue";
import CommentInfo from "@/components/CommentInfo.vue";
import BottomInfo from "@/components/BottomInfo.vue";
import TreeArticleInfo from "@/components/TreeArticleInfo.vue";
import {useRoute, useRouter} from "vue-router";
import AgreeInfo from "@/components/AgreeInfo.vue";
import DiscussTypeLable from "@/components/DiscussTypeLable.vue";
import {get as discussGet, del as discussDel,setResolve} from "@/apis/discussApi.js";
import {
  all as articleall,
  del as articleDel,
  get as articleGet,
} from "@/apis/articleApi.js";
import Breadcrumb from "@/components/Breadcrumb/index.vue";
import {getPermission} from "@/utils/auth";
import useUserStore from "@/stores/user.js";
import ThemeData from "@/views/home/components/RecommendTheme/index.vue";
import {getRecommendedTopic, getAuthorTopic} from "@/apis/analyseApi";
//数据定义
const route = useRoute();
const router = useRouter();
const spacer = h(ElDivider, {direction: "vertical"});
//子文章数据
const articleData = ref([]);
//主题内容
const discuss = ref({});
//推荐主题
const themeList = ref([]);
//作者主题
const authorList = ref([]);
//封面url
const getUrl = (str) => {
  return `${import.meta.env.VITE_APP_BASEAPI}/file/${str}`;
};

//当前默认选择的子文章
const currentNodeKey = route.params.articleId;
//目录数据
const catalogueData = ref([]);

// 面包屑导航列表
const breadcrumbsList = ref([]);
const resultRouters = ["index", "discuss", "themeCover"];
breadcrumbsList.value = route.matched[0].children
    .filter((item) => resultRouters.includes(item.name))
    .sort((a, b) => {
      return resultRouters.indexOf(a.name) - resultRouters.indexOf(b.name);
    });

// 当前文章名称
const currentArticle = ref("");

//子文章初始化
const loadArticleData = async () => {
  const response = await articleall(route.params.discussId);
  articleData.value = response.data;
};

//主题初始化
const isDisabledCreateComment = ref(false);
const isArticleUser = ref(false);
const {isHasPermission: isAddArticle} = getPermission("bbs:article:add");
const {isHasPermission: isEditTheme} = getPermission("bbs:discuss:update");
const {isHasPermission: isRemoveTheme} = getPermission("bbs:discuss:del");
const loadDiscuss = async (isRewrite) => {
  if (isRewrite) {
    //跳转路由
    router.push(`/article/${route.params.discussId}`);
  }
  discuss.value = (await discussGet(route.params.discussId)).data;
  isDisabledCreateComment.value = discuss.value.isDisabledCreateComment;
  isArticleUser.value = discuss.value?.user?.id === useUserStore().id;
  if (route.params.articleId != "") {
    const response = await articleGet(route.params.articleId);
    discuss.value.content = response.data.content;
  }
  breadcrumbsList.value[1].path = "/discuss/" + discuss.value.plateId;
  breadcrumbsList.value[1].meta.title = discuss.value.plate.name;
  ContentHander();
};
//加载文章及目录
const ContentHander = () => {
  //加载目录
  var reg = /(#{1,6})\s(.*)/g;

  if (discuss.value.content) {
    var myArray = discuss.value.content.match(reg);
    if (myArray != null) {
      catalogueData.value = myArray.map((x) => {
        return x.replace(/#/g, "").replace(/\s/g, "");
      });
    }
  }
};
//添加树型子文章
const addArticle = (parentArticleId) => {
  //跳转路由
  var routerPer = {
    path: "/editArt",
    query: {
      operType: "create",
      artType: "article",
      discussId: route.params.discussId,
      parentArticleId: parentArticleId,
    },
  };
  router.push(routerPer);
};

//删除主题
const delHander = async (ids) => {
  ElMessageBox.confirm(`确定是否删除编号[${ids}]的主题吗?`, "警告", {
    confirmButtonText: "确认",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    const response = await discussDel(ids);
    //删除成功后，跳转到主页
    router.push("/index");
    ElMessage({
      type: "success",
      message: "删除成功",
    });
  });
};
//设置已解决
const updateSolveHander=(discussId)=>{
  ElMessageBox.confirm(`确定要将此悬赏主题设置已解决吗？`, "警告", {
    confirmButtonText: "确认",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    await setResolve(discussId);
    ElMessage({
      type: "success",
      message: "设置成功",
    });
    await loadDiscuss();
    
  });
};
//更新操作
const updateHander = (discussId) => {
  //跳转路由
  var routerPer = {
    path: "/editArt",
    query: {
      operType: "update",
      artType: "discuss",
      discussId: discussId,
    },
  };
  router.push(routerPer);
};

//跳转添加子菜单
const addNextArticle = (node, data) => {
  //跳转路由
  var routerPer = {
    path: "/editArt",
    query: {
      operType: "create",
      artType: "article",
      discussId: data.discussId,
      parentArticleId: data.id,
    },
  };
  router.push(routerPer);
};

//跳转更新子菜单
const updateArticle = (node, data) => {
  //跳转路由
  var routerPer = {
    path: "/editArt",
    query: {
      operType: "update",
      artType: "article",
      discussId: data.discussId,
      parentArticleId: data.parentId,
      articleId: data.id,
    },
  };
  router.push(routerPer);
};
//单机节点
const handleNodeClick = async (data) => {
  currentArticle.value = data?.name;
  //跳转路由

  router.push(`/article/${route.params.discussId}/${data.id}`);


};
//删除子文章
const delArticle = (node, data) => {
  ElMessageBox.confirm(`确定是否删除编号[${data.id}]的子文章吗?`, "警告", {
    confirmButtonText: "确认",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    await articleDel(data.id);
    await loadArticleData();

    ElMessage({
      type: "success",
      message: "删除成功",
    });
  });
};
const loadThemeData = async () => {
  const {data: themeData} = await getRecommendedTopic();
  themeList.value = themeData;
}
const loadAuthorData = async () => {
  const {data: authorData} = await getAuthorTopic(discuss.value.user.id);
  authorList.value = authorData;
}
onMounted(async () => {
  await loadDiscuss();
  await loadArticleData();
  await loadAuthorData();
  await loadThemeData();
});


watch(
    () => currentArticle.value,
    (val) => {
      if (val !== "") {
        breadcrumbsList.value[3] = {
          name: "article",
          path: "/article/:discussId",
          meta: {
            title: currentArticle.value,
          },
        };
      }
    }
);
watch(
    () => route.params,
    async (val) => {
      if (val.articleId !=="")
      {
        const response = await articleGet(route.params.articleId);
        discuss.value.content = response.data.content;
        ContentHander();
      }
      else if (val.discussId !== "") {
        discuss.value = (await discussGet(route.params.discussId)).data;
      }
    }
);
//路由发送变化，重新加载
// watch(() => route.params, async () => {
//       await loadDiscuss();
//       await loadArticleData();
//       await loadAuthorData();
//       await loadThemeData();
//     }
// )
</script>
<style scoped lang="scss">

.subtitle {
  color: #999AAA;
  margin: 0;
}

.article-box {
  width: 1500px;
  height: 100%;

  .comment {
    min-height: 40rem;
  }

  .art-info-left .el-col {
    margin-bottom: 1rem;
  }

  .art-info-ul span {
    margin-left: 1rem;
  }

  .art-info-ul .el-tag {
    margin-left: 1rem;
  }

  .art-info-ul {
    padding: 0;
    margin: 0;
  }

  li {
    list-style: none;
    margin-bottom: 0.5rem;
  }

  .art-info-right {
    height: 100%;
  }

  .left-div .el-col {
    background-color: #ffffff;
    min-height: 12rem;
    margin-bottom: 1rem;
  }

  .right-div .el-col {
    background-color: #ffffff;
    min-height: 10rem;
    margin-bottom: 1rem;
  }

  .left-top-div .el-col {
    min-height: 2rem;
    background-color: #fafafa;
    margin-bottom: 1rem;
    margin-left: 10px;
  }

  .top-div {
    padding-top: 1rem;
  }

  .left-top-div {
    font-size: small;
    text-align: center;
    line-height: 2rem;
  }

  h2 {
    margin-bottom: 0.5em;
    color: rgba(0, 0, 0, 0.85);
    font-weight: 600;
    font-size: 30px;
    line-height: 1.35;
  }

  .left-div .el-col {
    padding: 1.4rem 1.4rem 0.5rem 1.4rem;
  }

  .el-space {
    display: flex;
    vertical-align: top;
    justify-content: space-evenly;
  }

  .tab-divider {
    margin-bottom: 0.5rem;
  }

  .breadcrumb {
    margin-bottom: 10px;
  }
}
</style>
