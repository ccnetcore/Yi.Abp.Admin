FROM nginx:stable-alpine3.20-perl as base
EXPOSE 18001

FROM node:20.18 as publish
WORKDIR /main
COPY . .
RUN npm i
RUN npm run build

FROM base AS final
WORKDIR /app
COPY --from=publish /main/dist/ .
ENTRYPOINT  ["nginx", "-g", "daemon off;"]