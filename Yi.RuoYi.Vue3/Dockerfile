FROM node:18-alpine AS build

WORKDIR /app

COPY package.json yarn.lock ./

RUN yarn cache clean
RUN rm -rf node_modules
RUN yarn install --registry=https://registry.npmmirror.com

COPY . .

# RUN node --max-old-space-size=4096
RUN yarn build:prod

FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist /usr/share/nginx/html
CMD ["nginx", "-g", "daemon off;"]
