import { createRouter, createWebHistory } from "vue-router";
import Layout from "../layout/Index.vue";
import NotFound from "../views/error/404.vue";
import LoginLayout from "../layout/LoginLayout.vue";
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
          component: () => import("../views/login/index.vue"),
        },
        {
          name: "register",
          path: "/register",
          component: () => import("../views/Register.vue"),
        },
        {
          name: "qq",
          path: "/qq",
          component: () => import("../views/qqAuth/index.vue"),
          meta: {
            title: "QQ授权",
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
          path: "/profile",
          component: () => import("../views/profile/Index.vue"),
        },
        {
          name: "themeCover",
          path: "/article/:discussId",
          component: () => import("../views/Article.vue"),
          meta: {
            title: "主题封面",
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
      ],
    },
    { path: "/:pathMatch(.*)*", name: "NotFound", component: NotFound },
  ],
});

export default router;
