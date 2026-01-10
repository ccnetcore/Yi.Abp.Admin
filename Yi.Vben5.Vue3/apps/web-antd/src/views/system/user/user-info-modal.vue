<script setup lang="ts">
import type { User } from '#/api/system/user/model';

import { computed, shallowRef } from 'vue';

import { useVbenModal } from '@vben/common-ui';

import { Avatar, Descriptions, DescriptionsItem, Tag } from 'ant-design-vue';

import { findUserInfo } from '#/api/system/user';

const [BasicModal, modalApi] = useVbenModal({
  onOpenChange: handleOpenChange,
  onClosed() {
    currentUser.value = null;
  },
});

const currentUser = shallowRef<null | User>(null);

async function handleOpenChange(open: boolean) {
  if (!open) {
    return null;
  }
  modalApi.modalLoading(true);

  const { userId } = modalApi.getData() as { userId: number | string };
  const response = await findUserInfo(userId);

  // 新接口直接返回完整的用户数据，包含posts和roles数组
  currentUser.value = response as User;

  modalApi.modalLoading(false);
}

const sexLabel = computed(() => {
  if (!currentUser.value) {
    return '-';
  }
  const { sex } = currentUser.value;
  if (sex === 'Man') return '男';
  if (sex === 'Woman') return '女';
  return '-';
});
</script>

<template>
  <BasicModal :footer="false" :fullscreen-button="false" title="用户信息">
    <Descriptions v-if="currentUser" size="small" :column="1" bordered>
      <DescriptionsItem label="用户ID">
        {{ currentUser.id }}
      </DescriptionsItem>
      <DescriptionsItem label="头像">
        <Avatar v-if="currentUser.icon" :src="currentUser.icon" :size="48" />
        <span v-else>-</span>
      </DescriptionsItem>
      <DescriptionsItem label="姓名">
        {{ currentUser.name || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="昵称">
        {{ currentUser.nick || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="用户名">
        {{ currentUser.userName || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="年龄">
        {{ currentUser.age || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="性别">
        {{ sexLabel }}
      </DescriptionsItem>
      <DescriptionsItem label="用户状态">
        <Tag :color="currentUser.state ? 'success' : 'error'">
          {{ currentUser.state ? '启用' : '禁用' }}
        </Tag>
      </DescriptionsItem>
      <DescriptionsItem label="手机号">
        {{ currentUser.phone || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="邮箱">
        {{ currentUser.email || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="地址">
        {{ currentUser.address || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="IP地址">
        {{ currentUser.ip || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="个人简介">
        {{ currentUser.introduction || '-' }}
      </DescriptionsItem>
      <DescriptionsItem label="岗位">
        <div
          v-if="currentUser.posts && currentUser.posts.length > 0"
          class="flex flex-wrap gap-0.5"
        >
          <Tag v-for="item in currentUser.posts" :key="item.postId">
            {{ item.postName }}
          </Tag>
        </div>
        <span v-else>-</span>
      </DescriptionsItem>
      <DescriptionsItem label="角色">
        <div
          v-if="currentUser.roles && currentUser.roles.length > 0"
          class="flex flex-wrap gap-0.5"
        >
          <Tag v-for="item in currentUser.roles" :key="item.roleId">
            {{ item.roleName }}
          </Tag>
        </div>
        <span v-else>-</span>
      </DescriptionsItem>
      <DescriptionsItem label="创建时间">
        {{ currentUser.creationTime }}
      </DescriptionsItem>
      <DescriptionsItem label="备注">
        {{ currentUser.remark || '-' }}
      </DescriptionsItem>
    </Descriptions>
  </BasicModal>
</template>
