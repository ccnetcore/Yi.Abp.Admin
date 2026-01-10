<script setup lang="ts">
import type { DeptOption } from '#/api/system/role/model';

import { ref } from 'vue';

import { useVbenModal } from '@vben/common-ui';
import { cloneDeep } from '@vben/utils';

import { useVbenForm } from '#/adapter/form';
import { roleDataScope, roleDeptTree, roleInfo } from '#/api/system/role';
import { TreeSelectPanel } from '#/components/tree';
import { defaultFormValueGetter, useBeforeCloseDiff } from '#/utils/popup';

import { authModalSchemas } from './data';

const emit = defineEmits<{ reload: [] }>();

const [BasicForm, formApi] = useVbenForm({
  commonConfig: {
    componentProps: {
      class: 'w-full',
    },
  },
  layout: 'vertical',
  schema: authModalSchemas(),
  showDefaultActions: false,
});

// 空GUID，用于判断根节点
const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

const deptTree = ref<DeptOption[]>([]);
async function setupDeptTree(id: number | string) {
  const resp = await roleDeptTree(id);
  // 处理部门树数据：将根节点的 parentId 置为 null，以便 Tree 组件正确识别根节点
  const processedDepts = resp.depts.map((dept) => {
    const processNode = (node: DeptOption): DeptOption => {
      return {
        ...node,
        parentId:
          !node.parentId || node.parentId === EMPTY_GUID ? null : node.parentId,
        children: node.children?.map(processNode) ?? null,
      };
    };
    return processNode(dept);
  });
  deptTree.value = processedDepts;
  // 设置已选中的部门IDs
  formApi.setFieldValue('deptIds', resp.checkedKeys);
}

async function customFormValueGetter() {
  const v = await defaultFormValueGetter(formApi)();
  // 获取勾选信息
  const menuIds = deptSelectRef.value?.[0]?.getCheckedKeys() ?? [];
  const mixStr = v + menuIds.join(',');
  return mixStr;
}

const { onBeforeClose, markInitialized, resetInitialized } = useBeforeCloseDiff(
  {
    initializedGetter: customFormValueGetter,
    currentGetter: customFormValueGetter,
  },
);

const [BasicModal, modalApi] = useVbenModal({
  fullscreenButton: false,
  onBeforeClose,
  onClosed: handleClosed,
  onConfirm: handleConfirm,
  onOpenChange: async (isOpen) => {
    if (!isOpen) {
      return null;
    }
    modalApi.modalLoading(true);

    const { id } = modalApi.getData() as { id: number | string };

    setupDeptTree(id);
    const record = await roleInfo(id);
    await formApi.setValues(record);
    markInitialized();

    modalApi.modalLoading(false);
  },
});

/**
 * 这里拿到的是一个数组ref
 */
const deptSelectRef = ref();

async function handleConfirm() {
  try {
    modalApi.lock(true);
    const { valid } = await formApi.validate();
    if (!valid) {
      return;
    }
    // formApi.getValues拿到的是一个readonly对象，不能直接修改，需要cloneDeep
    const data = cloneDeep(await formApi.getValues());
    // 不为自定义权限的话 删除部门id
    if (data.dataScope === 'CUSTOM') {
      const deptIds = deptSelectRef.value?.[0]?.getCheckedKeys() ?? [];
      data.deptIds = deptIds;
    } else {
      data.deptIds = [];
    }
    await roleDataScope(data);
    resetInitialized();
    emit('reload');
    modalApi.close();
  } catch (error) {
    console.error(error);
  } finally {
    modalApi.lock(false);
  }
}

async function handleClosed() {
  await formApi.resetForm();
  resetInitialized();
}

/**
 * 通过回调更新 无法通过v-model
 * @param value 菜单选择是否严格模式
 */
function handleCheckStrictlyChange(value: boolean) {
  formApi.setFieldValue('deptCheckStrictly', value);
}
</script>

<template>
  <BasicModal class="min-h-[600px] w-[550px]" title="分配权限">
    <BasicForm>
      <template #deptIds="slotProps">
        <TreeSelectPanel
          ref="deptSelectRef"
          v-bind="slotProps"
          :check-strictly="formApi.form.values.deptCheckStrictly"
          :expand-all-on-init="true"
          :field-names="{ key: 'id', title: 'deptName' }"
          :tree-data="deptTree"
          @check-strictly-change="handleCheckStrictlyChange"
        />
      </template>
    </BasicForm>
  </BasicModal>
</template>
