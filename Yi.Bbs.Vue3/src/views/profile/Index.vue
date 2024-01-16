<template>
  <div class="app-container" style="width: 1300px">
    <el-row :gutter="20">
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
                <el-icon><User /></el-icon> 用户名称
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
                <el-icon><HelpFilled /></el-icon> 所属部门
                <div class="pull-right" v-if="state.dept">
                  {{ state.dept.deptName }}
                </div>
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
                <el-icon><Stopwatch /></el-icon> 创建日期
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
    </el-row>
  </div>
</template>

<script setup name="Profile">
import userAvatar from "./UserAvatar.vue";
import userInfo from "./UserInfo.vue";
import resetPwd from "./ResetPwd.vue";
import accountSet from "./AccountSetting.vue";
import { getUserProfile } from "@/apis/userApi.js";
import { onMounted, ref, reactive } from "vue";

const activeTab = ref("userinfo");
const state = reactive({
  user: {},
  dept: {},
  roles: [],
  roleGroup: {},
  postGroup: {},
});

function getUser() {
  getUserProfile().then((response) => {
    const res = response.data;
    state.user = res.user;
    state.dept = res.dept;
    state.roles = res.roles;
    state.roleGroup = res.roleGroup;
    state.postGroup = res.postGroup;
  });
}
onMounted(() => {
  getUser();
});
</script>
<style scoped>
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
</style>
