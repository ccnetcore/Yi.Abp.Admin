<script setup lang="ts">
import type { OperationLog } from '#/api/monitor/operlog/model';

import { computed, shallowRef } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';
import { DictEnum } from '@vben/constants';

import { Descriptions, DescriptionsItem, Tag } from 'ant-design-vue';

import {
  renderDict,
  renderHttpMethodTag,
  renderJsonPreview,
} from '#/utils/render';

const [BasicDrawer, drawerApi] = useVbenDrawer({
  onOpenChange: handleOpenChange,
  onClosed() {
    currentLog.value = null;
  },
});

const currentLog = shallowRef<null | OperationLog>(null);
function handleOpenChange(open: boolean) {
  if (!open) {
    return null;
  }
  const { record } = drawerApi.getData() as { record: OperationLog };
  currentLog.value = record;
}

const actionInfo = computed(() => {
  if (!currentLog.value) {
    return '-';
  }
  const data = currentLog.value;
  return `账号: ${data.operUser} / ${data.operIp} / ${data.operLocation}`;
});
</script>

<template>
  <BasicDrawer :footer="false" class="w-[600px]" title="查看日志">
    <Descriptions v-if="currentLog" size="small" bordered :column="1">
      <DescriptionsItem label="日志编号" :label-style="{ minWidth: '120px' }">
        {{ currentLog.id }}
      </DescriptionsItem>
      <DescriptionsItem label="操作模块">
        <div class="flex items-center">
          <Tag>{{ currentLog.title }}</Tag>
          <component
            :is="renderDict(currentLog.operType, DictEnum.SYS_OPER_TYPE)"
          />
        </div>
      </DescriptionsItem>
      <DescriptionsItem label="操作信息">
        {{ actionInfo }}
      </DescriptionsItem>
      <DescriptionsItem label="请求信息">
        <component :is="renderHttpMethodTag(currentLog.requestMethod)" />
      </DescriptionsItem>
      <DescriptionsItem label="方法">
        {{ currentLog.method }}
      </DescriptionsItem>
      <DescriptionsItem label="请求参数">
        <div class="max-h-[300px] overflow-y-auto">
          <component :is="renderJsonPreview(currentLog.requestParam)" />
        </div>
      </DescriptionsItem>
      <DescriptionsItem v-if="currentLog.requestResult" label="响应参数">
        <div class="max-h-[300px] overflow-y-auto">
          <component :is="renderJsonPreview(currentLog.requestResult)" />
        </div>
      </DescriptionsItem>
      <DescriptionsItem label="操作时间">
        {{ `${currentLog.creationTime}` }}
      </DescriptionsItem>
    </Descriptions>
  </BasicDrawer>
</template>
