<script setup lang="ts">
import type { ServerInfo } from '#/api/service/model';

import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Descriptions,
  DescriptionsItem,
  Row,
  Spin,
  Table,
} from 'ant-design-vue';

import { getServerInfo } from '#/api/service';

const server = ref<ServerInfo>();
const loading = ref(false);

const diskColumns = [
  {
    title: '盘符路径',
    dataIndex: 'diskName',
    key: 'diskName',
  },
  {
    title: '盘符类型',
    dataIndex: 'typeName',
    key: 'typeName',
  },
  {
    title: '总大小',
    dataIndex: 'totalSize',
    key: 'totalSize',
  },
  {
    title: '可用大小',
    dataIndex: 'availableFreeSpace',
    key: 'availableFreeSpace',
  },
  {
    title: '已用大小',
    dataIndex: 'used',
    key: 'used',
  },
  {
    title: '已用百分比',
    dataIndex: 'availablePercent',
    key: 'availablePercent',
  },
];

async function loadData() {
  loading.value = true;
  try {
    server.value = await getServerInfo();
  } catch (error) {
    console.warn(error);
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  loadData();
});
</script>

<template>
  <Page>
    <Spin :spinning="loading" tip="正在加载服务监控数据，请稍候...">
      <Row :gutter="[15, 15]">
      <Col :lg="12" :md="24" :sm="24" :xl="12" :xs="24">
        <Card size="small">
          <template #title>
            <span>CPU</span>
          </template>
          <template #extra>
            <Button size="small" @click="loadData">
              <div class="flex">
                <span class="icon-[charm--refresh]"></span>
              </div>
            </Button>
          </template>
          <Descriptions v-if="server?.cpu" bordered :column="1" size="small">
            <DescriptionsItem label="核心数">
              {{ server.cpu.coreTotal }}
            </DescriptionsItem>
            <DescriptionsItem label="逻辑处理器">
              {{ server.cpu.logicalProcessors }}
            </DescriptionsItem>
            <DescriptionsItem label="系统使用率">
              {{ server.cpu.cpuRate }}%
            </DescriptionsItem>
            <DescriptionsItem label="当前空闲率">
              {{ 100 - server.cpu.cpuRate }}%
            </DescriptionsItem>
          </Descriptions>
        </Card>
      </Col>

      <Col :lg="12" :md="24" :sm="24" :xl="12" :xs="24">
        <Card size="small">
          <template #title>
            <span>内存</span>
          </template>
          <Descriptions v-if="server?.memory" bordered :column="1" size="small">
            <DescriptionsItem label="总内存">
              {{ server.memory.totalRAM }}
            </DescriptionsItem>
            <DescriptionsItem label="已用内存">
              {{ server.memory.usedRam }}
            </DescriptionsItem>
            <DescriptionsItem label="剩余内存">
              {{ server.memory.freeRam }}
            </DescriptionsItem>
            <DescriptionsItem label="使用率">
              <span :class="{ 'text-danger': server.memory.ramRate > 80 }">
                {{ server.memory.ramRate }}%
              </span>
            </DescriptionsItem>
          </Descriptions>
        </Card>
      </Col>

      <Col :span="24">
        <Card size="small">
          <template #title>
            <span>服务器信息</span>
          </template>
          <Descriptions v-if="server?.sys" bordered :column="2" size="small">
            <DescriptionsItem label="服务器名称">
              {{ server.sys.computerName }}
            </DescriptionsItem>
            <DescriptionsItem label="操作系统">
              {{ server.sys.osName }}
            </DescriptionsItem>
            <DescriptionsItem label="服务器IP">
              {{ server.sys.serverIP }}
            </DescriptionsItem>
            <DescriptionsItem label="系统架构">
              {{ server.sys.osArch }}
            </DescriptionsItem>
          </Descriptions>
        </Card>
      </Col>

      <Col :span="24">
        <Card size="small">
          <template #title>
            <span>应用信息</span>
          </template>
          <Descriptions v-if="server?.app" bordered :column="2" size="small">
            <DescriptionsItem label="应用环境">
              {{ server.app.name }}
            </DescriptionsItem>
            <DescriptionsItem label="应用版本">
              {{ server.app.version }}
            </DescriptionsItem>
            <DescriptionsItem label="启动时间">
              {{ server.app.startTime }}
            </DescriptionsItem>
            <DescriptionsItem label="运行时长">
              {{ server.app.runTime }}
            </DescriptionsItem>
            <DescriptionsItem label="安装路径" :span="2">
              {{ server.app.rootPath }}
            </DescriptionsItem>
            <DescriptionsItem label="项目路径" :span="2">
              {{ server.app.webRootPath }}
            </DescriptionsItem>
            <DescriptionsItem label="运行参数" :span="2">
              {{ server.app.name }}
            </DescriptionsItem>
          </Descriptions>
        </Card>
      </Col>

      <Col :span="24">
        <Card size="small">
          <template #title>
            <span>磁盘状态</span>
          </template>
          <Table
            v-if="server?.disk"
            :columns="diskColumns"
            :data-source="server.disk"
            :pagination="false"
            size="small"
            bordered
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'availablePercent'">
                <span :class="{ 'text-danger': record.availablePercent > 80 }">
                  {{ record.availablePercent }}%
                </span>
              </template>
            </template>
          </Table>
        </Card>
      </Col>
      </Row>
    </Spin>
  </Page>
</template>

<style scoped>
.text-danger {
  color: #ff4d4f;
}
</style>

