import { createRouter, createWebHistory } from "vue-router";
import Layout from "../layout/Index.vue";
import NotFound from "../views/error/404.vue";
import LoginLayout from "../layout/LoginLayout.vue";
import ActivityLayout from "../layout/activity/Index.vue";
import ChatLayout from "../layout/ChatLayout.vue"
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  scrollBehavior(to, from, savedPosition) {
    // 始终滚动到顶部
    return { top: 0 };
  },
  routes: [
    {
      name: "test",
      path: "/test",
      component: () => import("../views/Test.vue"),
    },
    {
      path: "/loginLayout",
      name: "loginLayout",
      component: LoginLayout,
      redirect: "/login",
      children: [
        {
          name: "login",
          path: "/login",
          // component: () => import("../views/Login.vue"),
          component: () => import("../views/login/login.vue"),
        },
        {
          name: "register",
          path: "/register",
          component: () => import("../views/login/register.vue"),
        },
        {
          name: "forgotPassword",
          path: "/forgotPassword",
          component: () => import("../views/login/forgotPassword.vue"),
        },
        {
          name: "auth",
          path: "/auth/:type",
          component: () => import("../views/Auth/index.vue"),
          meta: {
            title: "授权",
          },
        },
      ],
    },
    {
      path: "/",
      name: "layout",
      component: Layout,
      redirect: "/index",
      children: [
        {
          name: "index",
          path: "/index",
          component: () => import("../views/home/Index.vue"),
          meta: {
            title: "首页",
          },
        },
        {
          name: "start",
          path: "/start",
          component: () => import("../views/start/Index.vue"),
          meta: {
            title: "开始",
          },
        },
        {
          name: "article",
          path: "/article/:discussId/:articleId?",
          component: () => import("../views/Article.vue"),
        },
        {
          name: "discuss",
          path: "/discuss/:plateId?/:isPublish?",
          component: () => import("../views/discuss/index.vue"),
          meta: {
            title: "板块",
          },
        },
        {
          //artType：discuss主题、article文章
          //operType：create创建、update更新
          name: "editArt",
          path: "/editArt",
          component: () => import("../views/EditArticle.vue"),
        },
        {
          name: "profile",
          path: "/profile/:userName",
          component: () => import("../views/profile/Index.vue"),
        },
        {
          name: "themeCover",
          path: "/article/:discussId",
          component: () => import("../views/Article.vue"),
          meta: {
            title: "主题首页",
          },
        },
        {
          name: "contact",
          path: "/contact",
          component: () => import("../views/contact/index.vue"),
          meta: {
            title: "联系我们",
          },
        },

        {
          name:"money",
          path:"/money",
          component: () => import("../views/money/Index.vue"),
          meta: {
            title: "钱钱排行榜",
          },
        },
        {
          name:"shop",
          path:"/shop",
          component: () => import("../views/shop/Index.vue"),
          meta: {
            title: "商城",
          },
        },
        {
          name:"dc",
          path:"/dc",
          component: () => import("../views/dc/Index.vue"),
          meta: {
            title: "数字藏品",
          },
        },
        {
          name: "book",
          path: "/book",
          component: () => import("../views/book/Index.vue"),
          meta: {
            title: "面试宝典",
          },
        },
      ],
    },
    {
      path: "/activity",
      name: "activityLayout",
      component: ActivityLayout,
      redirect: "/activity/sign",
      children: [
        {
          name: "sign",
          path: "sign",
          component: () => import("../views/signIn/Index.vue"),
          meta: {
            title: "每日签到",
          },
        },

        {
          name: "level",
          path: "level",
          component: () => import("../views/level/Index.vue"),
          meta: {
            title: "等级",
          },
        },
        {
          name: "lucky",
          path: "lucky",
          component: () => import("../views/lucky/Index.vue"),
          meta: {
            title: "大转盘",
          },
        },
        {
          name: "bank",
          path: "bank",
          component: () => import("../views/bank/Index.vue"),
          meta: {
            title: "银行",
          },
        },
        {
          name: "assignment",
          path: "assignment",
          component: () => import("../views/assignment/Index.vue"),
          meta: {
            title: "任务",
          },
        },
      ],
    },
    {
      name: "stock",
      path: "/stock",
      component: () => import("../views/stock/Index.vue"),
      meta: {
        title: "股票",
      },
    },
    {
      path: "/hub",
      name: "hub",
      component: ChatLayout,
      redirect: "/chat",
      children: [
        {
          name: "main",
          path: "/chat",
          component: () => import("../views/chathub/Index.vue"),
          meta: {
            title: "聊天室",
          },
        }
      ],
    },

    { path: "/:pathMatch(.*)*", name: "NotFound", component: NotFound },
  ],
});

export default router;
