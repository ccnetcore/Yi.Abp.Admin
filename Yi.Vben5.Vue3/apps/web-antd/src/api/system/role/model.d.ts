export interface Role {
  id: string;
  creationTime: string;
  creatorId?: string | null;
  lastModifierId?: string | null;
  lastModificationTime?: string | null;
  isDeleted?: boolean;
  orderNum: number;
  state: boolean;
  roleName: string;
  roleCode: string;
  remark?: string | null;
  dataScope: string;
  menuIds?: string[];
  deptIds?: string[];
}

export interface DeptOption {
  id: string;
  parentId: string | null;
  orderNum: number;
  deptName: string;
  state: boolean;
  children?: DeptOption[] | null;
}

export interface DeptResp {
  checkedKeys: string[];
  depts: DeptOption[];
}
