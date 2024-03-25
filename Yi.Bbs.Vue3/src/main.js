import { createApp } from "vue";
import { createPinia } from "pinia";

import App from "./App.vue";
import router from "./router";
import piniaPluginPersistedstate from "pinia-plugin-persistedstate";

import "element-plus/dist/index.css";
import "./assets/main.css";
import "@/assets/styles/index.scss"; // global css


import * as ElementPlusIconsVue from "@element-plus/icons-vue";
import directive from "./directive"; // directive
import VueLuckyCanvas from '@lucky-canvas/vue'

import "./permission";

(async () => {
  const app = createApp(App);
  for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component);
  }
  const pinia = createPinia();
  pinia.use(piniaPluginPersistedstate);
  app.use(pinia);
  directive(app);
  app.use(router);
  app.use(VueLuckyCanvas);
  await router.isReady();
  app.mount("#app");
})();
