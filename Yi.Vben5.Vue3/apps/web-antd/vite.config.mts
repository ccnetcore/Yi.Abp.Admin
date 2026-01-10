import { defineConfig } from '@vben/vite-config';
import { loadEnv } from "vite";
// 自行取消注释来启用按需导入功能
// import { AntDesignVueResolver } from 'unplugin-vue-components/resolvers';
// import Components from 'unplugin-vue-components/vite';

export default defineConfig(async ({ command, mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  return {
    application: {},
    vite: {
      plugins: [
        // Components({
        //   dirs: [], // 默认会导入src/components目录下所有组件 不需要
        //   dts: './types/components.d.ts', // 输出类型文件
        //   resolvers: [
        //     AntDesignVueResolver({
        //       // 需要排除Button组件 全局已经默认导入了
        //       exclude: ['Button'],
        //       importStyle: false, // css in js
        //     }),
        //   ],
        // }),
      ],
      server: {
        proxy: {
          [env.VITE_GLOB_API_URL]: {
            target: env.VITE_APP_URL,
            changeOrigin: true,
            rewrite: (path) => path.replace(`${[env.VITE_GLOB_API_URL]}`, ""),

            //查看真实代理url
            bypass(req, res, options) {
              const proxyUrl = options.target + options.rewrite(req.url);
              console.log(proxyUrl);
              req.headers['X-req-proxyURL'] = proxyUrl;
              res.setHeader('X-req-proxyURL', proxyUrl);
            }
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
    },
  };
});
