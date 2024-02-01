<template>
  <div class="app-container" style="width: 1300px">
    <!-- 老版用户信息页面 -->
    <!-- <el-row :gutter="20">
      <el-col :span="6" :xs="24">
        <el-card class="box-card">
          <template v-slot:header>
            <div class="clearfix">
              <span>个人信息</span>
            </div>
          </template>
          <div>
            <div class="text-center">
              <userAvatar :user="state.user" />
            </div>
            <ul class="list-group list-group-striped">
              <li class="list-group-item">
                <el-icon><User /></el-icon> 账号
                <div class="pull-right">{{ state.user.userName }}</div>
              </li>
              <li class="list-group-item">
                <el-icon><Phone /></el-icon> 手机号码
                <div class="pull-right">{{ state.user.phone }}</div>
              </li>
              <li class="list-group-item">
                <el-icon><Message /></el-icon> 用户邮箱
                <div class="pull-right">{{ state.user.email }}</div>
              </li>
              <li class="list-group-item">
                <el-icon><Avatar /></el-icon> 所属角色
                <div class="pull-right">
                  <span v-for="role in state.roles" :key="role.id"
                    >{{ role.roleName }} /</span
                  >
                </div>
              </li>
              <li class="list-group-item">
                <el-icon><Stopwatch /></el-icon> 注册日期
                <div class="pull-right">{{ state.user.creationTime }}</div>
              </li>
            </ul>
          </div>
        </el-card>
      </el-col>
      <el-col :span="18" :xs="24">
        <el-card>
          <template v-slot:header>
            <div class="clearfix">
              <span>基本资料</span>
            </div>
          </template>
          <el-tabs v-model="activeTab">
            <el-tab-pane label="基本资料" name="userinfo">
              <userInfo :user="state.user" />
            </el-tab-pane>
            <el-tab-pane label="修改密码" name="resetPwd">
              <resetPwd />
            </el-tab-pane>
            <el-tab-pane label="第三方快捷登录" name="accountSetting">
              <accountSet />
            </el-tab-pane>
          </el-tabs>
        </el-card>
      </el-col>
    </el-row> -->

    <!-- 新版用户信息页面 -->
    <el-row>
      <el-col :span="24">
        <div class="div-top">
          <div class="div-top-user">
            <div class="user-icon text-center">

              <userAvatar :user="state.user" :isDisable="!iSCurrentUserLogin"/>
            </div>
            <div class="user-info">
              <div class="user-nick">
                <div class="user-nick-left">{{ state.user.nick }} 
                
                  <el-tag effect="light" type="success"
          >{{state.user.level }}-{{state.user.levelName}} 等级</el-tag
        >
        <UserLimitTag :userLimit="state.user.userLimit"/>

        <el-tag effect="light" type="success"
          >{{state.user.money }} 钱钱</el-tag
        >
                </div>
                <div class="user-nick-right">
                  
                  其他
                </div>
              </div>

              <div class="user-remark">
                <span>{{ state.user.agreeNumber }}</span> 点赞 | <span>{{ state.user.discussNumber }}</span> 主题 | <span>{{ state.user.commentNumber }}</span> 评论
          
              </div>
              <el-divider />
              <div>
                <p>账号：{{ state.user.userName }}</p>
                <p>手机号码：{{ state.user.phone }}</p>
                <p>注册时间：{{state.user.creationTime}}</p>
                <p>个人简介：你好，世界~</p>
              </div>
            </div>
          </div>

          <div class="user-edit">
            <div class="user-info-bottom">

                <el-tabs v-model="activeTab" class="user-edit-tab">
                  <el-tab-pane label="基本资料" name="userinfo">
                    <userInfo :user="state.user" :isDisable="!iSCurrentUserLogin" />
                  </el-tab-pane>
                  <el-tab-pane v-if="iSCurrentUserLogin" label="修改密码" name="resetPwd">
                    <resetPwd />
                  </el-tab-pane>
                  <el-tab-pane v-if="iSCurrentUserLogin" label="第三方快捷登录" name="accountSetting">
                    <accountSet />
                  </el-tab-pane>
                </el-tabs>
    
            </div>
          </div>
        </div>
      </el-col>
    </el-row>

    <el-row class="div-bottom">
      <el-col :span="5" class="div-bottom-left">
        <el-menu
        default-active="1"
        class="el-menu-left"
      >
      <el-menu-item index="1">
        <el-icon><ChatLineRound /></el-icon>
          <span>主题</span>
        </el-menu-item>

        <el-menu-item index="2">
          <el-icon><Discount /></el-icon>
          <span>收藏</span>
        </el-menu-item>

        <el-menu-item index="3">
          <el-icon><ChatDotRound /></el-icon>
          <span>评论</span>
        </el-menu-item>

        <el-menu-item index="4">
          <el-icon><View /></el-icon>
          <span>关注</span>
        </el-menu-item>
      </el-menu>
    
      </el-col>
      <el-col :span="19" class="div-bottom-right">
        <div  class="div-bottom-right-content">
          <div v-for="item in discussList" :key="item.id">
            <DisscussCard :discuss="item" />
          </div>


        </div>
        <el-empty v-if="discussList.length == 0" description="空空如也" />
        <div v-else class="pagination">
      <el-pagination
        v-model:current-page="query.skipCount"
        v-model:page-size="query.maxResultCount"
        :page-sizes="[10, 20, 30, 50]"
        :background="true"
        layout="total, sizes, prev, pager, next, jumper"
        :total="discussTotal"
        @size-change="
          async (val) => {
            await getDiscuss();
          }
        "
        @current-change="
          async (val) => {
            await getDiscuss();
          }
        "
      />
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script setup name="Profile">
import userAvatar from "./UserAvatar.vue";
import userInfo from "./UserInfo.vue";
import resetPwd from "./ResetPwd.vue";
import accountSet from "./AccountSetting.vue";
import { getBbsUserProfile } from "@/apis/userApi.js";
import { onMounted, ref, reactive } from "vue";
import { getList } from "@/apis/discussApi.js";
import { useRoute } from "vue-router";
import useAuths from "@/hooks/useAuths";
const { isLogin } = useAuths();
import useUserStore from "@/stores/user";
import UserLimitTag from "@/components/UserLimitTag.vue";
const route = useRoute();
const userStore=useUserStore();
const activeTab = ref("userinfo");
const state = reactive({
  user: {},
  dept: {},
  roles: [],
  roleGroup: {},
  postGroup: {},

});
const iSCurrentUserLogin=ref(false);
const  discussList= ref([]);
const discussTotal=ref(0);
const query = reactive({
  skipCount: 1,
  maxResultCount: 10,
  userName:route.params.userName
});

