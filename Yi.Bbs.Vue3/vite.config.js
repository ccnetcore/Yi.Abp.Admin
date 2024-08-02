import { fileURLToPath, URL } from "node:url";

import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { ElementPlusResolver } from "unplugin-vue-components/resolvers";

var CopyWebpackPlugin = require("copy-webpack-plugin");
/** @type {import('vite').UserConfig} */
export default defineConfig(({ command, mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  return {
    // envDir: 'env',
    plugins: [
      vue(),
      AutoImport({
        resolvers: [ElementPlusResolver()],
      }),
      Components({
        resolvers: [ElementPlusResolver()],
      }),
    ],
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      },
    },
    server: {
      port: 18001,
      open: true,
      proxy: {
        [env.VITE_APP_BASEAPI]: {
          target: env.VITE_APP_URL,
          changeOrigin: true,
          rewrite: (path) => path.replace(`${[env.VITE_APP_BASEAPI]}`, ""),

          //查看真实代理url
          // bypass(req, res, options) {

          //   const proxyUrl = options.target + options.rewrite(req.url);
          //   console.log(proxyUrl);
          //   req.headers['X-req-proxyURL'] = proxyUrl;
          //   res.setHeader('X-req-proxyURL', proxyUrl);

          // }
        },
        [env.VITE_APP_BASE_WS]: {
          target: env.VITE_APP_BASE_URL_WS,
          changeOrigin: true,
          rewrite: (p) => p.replace( `${[env.VITE_APP_BASE_WS]}`, ""),
          ws: true,
          //查看真实代理url
          bypass(req, res, options) {

            const proxyUrl = options.target + options.rewrite(req.url);
            // console.log(proxyUrl);
            req.headers['X-req-proxyURL'] = proxyUrl;
            res.setHeader('X-req-proxyURL', proxyUrl);

          }
        },
      },
    },
    // 增加新的配置
    build: {
      assetsInlineLimit: 0,
    },
  };
});
