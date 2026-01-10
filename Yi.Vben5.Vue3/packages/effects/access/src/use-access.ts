import { computed } from 'vue';

import { preferences, updatePreferences } from '@vben/preferences';
import { useAccessStore, useUserStore } from '@vben/stores';

/**
 * @zh_CN 超级管理员角色代码
 */
const SUPER_ADMIN_ROLE = 'admin';

/**
 * @zh_CN 超级管理员权限代码（拥有所有权限）
 */
const ALL_PERMISSION = '*:*:*';

function useAccess() {
  const accessStore = useAccessStore();
  const userStore = useUserStore();
  const accessMode = computed(() => {
    return preferences.app.accessMode;
  });

  /**
   * 基于角色判断是否有权限
   * @description: Determine whether there is permission，The role is judged by the user's role
   * @param roles
   */
  function hasAccessByRoles(roles: string[]) {
    const userRoleSet = new Set(userStore.userRoles);
    /**
     * 超级管理员角色拥有所有权限
     */
    if (userRoleSet.has(SUPER_ADMIN_ROLE)) {
      return true;
    }
    const intersection = roles.filter((item) => userRoleSet.has(item));
    return intersection.length > 0;
  }

  /**
   * 基于权限码判断是否有权限
   * @description: Determine whether there is permission，The permission code is judged by the user's permission code
   * @param codes
   */
  function hasAccessByCodes(codes: string[]) {
    const userCodesSet = new Set(accessStore.accessCodes);
    /**
     * 管理员权限
     */
    if (userCodesSet.has(ALL_PERMISSION)) {
      return true;
    }
    // 其他 判断是否存在
    const intersection = codes.filter((item) => userCodesSet.has(item));
    return intersection.length > 0;
  }

  /**
   * 验证用户是否含有指定权限，只需包含其中一个
   * @param codes 权限码数组
   * @returns 如果用户至少拥有其中一个权限则返回 true，否则返回 false
   */
  function hasAccessByCodesOr(codes: string[]) {
    return codes.some((code) => hasAccessByCodes([code]));
  }

  /**
   * 验证用户是否含有指定权限，必须全部拥有
   * @param codes 权限码数组
   * @returns 如果用户拥有所有权限则返回 true，否则返回 false
   */
  function hasAccessByCodesAnd(codes: string[]) {
    return codes.every((code) => hasAccessByCodes([code]));
  }

  /**
   * 验证用户是否含有指定角色，只需包含其中一个
   * @param roles 角色数组
   * @returns 如果用户至少拥有其中一个角色则返回 true，否则返回 false
   */
  function hasAccessByRolesOr(roles: string[]) {
    return roles.some((role) => hasAccessByRoles([role]));
  }

  /**
   * 验证用户是否含有指定角色，必须全部拥有
   * @param roles 角色数组
   * @returns 如果用户拥有所有角色则返回 true，否则返回 false
   */
  function hasAccessByRolesAnd(roles: string[]) {
    return roles.every((role) => hasAccessByRoles([role]));
  }

  async function toggleAccessMode() {
    updatePreferences({
      app: {
        accessMode:
          preferences.app.accessMode === 'frontend' ? 'backend' : 'frontend',
      },
    });
  }

  return {
    accessMode,
    hasAccessByCodes,
    hasAccessByCodesAnd,
    hasAccessByCodesOr,
    hasAccessByRoles,
    hasAccessByRolesAnd,
    hasAccessByRolesOr,
    toggleAccessMode,
  };
}

export { useAccess };
