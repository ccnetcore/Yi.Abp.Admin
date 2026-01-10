<script setup lang="ts">
import type { Role } from '#/api/system/user/model';

import { computed, h, onMounted, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';
import { $t } from '@vben/locales';
import { addFullName, cloneDeep, getPopupContainer } from '@vben/utils';

import { Tag } from 'ant-design-vue';

import { useVbenForm } from '#/adapter/form';
import { configInfoByKey } from '#/api/system/config';
import { postOptionSelect } from '#/api/system/post';
import { roleOptionSelect } from '#/api/system/role';
import {
  findUserInfo,
  getDeptTree,
  userAdd,
  userUpdate,
} from '#/api/system/user';
import { defaultFormValueGetter, useBeforeCloseDiff } from '#/utils/popup';
import { authScopeOptions } from '#/views/system/role/data';

import { drawerSchema } from './data';

const emit = defineEmits<{ reload: [] }>();

const isUpdate = ref(false);
const title = computed(() => {
  return isUpdate.value ? $t('pages.common.edit') : $t('pages.common.add');
});

const [BasicForm, formApi] = useVbenForm({
  commonConfig: {
    formItemClass: 'col-span-2',
    componentProps: {
      class: 'w-full',
    },
    labelWidth: 80,
  },
  schema: drawerSchema(),
  showDefaultActions: false,
  wrapperClass: 'grid-cols-2',
});

/**
 * 生成角色的自定义label
 * 也可以用option插槽来做
 * renderComponentContent: () => ({
    option: ({value, label, [disabled, key, title]}) => '',
  }),
 */
function genRoleOptionlabel(role: Role) {
  const found = authScopeOptions.find((item) => item.value === role.dataScope);
  if (!found) {
    return role.roleName;
  }
  return h('div', { class: 'flex items-center gap-[6px]' }, [
    h('span', null, role.roleName),
    h(Tag, { color: found.color }, () => found.label),
  ]);
}

/**
 * 根据部门ID加载岗位列表
 * @param deptId 部门ID
 */
async function setupPostOptions(deptId?: string) {
  if (!deptId) {
    // 没有选择部门时，显示提示
    formApi.updateSchema([
      {
        componentProps: {
          disabled: true,
          options: [],
          placeholder: '请先选择部门',
        },
        fieldName: 'postIds',
      },
    ]);
    // 清空已选岗位
    formApi.setFieldValue('postIds', []);
    return;
  }

  try {
    const postListResp = await postOptionSelect(deptId);
    // 确保返回的是数组
    const postList = Array.isArray(postListResp) ? postListResp : [];
    const options = postList.map((item) => ({
      label: item.postName,
      value: item.id,
    }));
    const placeholder = options.length > 0 ? '请选择岗位' : '该部门暂无岗位';
    formApi.updateSchema([
      {
        componentProps: {
          disabled: options.length === 0,
          options,
          placeholder,
        },
        fieldName: 'postIds',
      },
    ]);
    // 部门变化时清空已选岗位
    formApi.setFieldValue('postIds', []);
  } catch (error) {
    console.error('加载岗位信息失败:', error);
    formApi.updateSchema([
      {
        componentProps: {
          disabled: true,
          options: [],
          placeholder: '加载岗位失败',
        },
        fieldName: 'postIds',
      },
    ]);
  }
}

/**
 * 初始化部门选择
 */
async function setupDeptSelect() {
  try {
    // updateSchema
    const deptTree = await getDeptTree();
    // 确保返回的是数组
    const deptList = Array.isArray(deptTree) ? deptTree : [];
    // 选中后显示在输入框的值 即父节点 / 子节点
    addFullName(deptList, 'deptName', ' / ');
    formApi.updateSchema([
      {
        componentProps: {
          class: 'w-full',
          fieldNames: {
            label: 'deptName',
            key: 'id',
            value: 'id',
            children: 'children',
          },
          getPopupContainer,
          placeholder: '请选择',
          showSearch: true,
          treeData: deptList,
          treeDefaultExpandAll: true,
          treeLine: { showLeafIcon: false },
          // 筛选的字段
          treeNodeFilterProp: 'deptName',
          // 选中后显示在输入框的值
          treeNodeLabelProp: 'fullName',
          // 部门选择变化时加载对应岗位
          onChange: (value: string) => {
            setupPostOptions(value);
          },
        },
        fieldName: 'deptId',
      },
    ]);
  } catch (error) {
    console.error('加载部门树失败:', error);
    // 加载失败时设置空树
    formApi.updateSchema([
      {
        componentProps: {
          placeholder: '加载部门失败',
          treeData: [],
        },
        fieldName: 'deptId',
      },
    ]);
  }
}

const defaultPassword = ref('');
onMounted(async () => {
  const password = await configInfoByKey('sys.user.initPassword');
  if (password) {
    defaultPassword.value = password;
  }
});

/**
 * 新增时候 从参数设置获取默认密码
 */
async function loadDefaultPassword(update: boolean) {
  if (!update && defaultPassword.value) {
    formApi.setFieldValue('password', defaultPassword.value);
  }
}

const { onBeforeClose, markInitialized, resetInitialized } = useBeforeCloseDiff(
  {
    initializedGetter: defaultFormValueGetter(formApi),
    currentGetter: defaultFormValueGetter(formApi),
  },
);

const [BasicDrawer, drawerApi] = useVbenDrawer({
  onBeforeClose,
  onClosed: handleClosed,
  onConfirm: handleConfirm,
  async onOpenChange(isOpen) {
    if (!isOpen) {
      // 需要重置岗位选择
      formApi.updateSchema([
        {
          componentProps: {
            disabled: true,
            options: [],
            placeholder: '请先选择部门',
          },
          fieldName: 'postIds',
        },
      ]);
      return null;
    }
    drawerApi.drawerLoading(true);

    try {
      const { id } = drawerApi.getData() as { id?: number | string };
      isUpdate.value = !!id;
      /** update时 禁用用户名修改 不显示密码框 */
      formApi.updateSchema([
        { componentProps: { disabled: isUpdate.value }, fieldName: 'userName' },
        {
          dependencies: { show: () => !isUpdate.value, triggerFields: ['id'] },
          fieldName: 'password',
        },
      ]);

      let user: any | null = null;

      if (isUpdate.value && id) {
        // 编辑模式：从用户详情中获取用户信息（含岗位、角色ID）
        user = await findUserInfo(id);
      }

      // 角色下拉统一使用 roleOptionSelect
      const roleListResp = await roleOptionSelect();
      const allRoles = Array.isArray(roleListResp) ? (roleListResp as Role[]) : [];

      const userRoles = user?.roles ?? [];
      const posts = user?.posts ?? [];

      const postIds = posts.map((item: any) => item.id);
      const roleIds = userRoles.map((item: any) => item.roleId ?? item.id);

      const postOptions = posts.map((item: any) => ({
        label: item.postName,
        value: item.id,
      }));

      formApi.updateSchema([
        {
          componentProps: {
            // title用于选中后回填到输入框 默认为label
            optionLabelProp: 'title',
            options: allRoles.map((item: any) => ({
              label: genRoleOptionlabel(item),
              // title用于选中后回填到输入框 默认为label
              title: item.roleName,
              value: item.roleId ?? item.id,
            })),
          },
          fieldName: 'roleIds',
        },
      ]);

      // 部门选择、初始密码
      const promises = [
        setupDeptSelect(),
        loadDefaultPassword(isUpdate.value),
      ];

      if (user) {
        // 编辑模式：使用用户已有的岗位数据
        formApi.updateSchema([
          {
            componentProps: {
              disabled: false,
              options: postOptions,
              placeholder: '请选择岗位',
            },
            fieldName: 'postIds',
          },
        ]);
        // 处理用户数据，确保 phone 字段是字符串类型
        const userData = {
          ...user,
          // 将数字类型的 phone 转换为字符串，null/undefined 转为空字符串
          phone: user.phone != null ? String(user.phone) : '',
        };
        promises.push(
          // 添加基础信息
          formApi.setValues(userData),
          // 添加角色和岗位
          formApi.setFieldValue('postIds', postIds),
          formApi.setFieldValue('roleIds', roleIds),
        );
      } else {
        // 新增模式：等待选择部门后再加载岗位
        await setupPostOptions();
      }

      // 并行处理
      await Promise.all(promises);
      await markInitialized();
    } catch (error) {
      console.error('加载用户信息失败:', error);
    } finally {
      drawerApi.drawerLoading(false);
    }
  },
});

async function handleConfirm() {
  try {
    drawerApi.lock(true);
    const { valid } = await formApi.validate();
    if (!valid) {
      return;
    }
    const data = cloneDeep(await formApi.getValues());
    await (isUpdate.value ? userUpdate(data) : userAdd(data));
    resetInitialized();
    emit('reload');
    drawerApi.close();
  } catch (error) {
    console.error(error);
  } finally {
    drawerApi.lock(false);
  }
}

async function handleClosed() {
  formApi.resetForm();
  resetInitialized();
}
</script>

<template>
  <BasicDrawer :title="title" class="w-[600px]">
    <BasicForm />
  </BasicDrawer>
</template>