function getUser() {
  getBbsUserProfile(query.userName).then((response) => {
    const res = response.data;
    state.user = res;
  });
}

const getDiscuss=async ()=>
{
 const {data:{items,totalCount}}= await getList(query);

 discussList.value=items;
 discussTotal.value=totalCount;
}

onMounted(() => {
  //效验，是否登录用户等于当前的userInfo
    if(userStore.userName ==query.userName&&isLogin)
    {
      iSCurrentUserLogin.value=true;
    }
  getUser();
  getDiscuss();
});
</script>
<style scoped  lang="scss">
.pull-right {
  float: right !important;
}
.list-group-striped > .list-group-item {
  border-left: 0;
  border-right: 0;
  border-radius: 0;
  padding-left: 0;
  padding-right: 0;
}

.list-group {
  padding-left: 0px;
  list-style: none;
}

.list-group-item {
  border-bottom: 1px solid #e7eaec;
  border-top: 1px solid #e7eaec;
  margin-bottom: -1px;
  padding: 11px 0px;
  font-size: 13px;
}

.app-container {
  padding: 20px;
}
.text-center {
  display: flex;
  justify-content: center;
  margin-bottom: 10px;
}

$topHeight: 620px;
$userHeight: 220px;
$remarkHeight: $topHeight - $userHeight;
.div-top {
  padding-top: 10px;
  background-color: #ffffff;
  min-height: $topHeight;
  margin-top: 30px;
  font-size: 14px;
  color: #555666;
  &-user {
    display: flex;
    width: 100%;
    height: $userHeight;
    .user-icon {
      flex: 1;
      height: $userHeight;
      padding: 10px;
    }
    .user-info {
      padding-left: 10px;
      flex: 9;
      height: $userHeight;
      .user-nick {
        display: flex;
        justify-content: space-between;
        padding-top: 5px;
        padding-bottom: 5px;
        &-left {
          color:#222226;
          font-size: 23px;
          font-weight: 800;
          .el-tag{

            margin-right: 10px;
          }
        }
        &-right{
            margin-right: 30px;

        }
      }

      .user-remark span {
        font-size: larger;
        font-weight: bold;
        color: black;
      }

      p {
      margin-bottom: 10px;
    }
    }
  }
  .user-edit {
    height: $remarkHeight;
    flex: 1 0 auto;
    
    margin-left: 10%;
    padding-left: 20px;

  }
}

.el-divider--horizontal
{
margin-bottom: 10px;

}
.user-edit-tab
{
width: 80%;
}
.pagination
{
  display: flex;
  justify-content: center;
  padding: 20px 0;
}
.div-bottom {
  margin-top: 20px;

  &-left {
    padding-top: 30px;
    height: 280px;
    background-color: #FFFFFF;
  }
  &-right {
    background-color: #f0f2f5;
    padding-left: 20px;
    &-content {
      width: 100%;
      background-color: #FFFFFF;
    }
  }
  .el-menu
  {
    height: 100%;
  }
  .el-menu-item
  {
    justify-content: center;

  }
}
</style>
