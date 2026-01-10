<script setup lang="ts">
import type { CacheName, CacheValue } from '#/api/monitor/cache';

import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  FormItem,
  Input,
  Modal,
  Popconfirm,
  Row,
  Table,
  Textarea,
} from 'ant-design-vue';
import { DeleteOutlined, ReloadOutlined } from '@ant-design/icons-vue';

import {
  clearCacheAll,
  clearCacheKey,
  clearCacheName,
  getCacheValue,
  listCacheKey,
  listCacheName,
} from '#/api/monitor/cache';
import { message } from 'ant-design-vue';

const cacheNames = ref<CacheName[]>([]);
const cacheKeys = ref<string[]>([]);
const cacheForm = ref<Partial<CacheValue>>({});
const loading = ref(true);
const subLoading = ref(false);
const nowCacheName = ref('');

const tableHeight = computed(() => window.innerHeight - 200);

const cacheNameColumns = [
  {
    title: '序号',
    key: 'index',
    width: 60,
  },
  {
    title: '缓存名称',
    dataIndex: 'cacheName',
    key: 'cacheName',
    ellipsis: true,
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    ellipsis: true,
  },
  {
    title: '操作',
    key: 'action',
    width: 60,
    fixed: 'right' as const,
  },
];

const cacheKeyColumns = [
  {
    title: '序号',
    key: 'index',
    width: 60,
  },
  {
    title: '缓存键名',
    key: 'cacheKey',
    ellipsis: true,
  },
  {
    title: '操作',
    key: 'action',
    width: 60,
    fixed: 'right' as const,
  },
];

/** 查询缓存名称列表 */
async function getCacheNames() {
  loading.value = true;
  try {
    cacheNames.value = await listCacheName();
  } catch (error) {
    console.warn(error);
  } finally {
    loading.value = false;
  }
}

/** 刷新缓存名称列表 */
async function refreshCacheNames() {
  await getCacheNames();
  message.success('刷新缓存列表成功');
}

/** 清理指定名称缓存 */
async function handleClearCacheName(row: CacheName) {
  try {
    await clearCacheName(row.cacheName);
    message.success(`清理缓存名称[${row.cacheName}]成功`);
    await getCacheKeys();
  } catch (error) {
    console.warn(error);
  }
}

/** 查询缓存键名列表 */
async function getCacheKeys(row?: CacheName) {
  const cacheName = row !== undefined ? row.cacheName : nowCacheName.value;
  if (cacheName === '') {
    return;
  }
  subLoading.value = true;
  try {
    cacheKeys.value = await listCacheKey(cacheName);
    nowCacheName.value = cacheName;
  } catch (error) {
    console.warn(error);
  } finally {
    subLoading.value = false;
  }
}

/** 刷新缓存键名列表 */
async function refreshCacheKeys() {
  await getCacheKeys();
  message.success('刷新键名列表成功');
}

/** 清理指定键名缓存 */
async function handleClearCacheKey(cacheKey: string) {
  try {
    await clearCacheKey(nowCacheName.value, cacheKey);
    message.success(`清理缓存键名[${cacheKey}]成功`);
    await getCacheKeys();
  } catch (error) {
    console.warn(error);
  }
}

/** 查询缓存内容详细 */
async function handleCacheValue(cacheKey: string) {
  try {
    cacheForm.value = await getCacheValue(nowCacheName.value, cacheKey);
  } catch (error) {
    console.warn(error);
  }
}

/** 清理全部缓存 */
function handleClearCacheAll() {
  Modal.confirm({
    title: '提示',
    content: '确认清理全部缓存吗？',
    okType: 'danger',
    onOk: async () => {
      try {
        await clearCacheAll();
        message.success('清理全部缓存成功');
        await getCacheNames();
        cacheKeys.value = [];
        cacheForm.value = {};
        nowCacheName.value = '';
      } catch (error) {
        console.warn(error);
      }
    },
  });
}

onMounted(() => {
  getCacheNames();
});
</script>

<template>
  <Page>
    <Row :gutter="10">
      <Col :span="8">
        <Card :style="{ height: `${tableHeight}px` }">
          <template #title>
            <span>缓存列表</span>
          </template>
          <template #extra>
            <Button type="text" size="small" @click="refreshCacheNames">
              <template #icon>
                <ReloadOutlined />
              </template>
            </Button>
          </template>
          <Table
            :loading="loading"
            :data-source="cacheNames"
            :columns="cacheNameColumns"
            :scroll="{ y: tableHeight - 120 }"
            :pagination="false"
            size="small"
            :custom-row="(record: CacheName) => ({
              onClick: () => getCacheKeys(record),
              style: {
                cursor: 'pointer',
              },
            })"
          >
            <template #bodyCell="{ column, record, index }">
              <template v-if="column.key === 'index'">
                {{ index + 1 }}
              </template>
              <template v-else-if="column.key === 'cacheName'">
                {{ record.cacheName.replace(':', '') }}
              </template>
              <template v-else-if="column.key === 'action'">
                <Popconfirm
                  title="确认清理此缓存名称吗？"
                  @confirm="handleClearCacheName(record)"
                >
                  <Button type="text" danger size="small">
                    <template #icon>
                      <DeleteOutlined />
                    </template>
                  </Button>
                </Popconfirm>
              </template>
            </template>
          </Table>
        </Card>
      </Col>

      <Col :span="8">
        <Card :style="{ height: `${tableHeight}px` }">
          <template #title>
            <span>键名列表</span>
          </template>
          <template #extra>
            <Button type="text" size="small" @click="refreshCacheKeys">
              <template #icon>
                <ReloadOutlined />
              </template>
            </Button>
          </template>
          <Table
            :loading="subLoading"
            :data-source="cacheKeys"
            :columns="cacheKeyColumns"
            :scroll="{ y: tableHeight - 120 }"
            :pagination="false"
            size="small"
            :custom-row="(record: string) => ({
              onClick: () => handleCacheValue(record),
              style: {
                cursor: 'pointer',
              },
            })"
          >
            <template #bodyCell="{ column, record, index }">
              <template v-if="column.key === 'index'">
                {{ index + 1 }}
              </template>
              <template v-else-if="column.key === 'cacheKey'">
                {{ record.replace(nowCacheName, '') }}
              </template>
              <template v-else-if="column.key === 'action'">
                <Popconfirm
                  title="确认清理此缓存键名吗？"
                  @confirm="handleClearCacheKey(record)"
                >
                  <Button type="text" danger size="small">
                    <template #icon>
                      <DeleteOutlined />
                    </template>
                  </Button>
                </Popconfirm>
              </template>
            </template>
          </Table>
        </Card>
      </Col>

      <Col :span="8">
        <Card :bordered="false" :style="{ height: `${tableHeight}px` }">
          <template #title>
            <span>缓存内容</span>
          </template>
          <template #extra>
            <Button type="link" danger size="small" @click="handleClearCacheAll">
              清理全部
            </Button>
          </template>
          <Form :model="cacheForm" layout="vertical">
            <FormItem label="缓存名称:" name="cacheName">
              <Input v-model:value="cacheForm.cacheName" readonly />
            </FormItem>
            <FormItem label="缓存键名:" name="cacheKey">
              <Input v-model:value="cacheForm.cacheKey" readonly />
            </FormItem>
            <FormItem label="缓存内容:" name="cacheValue">
              <Textarea
                v-model:value="cacheForm.cacheValue"
                :rows="8"
                readonly
              />
            </FormItem>
          </Form>
        </Card>
      </Col>
    </Row>
  </Page>
</template>

