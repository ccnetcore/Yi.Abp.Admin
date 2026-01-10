import type { Dept } from '../dept/model';

/**
 * @description: 用户导入
 * @param updateSupport 是否覆盖数据
 * @param file excel文件
 */
export interface UserImportParam {
  updateSupport: boolean;
  file: Blob | File;
}

/**
 * @description: 重置密码
 */
export interface ResetPwdParam {
  id: string;
  password: string;
}

export interface Role {
  roleId: string;
  roleName: string;
  roleKey: string;
  roleSort: number;
  dataScope: string;
  menuCheckStrictly?: boolean;
  deptCheckStrictly?: boolean;
  status: string;
  remark: string;
  createTime?: string;
  flag: boolean;
  superAdmin: boolean;
}

export interface User {
  id: string;
  isDeleted: boolean;
  name?: string | null;
  age?: number | null;
  userName: string;
  icon?: string | null;
  nick?: string | null;
  email?: string | null;
  ip?: string | null;
  address?: string | null;
  phone?: number | null;
  introduction?: string | null;
  remark?: string | null;
  sex: string; // SexEnum
  deptId?: string | null;
  creationTime: string;
  creatorId?: string | null;
  lastModifierId?: string | null;
  lastModificationTime?: string | null;
  orderNum: number;
  state: boolean;
  deptName?: string | null;
  posts?: Post[];
  roles?: Role[];
  dept?: Dept | null;
}

export interface Post {
  postId: number;
  postCode: string;
  postName: string;
  postSort: number;
  status: string;
  remark: string;
  createTime: string;
}

/**
 * @description 用户信息
 * @param user 用户个人信息
 * @param roleIds 角色IDS 不传id为空
 * @param roles 所有的角色
 * @param postIds 岗位IDS 不传id为空
 * @param posts 所有的岗位
 */
export interface UserInfoResponse {
  user?: User;
  roleIds?: string[];
  roles: Role[];
  postIds?: number[];
  posts?: Post[];
}

/**
 * @description: 部门树
 */
export interface DeptTreeData {
  id: string;
  parentId: string;
  orderNum: number;
  deptName: string;
  state: boolean;
  children?: DeptTreeData[] | null;
}
