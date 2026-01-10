<!--
 Access control component for fine-grained access control.
 支持的功能：
 1. 支持多个权限码，只要有一个权限码满足即可（mode: 'or'）或者多个权限码全部满足（mode: 'and'）
 2. 支持多个角色，只要有一个角色满足即可（mode: 'or'）或者多个角色全部满足（mode: 'and'）
 3. 自动检测超级管理员权限（*:*:*）和超级管理员角色（admin）
-->
<script lang="ts" setup>
import { computed } from 'vue';

import { useAccess } from './use-access';

interface Props {
  /**
   * Specified codes is visible
   * @default []
   */
  codes?: string[];

  /**
   * 判断模式：or-只要有一个满足即可，and-必须全部满足
   * @default 'or'
   */
  mode?: 'and' | 'or';

  /**
   * 通过什么方式来控制组件，如果是 role，则传入角色，如果是 code，则传入权限码
   * @default 'code'
   */
  type?: 'code' | 'role';
}

defineOptions({
  name: 'AccessControl',
});

const props = withDefaults(defineProps<Props>(), {
  codes: () => [],
  mode: 'or',
  type: 'code',
});

const {
  hasAccessByCodes,
  hasAccessByCodesAnd,
  hasAccessByCodesOr,
  hasAccessByRoles,
  hasAccessByRolesAnd,
  hasAccessByRolesOr,
} = useAccess();

const hasAuth = computed(() => {
  const { codes, mode, type } = props;

  if (type === 'role') {
    if (mode === 'and') {
      return hasAccessByRolesAnd(codes);
    }
    if (mode === 'or') {
      return hasAccessByRolesOr(codes);
    }
    return hasAccessByRoles(codes);
  }

  if (mode === 'and') {
    return hasAccessByCodesAnd(codes);
  }
  if (mode === 'or') {
    return hasAccessByCodesOr(codes);
  }
  return hasAccessByCodes(codes);
});
</script>

<template>
  <slot v-if="!codes || codes.length === 0"></slot>
  <slot v-else-if="hasAuth"></slot>
</template>
